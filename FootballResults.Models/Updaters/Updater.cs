﻿using Extensions;
using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Models;
using FootballResults.Models.Api;
using FootballResults.Models.Api.FootballApi.Exceptions;
using FootballResults.Models.Api.FootballApi.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace FootballResults.Models.Updaters
{
    public abstract class Updater<TResponse, TResponseItem> : IUpdater where TResponse : GeneralResponse<TResponseItem>
    {
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly ILogger _logger;
        protected readonly IConfiguration _configuration;
        protected AppDbContext _dbContext = default!;
        protected WebApiClient _webApiClient;
        protected UpdaterMode _currentMode;

        protected ApiConfig _apiConfig;
        protected ApplicationConfig _applicationConfig;
        protected IEnumerable<EndpointConfig> _endpointConfigs;

        protected virtual EndpointConfig UpdaterSpecificSettings { get; } = null;
        protected virtual EndpointConfig UpdaterSpecificSettingsForDate { get; } = null;
        protected virtual EndpointConfig UpdaterSpecificSettingsForTeam { get; } = null;
        protected virtual EndpointConfig UpdaterSpecificSettingsForLeagueAndSeason { get; } = null;

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

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                _apiConfig = _dbContext.ApiConfig.OrderBy(i => i.ID).FirstOrDefault();
                _applicationConfig = _dbContext.ApplicationConfig.OrderBy(i => i.ID).FirstOrDefault();
                _endpointConfigs = _dbContext.EndpointConfig.ToList();
            }

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
                case UpdaterMode.ActiveLeaguesAllSeasons:
                    await UpdateForAllLeaguesAllSeasonsAsync(activeLeaguesOnly: true);
                    break;
                case UpdaterMode.ActiveLeaguesCurrentSeason:
                    await UpdateForAllLeaguesCurrentSeasonAsync(activeLeaguesOnly: true);
                    break;
                case UpdaterMode.ActiveLeaguesSpecificSeason:
                    if (modeParameters == null || modeParameters.Length == 0 || modeParameters[0].GetType() != typeof(int))
                        throw new InvalidOperationException("No parameters provided for active leagues specific season mode");
                    await UpdateForAllLeaguesSpecificSeasonAsync((int)modeParameters[0], activeLeaguesOnly: true);
                    break;
                case UpdaterMode.SpecificLeagueCurrentSeason:
                    if (modeParameters == null || modeParameters.Length == 0 || modeParameters[0].GetType() != typeof(int))
                        throw new InvalidOperationException("No parameters provided for specific league current season mode");
                    await UpdateForSpecificLeagueCurrentSeasonAsync((int)modeParameters[0]);
                    break;
                case UpdaterMode.SpecificLeagueAllSeasons:
                    if (modeParameters == null || modeParameters.Length == 0 || modeParameters[0].GetType() != typeof(int))
                        throw new InvalidOperationException("No parameters provided for specific league all seasons mode");
                    await UpdateForSpecificLeagueAllSeasonsAsync((int)modeParameters[0]);
                    break;
                case UpdaterMode.SpecificDate:
                case UpdaterMode.SpecificDateActiveLeagues:
                    if (modeParameters == null || modeParameters.Length == 0 || modeParameters[0].GetType() != typeof(DateTime))
                        throw new InvalidOperationException("No parameters provided for specific date mode");
                    await UpdateForSpecificDateAsync((DateTime)modeParameters[0]);
                    break;
                case UpdaterMode.CurrentDate:
                case UpdaterMode.CurrentDateActiveLeagues:
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
                case UpdaterMode.SpecificCountryAllTeams:
                    if (modeParameters == null || modeParameters.Length == 0 || modeParameters[0].GetType() != typeof(int))
                        throw new InvalidOperationException("No parameters provided for specific country all teams mode");
                    await UpdateForSpecificCountryAllTeamsAsync((int)modeParameters[0]);
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

        protected virtual async Task UpdateForAllLeaguesAllSeasonsAsync(bool activeLeaguesOnly = false)
        {
            _logger.LogInformation($"{GetType().Name} starting...");
            
            if (UpdaterSpecificSettingsForLeagueAndSeason == null)
                throw new InvalidOperationException("Updater settings missing");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                List<DataAccess.Entities.Football.League> leagues = _dbContext.Leagues
                    .Where(league => activeLeaguesOnly ? league.UpdatesActive : true)
                    .Include(league => league.LeagueSeasons)
                    .ToList();

                foreach (var league in leagues)
                {
                    foreach (var season in league.LeagueSeasons.OrderBy(s => s.Year))
                    {
                        await UpdateForLeagueAndSeasonAsync(league.ID, season.Year);
                        await DelayApiCallAsync();
                    }
                }
            }

            _logger.LogInformation($"{GetType().Name} has finished");
        }

        protected virtual async Task UpdateForAllLeaguesCurrentSeasonAsync(bool activeLeaguesOnly = false)
        {
            _logger.LogInformation($"{GetType().Name} starting...");
            
            if (UpdaterSpecificSettingsForLeagueAndSeason == null)
                throw new InvalidOperationException("Updater settings missing");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                List<DataAccess.Entities.Football.League> leagues = _dbContext.Leagues
                    .Where(league => activeLeaguesOnly ? league.UpdatesActive : true)
                    .Include(league => league.LeagueSeasons)
                    .ToList();

                foreach (var league in leagues)
                {
                    DataAccess.Entities.Football.LeagueSeason currentSeason = league.LeagueSeasons.FirstOrDefault(season => season.InProgress);

                    if (currentSeason != null)
                    {
                        await UpdateForLeagueAndSeasonAsync(league.ID, currentSeason.Year);
                        await DelayApiCallAsync();
                    }
                }
            }

            _logger.LogInformation($"{GetType().Name} has finished");
        }

        protected virtual async Task UpdateForAllLeaguesSpecificSeasonAsync(int year, bool activeLeaguesOnly = false)
        {
            _logger.LogInformation($"{GetType().Name} starting...");
            
            if (UpdaterSpecificSettingsForLeagueAndSeason == null)
                throw new InvalidOperationException("Updater settings missing");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                List<DataAccess.Entities.Football.League> leagues = _dbContext.Leagues
                    .Where(league => activeLeaguesOnly ? league.UpdatesActive : true)
                    .Include(league => league.LeagueSeasons)
                    .ToList();

                foreach (var league in leagues)
                {
                    DataAccess.Entities.Football.LeagueSeason specificSeason = league.LeagueSeasons.FirstOrDefault(season => season.Year == year);

                    if (specificSeason != null)
                    {
                        await UpdateForLeagueAndSeasonAsync(league.ID, specificSeason.Year);
                        await DelayApiCallAsync();
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

        protected virtual async Task UpdateForSpecificLeagueAllSeasonsAsync(int leagueID)
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

                if (league == null)
                    throw new InvalidOperationException("League not found");

                foreach (var season in league.LeagueSeasons.OrderBy(s => s.Year))
                {
                    await UpdateForLeagueAndSeasonAsync(league.ID, season.Year);
                    await DelayApiCallAsync();
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

        protected virtual async Task UpdateForSpecificCountryAllTeamsAsync(int countryID)
        {
            _logger.LogInformation($"{GetType().Name} starting...");

            if (UpdaterSpecificSettingsForTeam == null)
                throw new InvalidOperationException("Updater settings missing");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                Country country = _dbContext.Countries.FirstOrDefault(country => country.ID == countryID);

                if (country == null)
                {
                    _logger.LogWarning($"Country with ID {countryID} does not exist in the database.");
                }

                foreach (var team in _dbContext.Teams.Where(team => team.CountryID == country.ID))
                {
                    await UpdateForTeamAsync(team.ID);
                    await DelayApiCallAsync();
                }
            }

            _logger.LogInformation($"{GetType().Name} has finished");

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
            var requestHeaders = await _apiConfig.GetRequestHeadersAsync(_configuration.GetSection(Defaults.FootballApiKeyEncryptionKey).Value);

            _logger.LogInformation("API fetch in progress...");
            var response = await _webApiClient.GetAsync<TResponse>(endpoint, requestHeaders);
            _logger.LogInformation("Response received. Checking response...");

            return response;
        }

        protected virtual void BackupData(TResponse response, string backupPath)
        {
            if (response != null && _apiConfig.BackupData)
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
                foreach (var item in response.Errors)
                {
                    switch (item.Key)
                    {
                        case FootballApiErrorType.MISSING_TOKEN:
                            throw new MissingApiKeyException(item.Value);
                        case FootballApiErrorType.OUT_OF_QUOTA:
                            throw new OutOfQuotaException(item.Value);
                        default:
                            sb.AppendLine(item.Value);
                            break;
                    }
                }

                sb.AppendLine("API response contains errors:");
                throw new Exception(sb.ToString());
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
                    ProcessData(data, _currentMode);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation("Processing completed, the database has been successfully updated!");
                }
                catch (Exception)
                {   _logger.LogError("An error has occured while processing response. Changes will be unmade.");
                    _dbContext.ChangeTracker.Clear();
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public void ProcessData(AppDbContext context, IEnumerable<TResponseItem> data, UpdaterMode? mode = null)
        {
            _dbContext = context;
            ProcessData(data, mode);
        }

        protected abstract void ProcessData(IEnumerable<TResponseItem> data, UpdaterMode? mode = null);

        public async Task DelayApiCallAsync()
        {
            double secondsToWait = _apiConfig.RateLimit.HasValue && _apiConfig.RateLimit.Value > 0 ? (60.0 / _apiConfig.RateLimit.Value) : 0;
            int milliSecondsToWait = (int)Math.Ceiling(secondsToWait * 1000);
            _logger.LogInformation($"Delaying next API call for {milliSecondsToWait} milliseconds...");
            await Task.Delay(milliSecondsToWait);
        }
    }
}