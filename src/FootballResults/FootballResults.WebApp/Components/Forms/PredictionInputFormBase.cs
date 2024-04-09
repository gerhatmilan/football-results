using FootballResults.Models.Football;
using FootballResults.Models.Predictions;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Forms
{
    public class PredictionInputFormBase : FormBase
    {
        protected PredictionModel PredictionModel { get; set; } = new PredictionModel();

        [Parameter]
        public Match? Match { get; set; }

        protected string HomeGoalsState { get; set; } = "default";
        protected string AwayGoalsState { get; set; } = "default";

        protected void EnableSuccessIndicator()
        {
            HomeGoalsState = "success";
            AwayGoalsState = "success";
        }

        protected void ResetSucccessIndicator()
        {
            HomeGoalsState = "default";
            AwayGoalsState = "default";
        }

        protected bool IsValidPrediction()
        {
            if (!PredictionModel.HomeTeamGoals.HasValue || PredictionModel.HomeTeamGoals < 0)
                HomeGoalsState = "invalid";
            if (!PredictionModel.AwayTeamGoals.HasValue || PredictionModel.AwayTeamGoals < 0)
                AwayGoalsState = "invalid";

            return PredictionModel.HomeTeamGoals >= 0 && PredictionModel.AwayTeamGoals >= 0;
        }

        protected override void DisableForm()
        {
            EnableSuccessIndicator();
            base.DisableForm();
        }

        protected override async Task EnableForm()
        {
            await base.EnableForm();
            ResetSucccessIndicator();
        }

        // TODO: on initialized load prediction

        protected async Task OnInputChange()
        {
            if (IsValidPrediction())
            {
                DisableForm();
                // TODO: save prediction
                await EnableForm();
            }
        }
    }
}
