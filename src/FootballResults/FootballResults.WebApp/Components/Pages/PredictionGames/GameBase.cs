using FootballResults.Models.Football;
using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Predictions;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages.PredictionGames
{
    public class GameBase : ComponentBase
    {
        [Inject]
        protected IPredictionGameService GameService { get; set; } = default!;

        [Inject]
        protected ILeagueService LeagueService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [CascadingParameter(Name = "User")]
        protected User? User { get; set; }

        [Parameter]
        public string? GameID { get; set; }

        protected PredictionGame? Game { get; set; }

        protected IEnumerable<LeagueStanding> LeagueStandings { get; set; } = new List<LeagueStanding>();

        protected IEnumerable<Match>? Matches { get; set; }

        protected bool UserAuthorized { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            // both parameters are set
            if (GameID != null && User != null)
            {
                // Game not loaded yet
                if (Game == null)
                {
                    await LoadGameAsync();
                }

                // Game loaded but not found
                if (Game == null)
                {
                    NavigationManager!.NavigateTo("/Error", true);
                }
                else
                {
                    AuthorizeUser();
                    await LoadStandingsForLeaguesAsync();
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

        protected async Task LoadStandingsForLeaguesAsync()
        {
            try
            {
                foreach (League league in Game!.Leagues)
                {
                    LeagueStandings = LeagueStandings.Concat(await LeagueService.GetStandingsForLeagueAndSeasonAsync(league.Name, (int)league.CurrentSeason!));
                }
            }
            catch (Exception)
            {
                NavigationManager!.NavigateTo("/Error", true);
            }
        }

        protected void AuthorizeUser()
        {
            if (!Game!.Players.Select(p => p.UserID).Contains(User!.UserID))
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
