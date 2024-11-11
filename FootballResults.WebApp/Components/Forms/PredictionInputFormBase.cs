using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.ViewModels.PredictionGames;
using FootballResults.WebApp.Services.Predictions;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Forms
{
    public class PredictionInputFormBase : FormBase
    {
        [Inject]
        protected IPredictionGameService PredictionGameService { get; set; } = default!;

        [Inject]
        protected IUserService UserService { get; set; } = default!;

        [CascadingParameter(Name = "User")]
        public User User { get; set; } = default!;

        [CascadingParameter(Name = "Game")]
        public PredictionGame Game { get; set; } = default!;

        [CascadingParameter(Name = "Match")]
        public Match Match { get; set; } = default!;

        [CascadingParameter(Name = "Prediction")]
        protected Prediction? ExistingPrediction { get; set; }

        protected PredictionModel PredictionModel { get; set; } = new PredictionModel();


        protected override void OnParametersSet()
        {
            PredictionModel.HomeTeamGoals = ExistingPrediction?.HomeTeamGoals;
            PredictionModel.AwayTeamGoals = ExistingPrediction?.AwayTeamGoals;
        }

        protected bool CheckPrediction()
        {
            if (!PredictionModel.BothFilled)
                return false;

            if (PredictionModel.HomeTeamGoals < 0)
                PredictionModel.HomeGoalsState = "invalid";
            if (PredictionModel.AwayTeamGoals < 0)
                PredictionModel.AwayGoalsState = "invalid";
            if (Match.HasStarted)
            {
                PredictionModel.HomeGoalsState = PredictionModel.AwayGoalsState = "invalid";
                PredictionModel.MatchStartedError = true;
            }

            return PredictionModel.Valid;
        }

        protected override async Task EnableForm()
        {
            await base.EnableForm();
            PredictionModel.ResetIndicators();
        }

        protected async Task OnInputChange()
        {
            // if one of the inputs dont have a value, dont do anything
            if (!PredictionModel.BothFilled)
                return;
            else
            {
                // disable the form and check for valid input
                DisableForm();
                if (CheckPrediction())
                {
                    // valid inputs, save the prediction
                    PredictionModel.EnableSuccessIndicator();
                    if (ExistingPrediction == null)
                        ExistingPrediction = await PredictionGameService.MakePredictionAsync(User.ID, Game.ID, Match.ID, PredictionModel);
                    else
                        await PredictionGameService.UpdatePredictionAsync(ExistingPrediction.ID, PredictionModel);

                    await EnableForm();
                }

                // invalid input(s), reset the form only if the match has not started yet
                else if (!Match.HasStarted)
                {
                    await EnableForm();
                }
            }
        }
    }
}
