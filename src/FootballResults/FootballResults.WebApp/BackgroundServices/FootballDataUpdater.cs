using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Models;
using FootballResults.DatabaseUpdaters.Updaters;
using FootballResults.Models.Config;
using FootballResults.WebApp.Hubs;
using FootballResults.WebApp.Services.Chat;
using FootballResults.WebApp.Services.LiveUpdates;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FootballResults.WebApp.BackgroundServices
{
    public class FootballDataUpdater : BackgroundService
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger<FootballDataUpdater> _logger;
        protected readonly ApplicationConfig _applicationConfig;
        protected readonly TimeSpan _period;
        protected readonly IHubContext<MessageHub<UpdateMessageType>> _notificationHubContext;

        protected MatchUpdater _matchUpdater;
        protected StandingUpdater _standingUpdater;
        protected TopScorerUpdater _topScorerUpdater;

        protected static IEnumerable<string> MatchStatusesThatRequireUpdate { get; } = new List<string> { MatchStatus.NotStarted, MatchStatus.FirstHalf, MatchStatus.HalfTime, MatchStatus.SecondHalf, MatchStatus.ExtraTime, MatchStatus.BreakTime, MatchStatus.PenaltiesInProgress, MatchStatus.Suspended, MatchStatus.Interrupted, MatchStatus.Live };

        public FootballDataUpdater(IServiceProvider serviceProvider, ILogger<FootballDataUpdater> logger, IOptions<ApplicationConfig> applicationConfig,
            IHubContext<MessageHub<UpdateMessageType>> notificationHubContext)
        {
            _serviceProvider = serviceProvider;
            _applicationConfig = applicationConfig.Value;
            _notificationHubContext = notificationHubContext;
            _logger = logger;
            _period = _applicationConfig.UpdaterWorkerFrequency;

            _matchUpdater = (MatchUpdater)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(MatchUpdater));
            _standingUpdater = (StandingUpdater)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(StandingUpdater));
            _topScorerUpdater = (TopScorerUpdater)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(TopScorerUpdater));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (PeriodicTimer timer = new PeriodicTimer(_period))
            {
                do
                {
                    _logger.LogInformation($"Football data update worker started at {DateTime.Now}");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                        {
                            try
                            {
                                await UpdateMatchesForCurrentSeasonAsync(dbContext);
                                await UpdateStandingsForCurrentSeasonAsync(dbContext);
                                await UpdateTopScorersForCurrentSeasonAsync(dbContext);
                                await UpdateMatchesForCurrentDayAsync(dbContext);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred while updating data");
                            }
                        }
                    }

                    _logger.LogInformation($"Football data update worker finished at {DateTime.Now}");
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

            if (ShouldUpdate(lastUpdate, _applicationConfig.MatchesUpdateForCurrentDayFrequency)
                && MatchMightBeInProgress(dbContext))
            {
                try
                {
                    await _matchUpdater.StartAsync(DatabaseUpdaters.UpdaterMode.CurrentDate);

                    // notify connected clients about the update
                    await _notificationHubContext.Clients.All.SendAsync("ReceiveMessage", UpdateMessageType.MatchesUpdated);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Football data update worker for matches failed: {ex.Message}");
                }
            }
        }

        protected async Task UpdateMatchesForCurrentSeasonAsync(AppDbContext dbContext)
        {
            ICollection<LeagueSeason> currentLeagueSeasons = await dbContext.LeagueSeasons
                .Include(ls => ls.League)
                .Where(i => i.InProgress)
                .ToListAsync();

            foreach (var leagueSeason in currentLeagueSeasons)
            {
                DateTime? lastUpdate = leagueSeason.MatchesLastUpdate;

                if (ShouldUpdate(lastUpdate, _applicationConfig.MatchesUpdateForCurrentSeasonFrequency))
                {
                    try
                    {
                        await _matchUpdater.StartAsync(DatabaseUpdaters.UpdaterMode.SpecificLeagueCurrentSeason, leagueSeason.LeagueID);

                        // notify connected clients about the update
                        await _notificationHubContext.Clients.All.SendAsync("ReceiveMessage", UpdateMessageType.MatchesUpdated);
                    }
                    catch (Exception ex)
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
                .Where(i => i.InProgress)
                .ToListAsync();

            foreach (var leagueSeason in currentLeagueSeasons)
            {
                DateTime? lastUpdate = leagueSeason.StandingsLastUpdate;

                if (ShouldUpdate(lastUpdate, _applicationConfig.StandingsUpdateForCurrentSeasonFrequency))
                {
                    try
                    {
                        await _standingUpdater.StartAsync(DatabaseUpdaters.UpdaterMode.SpecificLeagueCurrentSeason, leagueSeason.LeagueID);
                    }
                    catch (Exception ex)
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
                .Where(i => i.InProgress)
                .ToListAsync();

            foreach (var leagueSeason in currentLeagueSeasons)
            {
                DateTime? lastUpdate = leagueSeason.TopScorersLastUpdate;

                if (ShouldUpdate(lastUpdate, _applicationConfig.TopScorersUpdateForCurrentSeasonFrequency))
                {
                    try
                    {
                        await _topScorerUpdater.StartAsync(DatabaseUpdaters.UpdaterMode.SpecificLeagueCurrentSeason, leagueSeason.LeagueID);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Football data update worker for topscorers failed to execute for {leagueSeason.League.Name} / {leagueSeason.Year}: {ex.Message}");
                    }
                }
            }
        }
    }
}
