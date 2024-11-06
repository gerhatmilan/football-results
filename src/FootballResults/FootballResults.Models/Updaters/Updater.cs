using Extensions;
using FootballResults.DataAccess;
using FootballResults.Models.Api;
using FootballResults.Models.Api.FootballApi.Exceptions;
using FootballResults.Models.Api.FootballApi.Responses;
using FootballResults.Models.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace FootballResults.Models.Updaters
{
    public abstract class Updater<TResponse, TResponseItem> : IUpdater where TResponse : GeneralResponse<TResponseItem>
    {
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly ILogger _logger;
        protected readonly FootballApiConfig _apiConfig;
        protected readonly ApplicationConfig _applicationConfig;
        protected AppDbContext _dbContext = default!;
        protected WebApiClient _webApiClient;
        protected UpdaterMode _currentMode;

        protected virtual UpdaterSpecificSettings UpdaterSpecificSettings { get; } = null;
        protected virtual UpdaterSpecificSettings UpdaterSpecificSettingsForDate { get; } = null;
        protected virtual UpdaterSpecificSettings UpdaterSpecificSettingsForTeam { get; } = null;
        protected virtual UpdaterSpecificSettings UpdaterSpecificSettingsForLeagueAndSeason { get; } = null;

        public IEnumerable<UpdaterMode> SupportedModes
        {
            get
            {
                SupportedModesAttribute attribute = GetType().GetCustomAttribute<SupportedModesAttribute>();

                if (attribute == null)
                    return new List<UpdaterMode>();
                else
                    return attribute.SupportedModes;
            }
        }

        public Updater(IServiceScopeFactory serviceScopeFactory, ILogger logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _apiConfig = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOptions<FootballApiConfig>>().Value;
            _applicationConfig = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOptions<ApplicationConfig>>().Value;
            _webApiClient = new WebApiClient(_apiConfig.BaseAddress, logger);
        }

        private bool ModeSupported(UpdaterMode mode)
        {
            return SupportedModes.Contains(mode);
        }

        public async Task StartAsync()
        {
            await UpdateAsync();
        }

        public async Task StartAsync(UpdaterMode mode, params object[] modeParameters)
        {
            if (!ModeSupported(mode))
                throw new InvalidOperationException("Mode not supported");

            _currentMode = mode;

            switch (mode)
            {
                case UpdaterMode.Classic:
                    await UpdateAsync();
                    break;
                case UpdaterMode.AllLeaguesAllSeasons:
                    await UpdateForAllLeaguesAllSeasonsAsync();
                    break;
                case UpdaterMode.AllLeaguesCurrentSeason:
                    await UpdateForAllLeaguesCurrentSeasonAsync();
                    break;
                case UpdaterMode.AllLeaguesSpecificSeason:
                    if (modeParameters == null || modeParameters.Length == 0 || modeParameters[0].GetType() != typeof(int))
                        throw new InvalidOperationException("No parameters provided for all leagues specific season mode");
                    await UpdateForAllLeaguesSpecificSeasonAsync((int)modeParameters[0]);
                    break;
                case UpdaterMode.SpecificLeagueCurrentSeason:
                    if (modeParameters == null || modeParameters.Length == 0 || modeParameters[0].GetType() != typeof(int))
                        throw new InvalidOperationException("No parameters provided for specific league current season mode");
                    await UpdateForSpecificLeagueCurrentSeasonAsync((int)modeParameters[0]);
                    break;
                case UpdaterMode.SpecificDate:
                    if (modeParameters == null || modeParameters.Length == 0 || modeParameters[0].GetType() != typeof(DateTime))
                        throw new InvalidOperationException("No parameters provided for specific date mode");
                    await UpdateForSpecificDateAsync((DateTime)modeParameters[0]);
                    break;
                case UpdaterMode.CurrentDate:
                    await UpdateForSpecificDateAsync(DateTime.UtcNow);
                    break;
                case UpdaterMode.BasedOnLastUpdate:
                    if (modeParameters == null || modeParameters.Length == 0 || modeParameters[0].GetType() != typeof(TimeSpan))
                        throw new InvalidOperationException("No parameters provided for based on last update mode");
                    await UpdateBasedOnLastUpdateAsync((TimeSpan)modeParameters[0]);
                    break;
                case UpdaterMode.SpecificTeam:
                    if (modeParameters == null || modeParameters.Length == 0 || modeParameters[0].GetType() != typeof(int))
                        throw new InvalidOperationException("No parameters provided for specific team mode");
                    await UpdateForSpecificTeamAsync((int)modeParameters[0]);
                    break;
                default:
                    throw new InvalidOperationException("Invalid mode");
            }
        }

        protected string GetEndpointFilledWithParameters(string endpoint, params object[] parameters)
        {

            return string.Format(endpoint, parameters);
        }

        protected string GetBackupPathFilledWithParameters(string backupPath, params object[] parameters)
        {
            return string.Format(backupPath, parameters);
        }

        protected ICollection<int> GetIncludedLeagueIDs()
        {
            return _applicationConfig.IncludedLeagues.Select(includedLeague => includedLeague.ID).ToList();
        }

        protected virtual async Task UpdateAsync()
        {
            _logger.LogInformation($"{GetType().Name} starting...");
            
            if (UpdaterSpecificSettings == null)
                throw new InvalidOperationException("Updater settings missing");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (UpdaterSpecificSettings.LoadDataFromBackup)
                {
                    IEnumerable<TResponseItem> data = LoadDataFromBackup(UpdaterSpecificSettings.BackupPath);

                    if (data != null)
                    {
                        await ProcessAsync(data);
                    }
                }
                else
                {
                    TResponse response = await FetchDataAsync(UpdaterSpecificSettings.Endpoint);

                    if (HandleErrors(response))
                    {
                        BackupData(response, UpdaterSpecificSettings.BackupPath);
                        await ProcessAsync(response!.Response);
                    }
                }
            }

            _logger.LogInformation($"{GetType().Name} has finished");
        }

        protected virtual async Task UpdateForAllLeaguesAllSeasonsAsync()
        {
            _logger.LogInformation($"{GetType().Name} starting...");
            
            if (UpdaterSpecificSettingsForLeagueAndSeason == null)
                throw new InvalidOperationException("Updater settings missing");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                List<DataAccess.Entities.Football.League> leagues = _dbContext.Leagues
                .Include(league => league.LeagueSeasons)
                .ToList();

                foreach (var league in leagues)
                {
                    foreach (var season in league.LeagueSeasons.OrderBy(s => s.Year))
                    {
                        await UpdateForLeagueAndSeasonAsync(league.ID, season.Year);
                    }
                }
            }

            _logger.LogInformation($"{GetType().Name} has finished");
        }

        protected virtual async Task UpdateForAllLeaguesCurrentSeasonAsync()
        {
            _logger.LogInformation($"{GetType().Name} starting...");
            
            if (UpdaterSpecificSettingsForLeagueAndSeason == null)
                throw new InvalidOperationException("Updater settings missing");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                List<DataAccess.Entities.Football.League> leagues = _dbContext.Leagues
                .Include(league => league.LeagueSeasons)
                .ToList();

                foreach (var league in leagues)
                {
                    DataAccess.Entities.Football.LeagueSeason currentSeason = league.LeagueSeasons.FirstOrDefault(season => season.InProgress);

                    if (currentSeason != null)
                    {
                        await UpdateForLeagueAndSeasonAsync(league.ID, currentSeason.Year);
                    }
                }
            }

            _logger.LogInformation($"{GetType().Name} has finished");
        }

        protected virtual async Task UpdateForAllLeaguesSpecificSeasonAsync(int year)
        {
            _logger.LogInformation($"{GetType().Name} starting...");
            
            if (UpdaterSpecificSettingsForLeagueAndSeason == null)
                throw new InvalidOperationException("Updater settings missing");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                List<DataAccess.Entities.Football.League> leagues = _dbContext.Leagues
                .Include(league => league.LeagueSeasons)
                .ToList();

                foreach (var league in leagues)
                {
                    DataAccess.Entities.Football.LeagueSeason specificSeason = league.LeagueSeasons.FirstOrDefault(season => season.Year == year);

                    if (specificSeason != null)
                    {
                        await UpdateForLeagueAndSeasonAsync(league.ID, specificSeason.Year);
                    }
                }
            }

            _logger.LogInformation($"{GetType().Name} has finished");
        }

        protected virtual async Task UpdateForSpecificLeagueCurrentSeasonAsync(int leagueID)
        {
            _logger.LogInformation($"{GetType().Name} starting...");
            
            if (UpdaterSpecificSettingsForLeagueAndSeason == null)
                throw new InvalidOperationException("Updater settings missing");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                DataAccess.Entities.Football.League league = _dbContext.Leagues
                    .Include(league => league.LeagueSeasons)
                    .FirstOrDefault(league => league.ID.Equals(leagueID));

                DataAccess.Entities.Football.LeagueSeason currentSeason = _dbContext.LeagueSeasons.FirstOrDefault(season => season.LeagueID == leagueID && season.InProgress);

                if (league == null)
                    throw new InvalidOperationException("League not found");

                if (currentSeason == null)
                {
                    _logger.LogInformation("No season in progress for this league.");
                    return;
                }
                else
                {
                    await UpdateForLeagueAndSeasonAsync(league.ID, currentSeason.Year);
                }
            }

            _logger.LogInformation($"{GetType().Name} has finished");
        }

        private async Task UpdateForLeagueAndSeasonAsync(int leagueID, int year)
        {
            if (UpdaterSpecificSettingsForLeagueAndSeason == null)
                throw new InvalidOperationException("Updater settings missing");

            if (UpdaterSpecificSettingsForLeagueAndSeason.LoadDataFromBackup)
            {
                IEnumerable<TResponseItem> data = LoadDataFromBackup(GetBackupPathFilledWithParameters(UpdaterSpecificSettingsForLeagueAndSeason.BackupPath, year, leagueID));

                if (data != null)
                {
                    await ProcessAsync(data);
                }
            }
            else
            {
                TResponse response = await FetchDataAsync(GetEndpointFilledWithParameters(UpdaterSpecificSettingsForLeagueAndSeason.Endpoint, leagueID, year));

                if (HandleErrors(response))
                {
                    BackupData(response, GetBackupPathFilledWithParameters(UpdaterSpecificSettingsForLeagueAndSeason.BackupPath, year, leagueID));
                    await ProcessAsync(response!.Response);
                }

                // to avoid rate limiting
                await DelayApiCallAsync();
            }
        }

        protected virtual async Task UpdateForSpecificDateAsync(DateTime date)
        {
            _logger.LogInformation($"{GetType().Name} starting...");
            
            if (UpdaterSpecificSettingsForDate == null)
                throw new InvalidOperationException("Updater settings missing");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                string dateFormatted = date.ToString("yyyy-MM-dd");
                string backupPath = GetBackupPathFilledWithParameters(UpdaterSpecificSettingsForDate.BackupPath, dateFormatted);
                string endpoint = GetEndpointFilledWithParameters(UpdaterSpecificSettingsForDate.Endpoint, dateFormatted);

                if (UpdaterSpecificSettingsForDate.LoadDataFromBackup)
                {
                    IEnumerable<TResponseItem> data = LoadDataFromBackup(backupPath);

                    if (data != null)
                    {
                        await ProcessAsync(data);
                    }
                }
                else
                {
                    TResponse response = await FetchDataAsync(endpoint);

                    if (HandleErrors(response))
                    {
                        BackupData(response, backupPath);
                        await ProcessAsync(response!.Response);
                    }
                }
            }

            _logger.LogInformation($"{GetType().Name} has finished");
        }

        protected virtual async Task UpdateForSpecificTeamAsync(int teamID)
        {
            _logger.LogInformation($"{GetType().Name} starting...");
            await UpdateForTeamAsync(teamID);
            _logger.LogInformation($"{GetType().Name} has finished");
        }


        protected virtual async Task UpdateForTeamAsync(int teamID)
        {
            if (UpdaterSpecificSettingsForTeam == null)
                throw new InvalidOperationException("Updater settings missing");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                DataAccess.Entities.Football.Team existingTeam = _dbContext.Teams.FirstOrDefault(team => team.ID == teamID);

                if (existingTeam == null)
                {
                    _logger.LogWarning($"Team with ID {teamID} does not exist in the database.");
                }
                else
                {
                    string backupPath = GetBackupPathFilledWithParameters(UpdaterSpecificSettingsForTeam.BackupPath, teamID);
                    string endpoint = GetEndpointFilledWithParameters(UpdaterSpecificSettingsForTeam.Endpoint, teamID);

                    if (UpdaterSpecificSettingsForTeam.LoadDataFromBackup)
                    {
                        IEnumerable<TResponseItem> data = LoadDataFromBackup(backupPath);

                        if (data != null)
                        {
                            await ProcessAsync(data);
                        }
                    }
                    else
                    {
                        TResponse response = await FetchDataAsync(endpoint);

                        if (HandleErrors(response))
                        {
                            BackupData(response, backupPath);
                            await ProcessAsync(response!.Response);
                        }
                    }
                }
            }
        }

        protected virtual Task UpdateBasedOnLastUpdateAsync(TimeSpan maximumElapsedTimeSinceLastUpdate) { return Task.CompletedTask; }

        protected virtual IEnumerable<TResponseItem> LoadDataFromBackup(string backupPath)
        {
            if (!File.Exists(backupPath))
            {
                _logger.LogWarning("Backup file does not exist");
                return null;
            }

            string json = File.ReadAllText(backupPath);
            _logger.LogInformation($"Data loaded from {backupPath}. Processing...");

            return JsonConvert.DeserializeObject<IEnumerable<TResponseItem>>(json);
        }

        protected virtual async Task<TResponse> FetchDataAsync(string endpoint)
        {
            _logger.LogInformation("API fetch in progress...");
            var response = await _webApiClient.GetAsync<TResponse>(endpoint, _apiConfig.RequestHeaders);
            _logger.LogInformation("Response received. Checking response...");

            return response;
        }

        protected virtual void BackupData(TResponse response, string backupPath)
        {
            if (response != null && _apiConfig.DataFetch.ShouldBackupData)
            {
                FileExtensions.WriteAllText(backupPath, JsonConvert.SerializeObject(response.Response, Formatting.Indented), createDirectory: true);
                _logger.LogInformation($"Data backup saved to {backupPath}");
            }
        }

        protected virtual bool HandleErrors(TResponse response)
        {
            if (response == null)
                throw new Exception("API response deserialization failed");
            else if (response.Errors.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("API response contains errors:");
                foreach (var item in response.Errors)
                {
                    sb.AppendLine(item.Value);
                }

                throw new MissingApiKeyException(sb.ToString());
            }
            else
            {
                _logger.LogInformation("API fetch has completed successfully. Processing response...");
            }

            return true;
        }

        protected virtual async Task ProcessAsync(IEnumerable<TResponseItem> data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    ProcessData(data);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation("Processing completed, the database has been successfully updated!");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    _logger.LogError("An error has occured while processing response. Changes will be unmade.");
                    _dbContext.ChangeTracker.Clear();
                    await transaction.RollbackAsync();
                }
            }
        }

        public void ProcessData(AppDbContext context, IEnumerable<TResponseItem> data)
        {
            _dbContext = context;
            ProcessData(data);
        }

        protected abstract void ProcessData(IEnumerable<TResponseItem> data);

        protected async Task DelayApiCallAsync()
        {
            double secondsToWait = 60.0 / _apiConfig.RateLimit;
            int milliSecondsToWait = (int)Math.Ceiling(secondsToWait * 1000);
            _logger.LogInformation($"Delaying next API call for {milliSecondsToWait} milliseconds...");
            await Task.Delay(milliSecondsToWait);
        }
    }
}