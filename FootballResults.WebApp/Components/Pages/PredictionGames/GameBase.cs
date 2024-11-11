using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.WebApp.Services.Football.Client;
using FootballResults.WebApp.Services.LiveUpdates;
using FootballResults.WebApp.Services.Predictions;
using FootballResults.WebApp.Services.Time;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages.PredictionGames
{
    public class GameBase : LiveUpdatePageBase
    {
        [Inject]
        protected IClientTimeService ClientTimeService { get; set; } = default!;

        [Inject]
        protected IPredictionGameService GameService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [CascadingParameter(Name = "User")]
        protected User? User { get; set; }

        [Parameter]
        public string? GameID { get; set; }

        protected PredictionGame? Game { get; set; }

        protected League? SelectedLeague { get; set; }

        protected IEnumerable<Match>? Matches
        {
            get
            {
                if (Game != null && SelectedLeague != null)
                {
                    return Game.Matches.Where(m => m.LeagueSeason == SelectedLeague.CurrentSeason);
                }
                else
                {
                    return null;
                }
            }
        }

        protected IEnumerable<Match> UpcomingMatchesToday => (Matches ?? new List<Match>()).Where(m => !m.IsFinished && m.Date.GetValueOrDefault().Add(ClientUtcDiff).Date == ClientDate.Date).OrderBy(m => m.Date);

        protected IEnumerable<Match> FinishedMatchesToday => (Matches ?? new List<Match>()).Where(m => m.IsFinished && m.Date.GetValueOrDefault().Add(ClientUtcDiff).Date == ClientDate.Date).OrderByDescending(m => m.Date);

        protected IEnumerable<Match> UpcomingMatchesDecludingToday => (Matches ?? new List<Match>()).Where(m => !m.IsFinished && m.Date.GetValueOrDefault().Add(ClientUtcDiff).Date != ClientDate.Date).OrderBy(m => m.Date);

        protected IEnumerable<Match> FinishedMatchesDecludingToday => (Matches ?? new List<Match>()).Where(m => m.IsFinished && m.Date.GetValueOrDefault().Add(ClientUtcDiff).Date != ClientDate.Date).OrderByDescending(m => m.Date);

        protected bool MatchInProgress => (Matches ?? new List<Match>()).Any(m => m.IsInProgress);

        protected bool UserAuthorized { get; set; }

        protected bool ShowJoinKey { get; set; } = false;

        protected TimeSpan ClientUtcDiff { get; set; }

        protected DateTime ClientDate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
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
                        NavigationManager.NavigateTo("/error", true);
                    }
                    else if (!AuthorizeUser())
                    {
                        NavigationManager.NavigateTo("/access-denied", true);
                    }
                }
            }
        }

        protected async Task LoadGameAsync()
        {
            try
            {
                await UpdateLock.WaitAsync();
                Game = await GameService!.GetPredictionGameAsync(int.Parse(GameID!));
                UpdateLock.Release();

                if (Game != null)
                {
                    SelectedLeague = SelectedLeague ?? Game.LeagueSeasons
                        .Select(ls => ls.League)
                        .OrderBy(l => l.Name)
                        .First();
                }
                else
                {
                    NavigationManager!.NavigateTo("/404", true);
                }
            }
            catch (Exception)
            {
                NavigationManager!.NavigateTo("/error", true);
            }
        }

        protected bool AuthorizeUser()
        {
            return UserAuthorized = Game!.Players.Select(p => p.ID).Contains(User!.ID);
        }

        protected async Task ChangeLeague(League league)
        {
            SelectedLeague = league;
            await InvokeAsync(StateHasChanged);
        }

        protected override async void OnUpdateMessageReceivedAsync(object? sender, UpdateMessageType notificationType)
        {
            if (notificationType == UpdateMessageType.MatchesUpdated)
            {
                if (Game != null && SelectedLeague != null)
                {
                    await UpdateLock.WaitAsync();
                    await GameService!.ReloadMatchesAsync(Game);
                    UpdateLock.Release();
                }
            }

            await InvokeAsync(StateHasChanged);
        }

        protected void OnInviteButtonClicked()
        {
            ShowJoinKey = !ShowJoinKey;
        }

        protected async Task OnDeleteButtonClickedAsync()
        {
            await GameService.DeletePredictionGameAsync(Game!);
            Game = null;
            NavigationManager.NavigateTo("/prediction-games", true);
        }
    }
}
