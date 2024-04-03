using FootballResults.WebApp.Database;
using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
using FootballResults.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp.Services.Predictions
{
    public class PredictionGameService : IPredictionGameService
    {
        private AppDbContext _dbContext;
        private IConfiguration _config;
        public PredictionGameService(AppDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        private PredictionGame CreateGameFromModel(CreateGameModel model, int userID)
        {
            return new PredictionGame
            {
                Name = model.Name,
                OwnerID = userID,
                JoinKey = Guid.NewGuid().ToString(),
                Description = model.Description,
                ImagePath = _config.GetValue<string>("Images:PredictionGameDefault"),
                ExactScorelineReward = model.ExactScorelineReward,
                OutcomeReward = model.OutcomeReward,
                GoalCountReward = model.GoalCountReward,
                GoalDifferenceReward = model.GoalDifferenceReward,
                IsFinished = false,
            };
        }
        
        private async Task AddIncludedLeaguesAsync(int predictionGameID, CreateGameModel model)
        {
            foreach (var pair in model.IncludedLeagues)
            {
                if (pair.Second)
                {
                    await _dbContext.IncludedLeagues.AddAsync(new IncludedLeague
                    {
                        GameID = predictionGameID,
                        LeagueID = pair.First.LeagueID
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<PredictionGame?> CreatePredictionGameAsync(User user, CreateGameModel model)
        {

            using(var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    PredictionGame gameToBeSaved = CreateGameFromModel(model, user.UserID);

                    // save the new entry to the database, and obtain the entity entry with the generated ID
                    var entityEntry = await _dbContext.PredictionGames.AddAsync(gameToBeSaved);
                    PredictionGame savedGame = entityEntry.Entity;
                    await _dbContext.SaveChangesAsync();

                    // save the picture to the file system based on the generated ID, also update the entity in the database
                    if (model.Picture != null)
                    {
                        string path = _config.GetValue<string>("Directories:PredictionGamePictures")!;
                        await ImageManager.SaveImageAsync(model.Picture, path, $"{savedGame.GameID}.jpg");

                        savedGame.ImagePath = Path.Combine(path, $"{savedGame.GameID}.jpg");
                    }

                    // add the selected leagues to the included leagues table
                    await AddIncludedLeaguesAsync(savedGame.GameID, model);

                    transaction.Commit();

                    return savedGame;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        public async Task<PredictionGame?> GetPredictionGameAsync(int gameID)
        {
            return await _dbContext.PredictionGames
                .Where(g => g.GameID == gameID)
                .Include(g => g.Players)
                .Include(g => g.IncludedLeagues)
                .Include(g => g.Predictions)
                .Include(g => g.Standings)
                .FirstOrDefaultAsync();
        }

        public async Task<PredictionGame?> GetPredictionGameByKeyAsync(string joinKey)
        {
            return await _dbContext.PredictionGames
                .Where(g => g.JoinKey.Equals(joinKey))
                .Include(g => g.Players)
                .Include(g => g.IncludedLeagues)
                .Include(g => g.Predictions)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> JoinGameAsync(User user, PredictionGame game)
        {
            if (user == null || game == null)
            {
                return false;
            }

            await _dbContext.Participations.AddAsync(new Participation
            {
                GameID = game.GameID,
                UserID = user.UserID
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
