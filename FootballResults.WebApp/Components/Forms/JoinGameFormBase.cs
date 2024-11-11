using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.ViewModels.PredictionGames;
using FootballResults.WebApp.Services.Predictions;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Forms
{
    public class JoinGameFormBase : FormBase
    {
        [Inject]
        protected IPredictionGameService PredictionGameService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [CascadingParameter(Name = "User")]
        public User? User { get; set; }

        protected JoinPredictionGameFormModel Model { get; set; } = new JoinPredictionGameFormModel();

        protected override void ResetErrorMessages()
        {
            Model.ResetMessages();
        }

        protected async Task Submit()
        {
            ResetErrorMessages();
            DisableForm();

            PredictionGame? gameByKey = await PredictionGameService.GetPredictionGameByKeyAsync(Model.JoinKey!);

            if (gameByKey == null)
            {
                Model.GameNotFoundError = true;
            }
            else if (gameByKey.Players.Any(i => i.ID == User!.ID))
            {
                NavigationManager!.NavigateTo($"/prediction-games/{gameByKey.ID}", true);
            }
            else
            {
                try
                {
                    if (await PredictionGameService.JoinGameAsync(User!.ID, gameByKey.ID) != null)
                    {
                        NavigationManager!.NavigateTo($"/prediction-games/{gameByKey.ID}", true);
                    }
                    else
                    {
                        Model.Error = true;
                    }
                }
                catch (Exception)
                {
                    Model.Error = true;
                }
            }

            await EnableForm();
        }
    }
}
