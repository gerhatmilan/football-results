using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Models;
using FootballResults.DatabaseUpdaters.Updaters;
using FootballResults.Models.Config;
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

        protected MatchUpdater _matchUpdater;
        protected StandingUpdater _standingUpdater;
        protected TopScorerUpdater _topScorerUpdater;

        protected static IEnumerable<string> MatchStatusesThatRequireUpdate { get; } = new List<string> { MatchStatus.NotStarted, MatchStatus.FirstHalf, MatchStatus.HalfTime, MatchStatus.SecondHalf, MatchStatus.ExtraTime, MatchStatus.BreakTime, MatchStatus.PenaltiesInProgress, MatchStatus.Interrupted, MatchStatus.Live };

        public FootballDataUpdater(IServiceProvider serviceProvider, ILogger<FootballDataUpdater> logger)
        {
            _serviceProvider = serviceProvider;
            _applicationConfig = serviceProvider.GetRequiredService<IOptions<ApplicationConfig>>().Value;
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
                                await UpdateMatchesForCurrentDayAsync(dbContext);
                                await UpdateMatchesForCurrentSeasonAsync(dbContext);
                                await UpdateStandingsForCurrentSeasonAsync(dbContext);
                                await UpdateTopScorersForCurrentSeasonAsync(dbContext);
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

        protected async Task<bool> MatchMightBeInProgressAsync(AppDbContext dbContext)
        {
            ICollection<Match> matchesThatMightBeInProgress = await dbContext.Matches
                .Where(i =>
                    i.Date != null
                    && i.Date < DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                    && MatchStatusesThatRequireUpdate.Contains(i.Status)
                ).ToListAsync();

            return matchesThatMightBeInProgress.Any();
        }

        protected async Task UpdateMatchesForCurrentDayAsync(AppDbContext dbContext)
        {
            DateTime? lastUpdate = dbContext.SystemInformation.Find(1)?.MatchesLastUpdateForCurrentDay;

            if (ShouldUpdate(lastUpdate, _applicationConfig.MatchesUpdateForCurrentDayFrequency)
                && await MatchMightBeInProgressAsync(dbContext))
            {
                try
                {
                    await _matchUpdater.StartAsync(DatabaseUpdaters.UpdaterMode.CurrentDate);
                }
                catch (Exception)
                {
                    _logger.LogError($"Football data update worker for matches failed");
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
                    }
                    catch (Exception)
                    {
                        _logger.LogError($"Football data update worker for matches failed to execute for {leagueSeason.League.Name} / {leagueSeason.Year}");
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
                    catch (Exception)
                    {
                        _logger.LogError($"Football data update worker for standings failed to execute for {leagueSeason.League.Name} / {leagueSeason.Year}");
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
                    catch (Exception)
                    {
                        _logger.LogError($"Football data update worker for topscorers failed to execute for {leagueSeason.League.Name} / {leagueSeason.Year}");
                    }
                }
            }
        }
    }
}
