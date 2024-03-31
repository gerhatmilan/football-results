using FootballResults.Models.Users;
using FootballResults.Models.Predictions;

namespace FootballResults.WebApp.Services.Predictions
{
    public interface IPredictionGameService
    {
        Task<PredictionGame?> CreatePredictionGameAsync(User user, CreateGameModel game);
        Task<PredictionGame?> GetPredictionGameAsync(int gameID);
    }
}
