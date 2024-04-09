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
        protected IMatchService MatchService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [CascadingParameter(Name = "User")]
        protected User? User { get; set; }

        [Parameter]
        public string? GameID { get; set; }

        protected PredictionGame? Game { get; set; }

        protected IEnumerable<LeagueStanding> LeagueStandings { get; set; } = new List<LeagueStanding>();

        protected ICollection<(League, IEnumerable<Match>)> MatchesByLeagues { get; set; } = new List<(League, IEnumerable<Match>)>();

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
                else if (AuthorizeUser())
                {
                    await LoadStandingsForLeaguesAsync();
                    await LoadMatchesForLeaguesAsync();
                }
                else
                {
                    NavigationManager!.NavigateTo("/access-denied", true);
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
                foreach (GameLeague gameLeague in Game!.GameLeagues)
                {
                    LeagueStandings = LeagueStandings.Concat(await LeagueService.GetStandingsForLeagueAndSeasonAsync(gameLeague.League.Name, gameLeague.Season));
                }
            }
            catch (Exception)
            {
                NavigationManager!.NavigateTo("/Error", true);
            }
        }

        protected async Task LoadMatchesForLeaguesAsync()
        {
            try
            {
                foreach (GameLeague gameLeague in Game!.GameLeagues)
                {
                    var matches = await MatchService.GetMatchesForLeagueAndSeasonAsync(gameLeague.League.Name, gameLeague.Season);
                    MatchesByLeagues.Add((gameLeague.League, matches));
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
