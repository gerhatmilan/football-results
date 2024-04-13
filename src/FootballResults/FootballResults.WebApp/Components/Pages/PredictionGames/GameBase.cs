using FootballResults.Models.Football;
using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
using FootballResults.WebApp.Components.Pages.MainMenu;
using FootballResults.WebApp.Services.Chat;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Predictions;
using FootballResults.WebApp.Services.Time;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages.PredictionGames
{
    public class GameBase : ComponentBase
    {
        [Inject]
        protected IClientTimeService ClientTimeService { get; set; } = default!;

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

        protected League? SelectedLeague { get; set; }
        protected IEnumerable<LeagueStanding>? LeagueStandings { get; set; }
        protected IEnumerable<Match>? Matches { get; set; }

        protected IEnumerable<Match> UpcomingMatchesToday { get => (Matches ?? new List<Match>()).Where(m => !m.IsFinished() && m.Date.GetValueOrDefault().Add(ClientUtcDiff).Date == ClientDate).OrderBy(m => m.Date); }

        protected IEnumerable<Match> FinishedMatchesToday { get => (Matches ?? new List<Match>()).Where(m => m.IsFinished() && m.Date.GetValueOrDefault().Add(ClientUtcDiff).Date == ClientDate).OrderByDescending(m => m.Date); }

        protected IEnumerable<Match> UpcomingMatchesDecludingToday { get => (Matches ?? new List<Match>()).Where(m => !m.IsFinished() && m.Date.GetValueOrDefault().Add(ClientUtcDiff).Date != ClientDate).OrderBy(m => m.Date); }

        protected IEnumerable<Match> FinishedMatchesDecludingToday { get => (Matches ?? new List<Match>()).Where(m => m.IsFinished() && m.Date.GetValueOrDefault().Add(ClientUtcDiff).Date != ClientDate).OrderByDescending(m => m.Date); }

        protected bool UserAuthorized { get; set; }

        protected bool ShowJoinKey { get; set; } = false;

        protected TimeSpan ClientUtcDiff { get; set; }

        protected DateTime ClientDate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ClientUtcDiff = await ClientTimeService.GetClientUtcDiffAsync();
            ClientDate = (await ClientTimeService.GetClientDateAsync()).Date;
        }

        protected bool IsLoading { get; set; } = false;

        protected override async Task OnParametersSetAsync()
        {
            // both parameters are set
            if (GameID != null && User != null)
            {
                // Game not loaded yet
                if (Game == null && !IsLoading)
                {
                    IsLoading = true;
                    await LoadGameAsync();
                    IsLoading = false;

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
                    SelectedLeague = Game.Leagues.ElementAt(0);
                    LeagueStandings = await GameService.GetStandingsAsync(Game);
                    Matches = await GameService.GetMatchesAsync(Game, SelectedLeague);
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

        protected async Task ChangeLeague(League league)
        {
            SelectedLeague = league;
            Matches = null;
            Matches = await GameService.GetMatchesAsync(Game!, SelectedLeague);
            await InvokeAsync(StateHasChanged);
        }
    }
}
