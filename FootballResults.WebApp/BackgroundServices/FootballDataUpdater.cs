using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Models;
using FootballResults.Models.Api.FootballApi.Exceptions;
using FootballResults.Models.Updaters;
using FootballResults.WebApp.Hubs;
using FootballResults.WebApp.Services.Application;
using FootballResults.WebApp.Services.LiveUpdates;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp.BackgroundServices
{
    public class FootballDataUpdater : BackgroundService
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger<FootballDataUpdater> _logger;
        protected readonly ApplicationConfig _applicationConfig;
        protected readonly IHubContext<MessageHub<UpdateMessageType>> _notificationHubContext;

        protected static IEnumerable<string> MatchStatusesThatRequireUpdate { get; } = new List<string> { MatchStatus.NotStarted, MatchStatus.FirstHalf, MatchStatus.HalfTime, MatchStatus.SecondHalf, MatchStatus.ExtraTime, MatchStatus.BreakTime, MatchStatus.PenaltiesInProgress, MatchStatus.Suspended, MatchStatus.Interrupted, MatchStatus.Live };

        public FootballDataUpdater(IServiceProvider serviceProvider, ILogger<FootballDataUpdater> logger, IHubContext<MessageHub<UpdateMessageType>> notificationHubContext)
        {
            _serviceProvider = serviceProvider;
            _notificationHubContext = notificationHubContext;
            _logger = logger;

            using (var scope = _serviceProvider.CreateScope())
            {
                IApplicationService applicationService = scope.ServiceProvider.GetRequiredService<IApplicationService>();
                _applicationConfig = applicationService.GetApplicationConfigAsync().Result;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (PeriodicTimer timer = new PeriodicTimer(_applicationConfig.UpdateWorkerFrequency))
            {
                do
                {
                    _logger.LogInformation($"Football data update worker started");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                        {
                            try
                            {
                                await dbContext.Entry(_applicationConfig).ReloadAsync();

                                await UpdateMatchesForCurrentSeasonAsync(dbContext);
                                await UpdateStandingsForCurrentSeasonAsync(dbContext);
                                await UpdateTopScorersForCurrentSeasonAsync(dbContext);
                                await UpdateMatchesForCurrentDayAsync(dbContext);
                            }
                            catch (ApiException ex)
                            {
                                _logger.LogError(ex.Message);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred while updating data");
                            }
                        }
                    }

                    _logger.LogInformation($"Football data update worker finished");
                }
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
            }
        }

        protected bool ShouldUpdate(DateTime? lastUpdate, TimeSpan maximumPassedTime)
        {
            return !lastUpdate.HasValue || lastUpdate.Value.Add(maximumPassedTime) < DateTime.UtcNow;
        }

        protected bool MatchMightBeInProgress(AppDbContext dbContext)
        {
            return dbContext.Matches
                .Any(i =>
                    i.Date != null
                    && i.Date < DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                    && MatchStatusesThatRequireUpdate.Contains(i.Status)
                );
        }

        protected async Task UpdateMatchesForCurrentDayAsync(AppDbContext dbContext)
        {
            DateTime? lastUpdate = dbContext.SystemInformation.Find(1)?.MatchesLastUpdateForCurrentDay;

            if (ShouldUpdate(lastUpdate, _applicationConfig.MatchUpdateForCurrentDayFrequency)
                && MatchMightBeInProgress(dbContext))
            {
                try
                {
                    MatchUpdater matchUpdater = (MatchUpdater)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(MatchUpdater));
                    await matchUpdater.StartAsync(UpdaterMode.CurrentDateActiveLeagues);

                    // notify connected clients about the update
                    await _notificationHubContext.Clients.All.SendAsync("ReceiveMessage", UpdateMessageType.MatchesUpdated);
                }
                catch (Exception ex) when (ex is not ApiException)
                {
                    _logger.LogError($"Football data update worker for matches failed: {ex.Message}");
                }
            }
        }

        protected async Task UpdateMatchesForCurrentSeasonAsync(AppDbContext dbContext)
        {
            ICollection<LeagueSeason> currentLeagueSeasons = await dbContext.LeagueSeasons
                .Include(ls => ls.League)
                .Where(i => i.InProgress && i.League.UpdatesActive)
                .ToListAsync();

            foreach (var leagueSeason in currentLeagueSeasons)
            {
                DateTime? lastUpdate = leagueSeason.MatchesLastUpdate;

                if (ShouldUpdate(lastUpdate, _applicationConfig.MatchUpdateForCurrentSeasonFrequency))
                {
                    try
                    {
                        MatchUpdater matchUpdater = (MatchUpdater)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(MatchUpdater));
                        await matchUpdater.StartAsync(UpdaterMode.SpecificLeagueCurrentSeason, leagueSeason.LeagueID);

                        // notify connected clients about the update
                        await _notificationHubContext.Clients.All.SendAsync("ReceiveMessage", UpdateMessageType.MatchesUpdated);

                        // delay next call
                        await matchUpdater.DelayApiCallAsync();
                    }
                    catch (Exception ex) when (ex is not ApiException)
                    {
                        _logger.LogError($"Football data update worker for matches failed to execute for {leagueSeason.League.Name} / {leagueSeason.Year}: {ex.Message}");
                    }
                }
            }
        }

        protected async Task UpdateStandingsForCurrentSeasonAsync(AppDbContext dbContext)
        {
            ICollection<LeagueSeason> currentLeagueSeasons = await dbContext.LeagueSeasons
                .Include(ls => ls.League)
                .Where(i => i.InProgress && i.League.UpdatesActive)
                .ToListAsync();

            foreach (var leagueSeason in currentLeagueSeasons)
            {
                DateTime? lastUpdate = leagueSeason.StandingsLastUpdate;

                if (ShouldUpdate(lastUpdate, _applicationConfig.StandingsUpdateForCurrentSeasonFrequency))
                {
                    try
                    {
                        StandingUpdater standingUpdater = (StandingUpdater)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(StandingUpdater));
                        await standingUpdater.StartAsync(UpdaterMode.SpecificLeagueCurrentSeason, leagueSeason.LeagueID);

                        // delay next call
                        await standingUpdater.DelayApiCallAsync();
                    }
                    catch (Exception ex) when (ex is not ApiException)
                    {
                        _logger.LogError($"Football data update worker for standings failed to execute for {leagueSeason.League.Name} / {leagueSeason.Year}: {ex.Message}");
                    }
                }
            }
        }

        protected async Task UpdateTopScorersForCurrentSeasonAsync(AppDbContext dbContext)
        {
            ICollection<LeagueSeason> currentLeagueSeasons = await dbContext.LeagueSeasons
                .Include(ls => ls.League)
                .Where(i => i.InProgress && i.League.UpdatesActive)
                .ToListAsync();

            foreach (var leagueSeason in currentLeagueSeasons)
            {
                DateTime? lastUpdate = leagueSeason.TopScorersLastUpdate;

                if (ShouldUpdate(lastUpdate, _applicationConfig.TopScorersUpdateForCurrentSeasonFrequency))
                {
                    try
                    {
                        TopScorerUpdater topScorerUpdater = (TopScorerUpdater)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(TopScorerUpdater));
                        await topScorerUpdater.StartAsync(UpdaterMode.SpecificLeagueCurrentSeason, leagueSeason.LeagueID);

                        // delay next call
                        await topScorerUpdater.DelayApiCallAsync();
                    }
                    catch (Exception ex) when (ex is not ApiException)
                    {
                        _logger.LogError($"Football data update worker for topscorers failed to execute for {leagueSeason.League.Name} / {leagueSeason.Year}: {ex.Message}");
                    }
                }
            }
        }
    }
}
