using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Models;
using FootballResults.DatabaseUpdaters.Updaters;
using FootballResults.Models.Config;
using FootballResults.WebApp.Components.Pages.PredictionGames;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FootballResults.WebApp.BackgroundServices
{
    public class PredictionGamesUpdater : BackgroundService
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger<PredictionGamesUpdater> _logger;
        protected readonly ApplicationConfig _applicationConfig;
        protected readonly TimeSpan _period;

        public PredictionGamesUpdater(IServiceProvider serviceProvider, ILogger<PredictionGamesUpdater> logger)
        {
            _serviceProvider = serviceProvider;
            _applicationConfig = serviceProvider.GetRequiredService<IOptions<ApplicationConfig>>().Value;
            _logger = logger;
            _period = _applicationConfig.UpdaterWorkerFrequency;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (PeriodicTimer timer = new PeriodicTimer(_period))
            {
                do
                {
                    _logger.LogInformation($"Prediction games update worker started at {DateTime.Now}");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                        {
                            try
                            {
                                await UpdatePredictionGamesAsync(dbContext);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "An error occurred while updating prediction games data");
                            }
                        }
                    }

                    _logger.LogInformation($"Prediction games update worker finished at {DateTime.Now}");
                }
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
            }
        }

        protected async Task UpdatePredictionGamesAsync(AppDbContext dbContext)
        {
            ICollection<PredictionGame> games = await dbContext.PredictionGames
                .Include(g => g.Participations)
                    .ThenInclude(p => p.Predictions)
                        .ThenInclude(pred => pred.Match)
                .Include(g => g.Participations)
                    .ThenInclude(p => p.Standing)
                .Include(g => g.LeagueSeasons)
                    .ThenInclude(ls => ls.Matches)
                .AsSplitQuery()
                .ToListAsync();

            foreach (var game in games)
            {
                try
                {
                    game.RefreshData();
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    _logger.LogError($"An error has occured while updating prediction games data (ID {game.ID}). Changes will be unmade for this game.");
                    dbContext.ChangeTracker.Clear();
                }
            }
        }
    }
}
