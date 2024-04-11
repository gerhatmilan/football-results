using FootballResults.Models.Football;
using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
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

        protected Prediction? ExistingPrediction { get; set; }

        protected override void OnParametersSet()
        {
            if (User != null && Game != null && Match != null)
            {
                ExistingPrediction = User.Predictions.FirstOrDefault(p => (p.UserID == User.UserID && p.GameID == Game.GameID && p.MatchID == Match.MatchID));
            }
        }
    }
}
