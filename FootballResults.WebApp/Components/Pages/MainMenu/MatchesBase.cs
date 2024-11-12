using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Time;
using FootballResults.WebApp.Services.LiveUpdates;
using FootballResults.Models.ViewModels.Football;

namespace FootballResults.WebApp.Components.Pages.MainMenu
{
    public partial class MatchesBase : LiveUpdatePageBase
    {
        [Inject]
        protected IClientTimeService ClientTimeService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected IMatchService MatchService { get; set; } = default!;

        [CascadingParameter(Name = "User")]
        protected User User { get; set; } = default!;

        protected TimeSpan ClientUtcDiff { get; set; } = default!;

        protected DateTime SelectedDate { get; set; }

        protected IEnumerable<Match>? Matches { get; set; }

        protected IEnumerable<LeagueMatchGroup> LeagueMatchGroups
        {
            get
            {
                return Matches != null ? ViewHelper.GetMatchesGroupedByLeague(Matches!, User) : Enumerable.Empty<LeagueMatchGroup>();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            ClientUtcDiff = await ClientTimeService.GetClientUtcDiffAsync();
        }

        protected async Task OnSelectedDateChangedInCalendarAsync(DateTime newDate)
        {
            await RefreshMatchesAsync(newDate);
        }

        protected async Task RefreshMatchesAsync(DateTime newDate)
        {
            if (newDate.Date != SelectedDate.Date)
            {
                SelectedDate = newDate;
                await LoadMatchesAsync();
                StateHasChanged();
            }
        }

        protected async Task LoadMatchesAsync()
        {
            var selectedDateInUtc = SelectedDate.Add(ClientUtcDiff.Negate());

            Matches = null;

            await UpdateLock.WaitAsync();

            // in case the matches based on client's date extends to the next or previous day according to UTC time
            // e.g if the client time is UTC+5, and the match is at 3:00 at client's time, then the match starts at 22:00 UTC, but
            // if a match starts at 5:00 at client's time, then the match starts at 0:00 UTC, which extends to the next day
            Matches = await MatchService.GetMatchesForIntervalAsync(selectedDateInUtc.AddDays(-1), selectedDateInUtc.AddDays(1));
            FilterMatchesBasedOnClientDate();

            UpdateLock.Release();
            InitialLoadCompletedEvent.Set();
        }

        protected void FilterMatchesBasedOnClientDate()
        {
            Matches = Matches!
                .Where(m => m.Date.GetValueOrDefault().Add(ClientUtcDiff).Day == SelectedDate.Date.Day)
                .ToList();
        }

        protected override async void OnUpdateMessageReceivedAsync(object? sender, UpdateMessageType notificationType)
        {
            if (notificationType == UpdateMessageType.MatchesUpdated)
            {
                if (Matches == null)
                {
                    InitialLoadCompletedEvent.WaitOne();
                }
                await LoadMatchesAsync();
                InitialLoadCompletedEvent.Reset();
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}
