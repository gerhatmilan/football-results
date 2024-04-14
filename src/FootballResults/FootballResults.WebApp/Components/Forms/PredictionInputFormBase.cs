﻿using FootballResults.Models.Football;
using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
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
        protected string HomeGoalsState { get; set; } = "default";
        protected string AwayGoalsState { get; set; } = "default";
        protected bool MatchStartedError { get; set; } = false;

        protected override void OnParametersSet()
        {
            if (ExistingPrediction != null)
            {
                PredictionModel.HomeTeamGoals = ExistingPrediction.HomeTeamGoals;
                PredictionModel.AwayTeamGoals = ExistingPrediction.AwayTeamGoals;
            }
        }

        protected void EnableSuccessIndicator()
        {
            HomeGoalsState = "success";
            AwayGoalsState = "success";
        }

        protected void ResetIndicators()
        {
            HomeGoalsState = "default";
            AwayGoalsState = "default";
        }

        protected bool IsValidPrediction()
        {
            if (!PredictionModel.HomeTeamGoals.HasValue || !PredictionModel.AwayTeamGoals.HasValue)
                return false;

            if (PredictionModel.HomeTeamGoals < 0)
                HomeGoalsState = "invalid";
            if (PredictionModel.AwayTeamGoals < 0)
                AwayGoalsState = "invalid";
            if (Match.HasStarted())
            {
                HomeGoalsState = AwayGoalsState = "invalid";
                MatchStartedError = true;
            }

            return PredictionModel.HomeTeamGoals >= 0 && PredictionModel.AwayTeamGoals >= 0 && !Match.HasStarted();
        }

        protected override void DisableForm()
        {
            base.DisableForm();
        }

        protected override async Task EnableForm()
        {
            await base.EnableForm();
            ResetIndicators();
        }

        protected async Task OnInputChange()
        {
            // if one of the inputs dont have a value, dont do anything
            if (!PredictionModel.HomeTeamGoals.HasValue || !PredictionModel.AwayTeamGoals.HasValue)
                return;
            else
            {
                // disable the form and check for valid input
                DisableForm();
                if (IsValidPrediction())
                {
                    // valid inputs, save the prediction
                    EnableSuccessIndicator();
                    if (ExistingPrediction == null)
                        await PredictionGameService.MakePredictionAsync(User, Game, Match, PredictionModel);
                    else
                        await PredictionGameService.UpdatePredictionAsync(ExistingPrediction, PredictionModel);

                    await EnableForm();
                }

                // invalid input(s), reset the form only if the match has not started yet
                else if (!Match.HasStarted())
                {
                    await EnableForm();
                }
            }
        }
    }
}
