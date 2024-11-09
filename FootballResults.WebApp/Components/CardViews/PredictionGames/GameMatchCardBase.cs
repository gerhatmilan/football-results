using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.WebApp.Components.Pages.PredictionGames;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.CardViews.PredictionGames
{
    public partial class GameMatchCardBase : ComponentBase
    {
        [CascadingParameter(Name = "User")]
        public User? User { get; set; }

        [CascadingParameter(Name = "Game")]
        public PredictionGame? Game { get; set; }

        [CascadingParameter(Name = "Match")]
        public Match? Match { get; set; }

        protected Prediction? ExistingPrediction => Match?.Predictions != null
            ? Match.Predictions.FirstOrDefault(p => p.User.ID == User?.ID && p.PredictionGame.ID == Game?.ID)
            : null;
    }
}
