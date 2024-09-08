using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.Models.General;
using Microsoft.EntityFrameworkCore;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.Models.Predictions;
using FootballResults.DataAccess.Entities.Users;

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
                ImagePath = model.PicturePath,
                ExactScorelineReward = model.ExactScorelineReward,
                OutcomeReward = model.OutcomeReward,
                GoalCountReward = model.GoalCountReward,
                GoalDifferenceReward = model.GoalDifferenceReward,
                Finished = false,
                LeagueSeasons = new List<LeagueSeason>(),
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };
        }

        private async Task AddIncludedLeaguesAsync(PredictionGame game, CreateGameModel model)
        {
            foreach (var pair in model.IncludedLeagues)
            {
                if (pair.Second)
                {
                    LeagueSeason? leagueSeason = await _dbContext.LeagueSeasons.FindAsync(pair.First.ID);

                    _dbContext.PredictionGameSeasons.Add(new PredictionGameSeason { PredictionGameID = game.ID, LeagueSeasonID = leagueSeason!.ID });
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<PredictionGame?> CreatePredictionGameAsync(User user, CreateGameModel model)
        {

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    PredictionGame gameToBeSaved = CreateGameFromModel(model, user.ID);

                    // save the new entry to the database, and obtain the entity entry with the generated ID
                    var entityEntry = await _dbContext.PredictionGames.AddAsync(gameToBeSaved);
                    PredictionGame savedGame = entityEntry.Entity;
                    await _dbContext.SaveChangesAsync();

                    var defaultPicturePath = _config.GetValue<string>("Images:PredictionGameDefault")!;
                    // save the picture to the file system based on the generated ID, also update the entity in the database
                    if (model.PicturePath != defaultPicturePath)
                    {
                        string saveFilePath = _config.GetValue<string>("Directories:PredictionGamePictures")!;
                        string saveFileName = $"{savedGame.ID}{Path.GetExtension(model.PicturePath)}";

                        // delete all old game pictures for this game, regardless of extension
                        await FileManager.DeleteFilesWithNameAsync(saveFilePath, savedGame.ID.ToString());

                        await FileManager.MoveFileAsync(model.PicturePath, Path.Combine(saveFilePath, saveFileName));

                        // rename the uploaded picture file to the game's ID
                        savedGame.ImagePath = saveFilePath + $"/{saveFileName}";
                    }

                    // add the selected leagues to the game
                    await AddIncludedLeaguesAsync(savedGame, model);

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
            .Where(g => g.ID == gameID)
            .Include(g => g.Players)
            .Include(g => g.LeagueSeasons)
            .Include(g => g.Predictions).ThenInclude(p => p.Match)
            .Include(g => g.Standings)
            .Include(g => g.Messages)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
        }

        public async Task<PredictionGame?> GetPredictionGameByKeyAsync(string joinKey)
        {
            return await _dbContext.PredictionGames
                .Where(g => g.JoinKey.Equals(joinKey))
                .Include(g => g.Players)
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
                PredictionGameID = game.ID,
                UserID = user.ID,
                JoinDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PredictionGameStanding>> GetPredictionGameStandingsAsync(PredictionGame game)
        {

            return await _dbContext.PredictionGameStandings
                .Where(s => s.Participation.PredictionGameID == game.ID)
                .Include(s => s.User)
                .OrderByDescending(s => s.Points)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetMatchesAsync(PredictionGame game)
        {
            PredictionGame? gameInDatabase = await _dbContext.PredictionGames.FindAsync(game.ID);
            return gameInDatabase!.Matches;
        }

        public async Task<IEnumerable<LeagueStanding>> GetLeagueStandingsAsync(PredictionGame game, League league)
        {
            PredictionGame? gameInDatabase = await _dbContext.PredictionGames.FindAsync(game.ID);
            return gameInDatabase!.LeagueSeasons.SelectMany(ls => ls.Standings);
        }

        public async Task<IEnumerable<Match>> GetMatchesAsync(PredictionGame game, League league)
        {
            PredictionGame? gameInDatabase = await _dbContext.PredictionGames.FindAsync(game.ID);
            return gameInDatabase!.Matches.Where(m => m.LeagueSeason.League.ID == league.ID);
        }

        public async Task<Prediction?> MakePredictionAsync(User user, PredictionGame game, Match match, PredictionModel predictionModel)
        {
            if (user == null || game == null || match == null || !predictionModel.HomeTeamGoals.HasValue
                || !predictionModel.AwayTeamGoals.HasValue)
            {
                return null;
            }

            Participation? relevantParticipation = await _dbContext.Participations.FirstOrDefaultAsync(p => p.UserID == user.ID && p.PredictionGameID == game.ID);

            Prediction prediction = new Prediction
            {
                ParticipationID = relevantParticipation!.ID,
                MatchID = match.ID,
                HomeTeamGoals = predictionModel.HomeTeamGoals.Value,
                AwayTeamGoals = predictionModel.AwayTeamGoals.Value,
                Date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            await _dbContext.Predictions.AddAsync(prediction);
            await _dbContext.SaveChangesAsync();
            return prediction;
        }

        public async Task<bool> UpdatePredictionAsync(Prediction prediction, PredictionModel model)
        {
            if (prediction == null || model == null || !model.HomeTeamGoals.HasValue
                || !model.AwayTeamGoals.HasValue)
                return false;

            prediction.HomeTeamGoals = model.HomeTeamGoals.Value;
            prediction.AwayTeamGoals = model.AwayTeamGoals.Value;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        
        public async Task RefreshData(PredictionGame game)
        {
            game.RefreshData();
            await _dbContext.SaveChangesAsync();
        }
    }
}
