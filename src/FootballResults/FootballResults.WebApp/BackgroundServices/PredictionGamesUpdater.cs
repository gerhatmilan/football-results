using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Models;
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
                                _logger.LogError($"An error occurred while updating prediction games data: {ex.Message}");
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
            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    IQueryable<PredictionGame> games = dbContext.PredictionGames
                        .Include(g => g.Participations)
                            .ThenInclude(p => p.Predictions)
                                .ThenInclude(pred => pred.Match)
                        .Include(g => g.Participations)
                            .ThenInclude(p => p.Standing)
                        .Include(g => g.LeagueSeasons)
                            .ThenInclude(ls => ls.Matches)
                        .AsSplitQuery();

                    foreach (var game in games)
                    {
                        try
                        {
                            game.RefreshData();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                            _logger.LogError($"An error has occured while updating prediction games data (ID {game.ID}). Changes will be unmade for this game.");
                            
                            // revert changes
                            game.Predictions.ToList().ForEach(p => dbContext.Entry(p).State = EntityState.Unchanged);
                            game.Standings.ToList().ForEach(s => dbContext.Entry(s).State = EntityState.Unchanged);
                            dbContext.Entry(game).State = EntityState.Unchanged;
                        }
                    }

                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Prediction games update failed: {ex.Message}");
                    transaction.Rollback();
                }
            }
        }
    }
}
