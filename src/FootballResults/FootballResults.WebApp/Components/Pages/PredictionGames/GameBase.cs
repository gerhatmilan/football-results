using FootballResults.Models.Football;
using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Predictions;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages.PredictionGames
{
    public class GameBase : ComponentBase
    {
        [Inject]
        protected IPredictionGameService GameService { get; set; } = default!;

        [Inject]
        protected IUserService UserService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [CascadingParameter(Name = "User")]
        protected User? User { get; set; }

        [Parameter]
        public string? GameID { get; set; }

        protected PredictionGame? Game { get; set; }

        protected IEnumerable<LeagueStanding>? LeagueStandings { get; set; }

        protected IEnumerable<Match>? Matches { get; set; }

        protected bool UserAuthorized { get; set; }

        protected bool ShowJoinKey { get; set; } = false;

        protected override async Task OnParametersSetAsync()
        {
            // both parameters are set
            if (GameID != null && User != null)
            {
                // Game not loaded yet
                if (Game == null)
                {
                    await LoadGameAsync();

                    // Game loaded but not found
                    if (Game == null)
                    {
                        NavigationManager!.NavigateTo("/Error", true);
                    }
                    else if (AuthorizeUser())
                    {
                        await GameService.RefreshData(Game);
                        await UserService.GetGameDataForUserAsync(User);
                    }
                    else
                    {
                        NavigationManager!.NavigateTo("/access-denied", true);
                    }
                }
            }
        }

        protected async Task LoadGameAsync()
        {
            try
            {
                Game = await GameService!.GetPredictionGameAsync(int.Parse(GameID!));

                if (Game != null)
                {
                    LeagueStandings = await GameService.GetStandingsAsync(Game);
                    Matches = await GameService.GetMatchesAsync(Game);
                }
                else
                {
                    NavigationManager!.NavigateTo("/Error", true);
                }
            }
            catch (Exception)
            {
                NavigationManager!.NavigateTo("/Error", true);
            }
        }

        protected bool AuthorizeUser()
        {
            return (UserAuthorized = Game!.Players.Select(p => p.UserID).Contains(User!.UserID));
        }
    }
}
