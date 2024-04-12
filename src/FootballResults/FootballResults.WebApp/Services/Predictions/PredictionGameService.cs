using FootballResults.WebApp.Database;
using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
using FootballResults.Models.General;
using Microsoft.EntityFrameworkCore;
using FootballResults.Models.Football;
using FootballResults.WebApp.Services.Football;
using System.Collections;

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
                IsFinished = false,
                Leagues = new List<League>()
            };
        }

        private async Task AddIncludedLeaguesAsync(PredictionGame game, CreateGameModel model)
        {
            foreach (var pair in model.IncludedLeagues)
            {
                if (pair.Second)
                {
                    _dbContext.IncludedLeagues.Add(new GameLeague { GameID = game.GameID, LeagueID = pair.First.LeagueID, Season = pair.First.CurrentSeason });
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
                    PredictionGame gameToBeSaved = CreateGameFromModel(model, user.UserID);

                    // save the new entry to the database, and obtain the entity entry with the generated ID
                    var entityEntry = await _dbContext.PredictionGames.AddAsync(gameToBeSaved);
                    PredictionGame savedGame = entityEntry.Entity;
                    await _dbContext.SaveChangesAsync();

                    var defaultPicturePath = _config.GetValue<string>("Images:PredictionGameDefault")!;
                    // save the picture to the file system based on the generated ID, also update the entity in the database
                    if (model.PicturePath != defaultPicturePath)
                    {
                        string saveFilePath = _config.GetValue<string>("Directories:PredictionGamePictures")!;
                        string saveFileName = $"{savedGame.GameID}{Path.GetExtension(model.PicturePath)}";

                        // delete all old game pictures for this game, regardless of extension
                        await FileManager.DeleteFilesWithNameAsync(saveFilePath, savedGame.GameID.ToString());

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
            .Where(g => g.GameID == gameID)
            .Include(g => g.Players)
            .Include(g => g.Leagues)
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
                GameID = game.GameID,
                UserID = user.UserID
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LeagueStanding>> GetStandingsAsync(PredictionGame game)
        {
            IEnumerable<LeagueStanding> allStandings = new List<LeagueStanding>();

            foreach (var league in game.Leagues)
            {
                int relevantSeason = game.GameLeagues
                    .Where(gl => gl.LeagueID == league.LeagueID)
                    .FirstOrDefault()!
                    .Season;

                var standings = await _dbContext.LeagueStandings
                    .Where(l => l.LeagueID == league.LeagueID && l.Season == relevantSeason)
                    .Include(l => l.Team)
                    .ToListAsync();

                allStandings = allStandings.Concat(standings);
            }

            return allStandings;
        }

        public async Task<IEnumerable<Match>> GetMatchesAsync(PredictionGame game)
        {
            IEnumerable<Match> allMatches = new List<Match>();

            foreach (var league in game.Leagues)
            {
                int relevantSeason = game.GameLeagues
                    .Where(gl => gl.LeagueID == league.LeagueID)
                    .FirstOrDefault()!
                    .Season;

                var matches = await _dbContext.Matches
                    .Where(m => m.LeagueID == league.LeagueID && m.Season == relevantSeason)
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam)
                    .ToListAsync();

                allMatches = allMatches.Concat(matches);
            }

            return allMatches;
        }

        public async Task<bool> MakePredictionAsync(User user, PredictionGame game, Match match, PredictionModel prediction)
        {
            if (user == null || game == null || match == null || !prediction.HomeTeamGoals.HasValue
                || !prediction.AwayTeamGoals.HasValue)
            {
                return false;
            }

            await _dbContext.Predictions.AddAsync(new Prediction
            {
                UserID = user.UserID,
                GameID = game.GameID,
                MatchID = match.MatchID,
                HomeTeamGoals = prediction.HomeTeamGoals.Value,
                AwayTeamGoals = prediction.AwayTeamGoals.Value
            });

            await _dbContext.SaveChangesAsync();
            return true;
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
