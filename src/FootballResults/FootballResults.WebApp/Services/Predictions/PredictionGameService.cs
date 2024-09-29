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

        private PredictionGame CreateGameFromModel(CreatePredictionGameModel model, int userID)
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

        private async Task AddIncludedLeaguesAsync(int predictionGameID, CreatePredictionGameModel model)
        {
            foreach (var pair in model.IncludedLeagues)
            {
                if (pair.Second)
                {
                    // get the the current season for the given league
                    LeagueSeason? leagueSeason = await _dbContext.LeagueSeasons.FirstOrDefaultAsync(i => i.LeagueID == pair.First.ID && i.InProgress);

                    if (leagueSeason != null)
                    {
                        _dbContext.PredictionGameSeasons.Add(new PredictionGameSeason { PredictionGameID = predictionGameID, LeagueSeasonID = leagueSeason.ID });
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<PredictionGame?> CreatePredictionGameAsync(int userID, CreatePredictionGameModel model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    PredictionGame gameToBeSaved = CreateGameFromModel(model, userID);

                    // save the new entry to the database, and obtain the entity entry with the generated ID
                    PredictionGame savedGame = _dbContext.PredictionGames.Add(gameToBeSaved).Entity;
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
                    await AddIncludedLeaguesAsync(savedGame.ID, model);

                    // register participation
                    await JoinGameAsync(userID, savedGame.ID);

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
            PredictionGame? game = await _dbContext.PredictionGames
                .Include(g => g.Players)
                .Include(g => g.LeagueSeasons)
                    .ThenInclude(ls => ls.League)
                .Include(g => g.LeagueSeasons)
                    .ThenInclude(ls => ls.Matches)
                .Include(g => g.LeagueSeasons)
                    .ThenInclude(ls => ls.Standings)
                        .ThenInclude(s => s.Team)
                .Include(g => g.Participations)
                    .ThenInclude(p => p.Predictions)
                        .ThenInclude(p => p.Match)
                .Include(g => g.Participations)
                    .ThenInclude(p => p.Standing)
                .Include(g => g.Messages)
                .AsSplitQuery()
                .FirstOrDefaultAsync(g => g.ID == gameID);

            return game;
        }

        public async Task<PredictionGame?> GetPredictionGameByKeyAsync(string joinKey)
        {
            return await _dbContext.PredictionGames
                .Include(g => g.Players)
                .FirstOrDefaultAsync(g => g.JoinKey.Equals(joinKey));
        }

        public async Task<Participation?> JoinGameAsync(int userID, int predictionGameID)
        {
            Participation participation = _dbContext.Participations.Add(new Participation
            {
                PredictionGameID = predictionGameID,
                UserID = userID,
                JoinDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            }).Entity;

            await _dbContext.SaveChangesAsync();
            PredictionGameStanding? relatedStanding = await CreateStandingAsync(predictionGameID, participation.ID);
            participation.StandingID = relatedStanding.ID;
            await _dbContext.SaveChangesAsync();

            return participation;
        }

        private async Task<PredictionGameStanding> CreateStandingAsync(int predictionGameID, int participationID)
        {
            PredictionGameStanding standing = _dbContext.PredictionGameStandings.Add(new PredictionGameStanding
            {
                ParticipationID = participationID,
                Points = 0
            }).Entity;

            await _dbContext.SaveChangesAsync();

            return standing;
        }

        public async Task<IEnumerable<PredictionGameStanding>?> GetPredictionGameStandingsAsync(int predictionGameID)
        {
            PredictionGame? gameInDatabase = await _dbContext.PredictionGames
                .Include(g => g.Participations)
                    .ThenInclude(p => p.Standing)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(g => g.ID == predictionGameID);

            return gameInDatabase != null ? gameInDatabase.Standings.OrderByDescending(standing => standing.Points) : null;
        }
        public async Task<IEnumerable<Match>?> GetMatchesAsync(int predictionGameID)
        {
            PredictionGame? gameInDatabase = await _dbContext.PredictionGames
                .Include(g => g.LeagueSeasons)
                    .ThenInclude(ls => ls.Matches)
                .FirstOrDefaultAsync(i => i.ID == predictionGameID);

            return gameInDatabase != null ? gameInDatabase.Matches : null;
        }
        public async Task<IEnumerable<LeagueStanding>?> GetLeagueStandingsAsync(int predictionGameID, int leagueSeasonID)
        {
            PredictionGame? gameInDatabase = await _dbContext.PredictionGames
                .Include(g => g.LeagueSeasons).ThenInclude(ls => ls.League)
                .Include(g => g.LeagueSeasons).ThenInclude(ls => ls.Standings)
                .FirstOrDefaultAsync(i => i.ID == predictionGameID);

            return gameInDatabase != null ? gameInDatabase.LeagueSeasons.FirstOrDefault(ls => ls.ID == leagueSeasonID)?.Standings : null;
        }

        public async Task<IEnumerable<Match>?> GetMatchesAsync(int predictionGameID, int leagueSeasonID)
        {
            PredictionGame? gameInDatabase = await _dbContext.PredictionGames
                .Include(g => g.LeagueSeasons).ThenInclude(ls => ls.League)
                .Include(g => g.LeagueSeasons).ThenInclude(ls => ls.Matches)
                .FirstOrDefaultAsync(i => i.ID == predictionGameID);

            return gameInDatabase != null ? gameInDatabase.Matches.Where(m => m.LeagueSeason.ID == leagueSeasonID) : null;
        }

        public async Task<Prediction?> MakePredictionAsync(int userID, int predictionGameID, int matchID, PredictionModel predictionModel)
        {
            Participation? relevantParticipation = await _dbContext.Participations.FirstOrDefaultAsync(p => p.UserID == userID && p.PredictionGameID == predictionGameID);

            if (relevantParticipation != null && predictionModel.Valid)
            {
                Prediction prediction = new Prediction
                {
                    ParticipationID = relevantParticipation.ID,
                    MatchID = matchID,
                    HomeTeamGoals = predictionModel.HomeTeamGoals!.Value,
                    AwayTeamGoals = predictionModel.AwayTeamGoals!.Value,
                    Date = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                await _dbContext.Predictions.AddAsync(prediction);
                await _dbContext.SaveChangesAsync();
                return prediction;
            }
            else
            {
                return null;
            }
 
        }

        public async Task<bool> UpdatePredictionAsync(int predictionID, PredictionModel model)
        {
            if (model.Valid)
            {
                Prediction? predictionInDatabase = await _dbContext.Predictions.FirstOrDefaultAsync(p => p.ID == predictionID);

                if (predictionInDatabase != null)
                {
                    predictionInDatabase.HomeTeamGoals = model.HomeTeamGoals!.Value;
                    predictionInDatabase.AwayTeamGoals = model.AwayTeamGoals!.Value;

                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
