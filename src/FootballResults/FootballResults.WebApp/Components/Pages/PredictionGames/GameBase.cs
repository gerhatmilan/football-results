using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
using FootballResults.WebApp.Services.Predictions;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages.PredictionGames
{
    public class GameBase : ComponentBase
    {
        [Inject]
        protected IPredictionGameService? GameService { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [CascadingParameter(Name = "User")]
        protected User? User { get; set; }

        [Parameter]
        public string? GameID { get; set; }

        protected PredictionGame? Game { get; set; }

        protected bool UserAuthorized { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            // both parameters are set
            if (GameID != null && User != null)
            {
                await LoadGameAsync();
                if (Game == null)
                {
                    // Game not found
                    NavigationManager!.NavigateTo("/Error", true);
                }
                else
                {
                    AuthorizeUser();
                }
            }
        }

        protected async Task LoadGameAsync()
        {
            try
            {
                Game = await GameService!.GetPredictionGameAsync(int.Parse(GameID!));

                if (Game == null)
                {
                    NavigationManager!.NavigateTo("/Error", true);
                }
            }
            catch (Exception)
            {
                NavigationManager!.NavigateTo("/Error", true);
            }
        }

        protected void AuthorizeUser()
        {
            if (!Game!.Participants.Select(p => p.UserID).Contains(User!.UserID))
            {
                NavigationManager!.NavigateTo("/access-denied", true);
            }
            else
            {
                UserAuthorized = true;
            }
        }
    }
}
