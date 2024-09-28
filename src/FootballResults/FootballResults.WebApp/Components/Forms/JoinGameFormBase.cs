using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.Predictions;
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

        [Parameter]
        public User? User { get; set; }

        protected JoinGameFormModel Model { get; set; } = new JoinGameFormModel();

        protected string? ErrorMessage { get; set; }

        protected override void ResetErrorMessages()
        {
            ErrorMessage = null;
            StateHasChanged();
        }

        protected async Task Submit()
        {
            ResetErrorMessages();
            DisableForm();

            PredictionGame? gameByKey = await PredictionGameService.GetPredictionGameByKeyAsync(Model.JoinKey!);

            if (gameByKey == null)
                ErrorMessage = "No game found with the provided key";
            else if (gameByKey.Players.Contains(User))
                ErrorMessage = "You have already joined this game";
            else
            {
                if (await PredictionGameService.JoinGameAsync(User!.ID, gameByKey.ID) != null)
                    NavigationManager!.NavigateTo($"/prediction-games/{gameByKey.ID}", true);
                else
                    ErrorMessage = "Failed to join the game at this time. Try again later";
            }

            StateHasChanged();
            await EnableForm();
        }
    }
}
