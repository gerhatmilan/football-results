using FootballResults.Models.Users;
using FootballResults.Models.Predictions;

namespace FootballResults.WebApp.Services.Predictions
{
    public interface IPredictionGameService
    {
        Task<bool> CreatePredictionGameAsync(User user, CreateGameModel game);
        Task GetPredictionGameAsync(string joinKey);
    }
}
