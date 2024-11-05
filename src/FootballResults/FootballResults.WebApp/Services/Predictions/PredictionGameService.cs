using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Predictions;
using Microsoft.EntityFrameworkCore;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.Files;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using FootballResults.Models.Config;
using FootballResults.Models.ViewModels.PredictionGames;

namespace FootballResults.WebApp.Services.Predictions
{
    public class PredictionGameService : IPredictionGameService
    {
        private readonly AppDbContext _dbContext;
        private readonly IOptions<ApplicationConfig> _applicationSettings;
        private const string WWWROOT = "wwwroot";

        public PredictionGameService(AppDbContext dbContext, IOptions<ApplicationConfig> applicationSettings)
        {
            _dbContext = dbContext;
            _applicationSettings = applicationSettings;
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
            foreach (IncludedLeague includedLeague in model.IncludedLeagues)
            {
                if (includedLeague.Included)
                {
                    // get the the current season for the given league
                    LeagueSeason? leagueSeason = await _dbContext.LeagueSeasons.FirstOrDefaultAsync(i => i.LeagueID == includedLeague.League.ID && i.InProgress);

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

                    var defaultPicturePath = _applicationSettings.Value.PredictionGameDefaultImage;
                    // save the picture to the file system based on the generated ID, also update the entity in the database
                    if (model.PicturePath != defaultPicturePath)
                    {
                        string saveFilePath = _applicationSettings.Value.PredictionGamePicturesDirectory;
                        string saveFileName = $"{savedGame.ID}{Path.GetExtension(model.PicturePath)}";

                        // delete all old game pictures for this game, regardless of extension
                        await FileManager.DeleteFilesWithNameAsync(Path.Combine(WWWROOT, saveFilePath), savedGame.ID.ToString());

                        await FileManager.MoveFileAsync(Path.Combine(WWWROOT, model.PicturePath), Path.Combine(WWWROOT, saveFilePath, saveFileName));

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
                        .ThenInclude(m => m.HomeTeam)
                .Include(g => g.LeagueSeasons)
                    .ThenInclude(ls => ls.Matches)
                        .ThenInclude(m => m.AwayTeam)
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

        public async Task DeletePredictionGameAsync(PredictionGame game)
        {
            foreach (Message message in game.Messages)
            {
                message.User = null;
                _dbContext.Messages.Remove(message);
            }

            _dbContext.PredictionGames.Remove(game);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ReloadMatchesAsync(PredictionGame game)
        {
            foreach (Match match in game.Matches.Where(m => m.IsInProgress))
            {
                await _dbContext.Entry(match).ReloadAsync();
            }
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
