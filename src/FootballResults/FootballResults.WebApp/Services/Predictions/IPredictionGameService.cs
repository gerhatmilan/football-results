using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.Predictions;

namespace FootballResults.WebApp.Services.Predictions
{
    public interface IPredictionGameService
    {
        Task<PredictionGame?> CreatePredictionGameAsync(int userID, CreatePredictionGameModel game);
        Task<PredictionGame?> GetPredictionGameAsync(int gameID);
        Task<PredictionGame?> GetPredictionGameByKeyAsync(string joinKey);
        Task<Participation?> JoinGameAsync(int userID, int predictionGameID);
        Task<IEnumerable<PredictionGameStanding>?> GetPredictionGameStandingsAsync(int predictionGameID);
        Task<IEnumerable<Match>?> GetMatchesAsync(int predictionGameID);
        Task<IEnumerable<LeagueStanding>?> GetLeagueStandingsAsync(int predictionGameID, int leagueSeasonID);
        Task<IEnumerable<Match>?> GetMatchesAsync(int predictionGameID, int leagueSeasonID);
        Task<Prediction?> MakePredictionAsync(int userID, int predictionGameID, int matchID, PredictionModel model);    
        Task<bool> UpdatePredictionAsync(int predictonID, PredictionModel model);
    }
}
