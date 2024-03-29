using FootballResults.WebApp.Models;
using FootballResults.WebApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections;

namespace FootballResults.WebApp.Components.Pages.PredictionGames
{
    public partial class PredictionGamesBase : ComponentBase
    {
        [Inject]
        protected IUserService? UserService { get; set; }

        [Inject]
        protected AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        protected User? User { get; set; }

        protected IEnumerable<PredictionGame> PredictionGames => User?.PredictionGames ?? new List<PredictionGame>();

        protected string? ActiveSubMenu { get; set; } = "games";

        protected async override Task OnInitializedAsync()
        {
            // get user from database based on user id claim
            var authenticationState = await AuthenticationStateProvider!.GetAuthenticationStateAsync();
            var user = authenticationState.User;
            if (user.Identity?.IsAuthenticated ?? false)
            {
                var userID = user.FindFirst("UserID")!.Value;

                int userIDConverted;
                if (int.TryParse(userID, out userIDConverted))
                {
                    User = await UserService!.GetUserIncludingPredictionGamesAsync(userIDConverted);
                    if (User == null)
                    {
                        NavigationManager!.NavigateTo("/logout", true);
                    }
                }
            }
        }
    }
}
