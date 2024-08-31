using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.Predictions;

namespace FootballResults.WebApp.Services.Predictions
{
    public interface IPredictionGameService
    {
        Task<PredictionGame?> CreatePredictionGameAsync(User user, CreateGameModel game);
        Task<PredictionGame?> GetPredictionGameAsync(int gameID);
        Task<PredictionGame?> GetPredictionGameByKeyAsync(string joinKey);
        Task<bool> JoinGameAsync(User user, PredictionGame game);
        Task<IEnumerable<PredictionGameStanding>> GetPredictionGameStandingsAsync(PredictionGame game);
        Task<IEnumerable<Match>> GetMatchesAsync(PredictionGame game);
        Task<IEnumerable<LeagueStanding>> GetLeagueStandingsAsync(PredictionGame game, League league);
        Task<IEnumerable<Match>> GetMatchesAsync(PredictionGame game, League league);
        Task<Prediction?> MakePredictionAsync(User user, PredictionGame game, Match match, PredictionModel model);    
        Task<bool> UpdatePredictionAsync(Prediction prediction, PredictionModel model);
        Task RefreshData(PredictionGame game);
    }
}
