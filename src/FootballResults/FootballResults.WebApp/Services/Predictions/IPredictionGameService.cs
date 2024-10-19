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
        Task ReloadMatchesAsync(PredictionGame game);
        Task<PredictionGame?> GetPredictionGameByKeyAsync(string joinKey);
        Task<Participation?> JoinGameAsync(int userID, int predictionGameID);
        Task<Prediction?> MakePredictionAsync(int userID, int predictionGameID, int matchID, PredictionModel model);    
        Task<bool> UpdatePredictionAsync(int predictonID, PredictionModel model);
    }
}
