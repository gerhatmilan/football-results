using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Time;
using FootballResults.WebApp.Services.LiveUpdates;

namespace FootballResults.WebApp.Components.Pages.MainMenu
{
    public partial class HomeBase : LiveUpdatePageBase
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

        protected IEnumerable<Match>? UpcomingMatches { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            ClientUtcDiff = await ClientTimeService.GetClientUtcDiffAsync();
            SelectedDate = DateTime.UtcNow.Add(ClientUtcDiff);

            await LoadMatchesAsync();
        }

        protected void FilterMatchesBasedOnClientDate()
        {
            Matches = Matches!
                .Where(m => m.Date.GetValueOrDefault().Add(ClientUtcDiff).Day == SelectedDate.Date.Day)
                .ToList();
        }

        protected async Task LoadMatchesAsync()
        {
            var selectedDateInUtc = SelectedDate.Add(ClientUtcDiff.Negate()).Date;

            try
            {
                // in case the matches based on client's date extends to the next or previous day according to UTC time
                // e.g if the client time is UTC+5, and the match is at 3:00 at client's time, then the match starts at 22:00 UTC, but
                // if a match starts at 5:00 at client's time, then the match starts at 0:00 UTC, which extends to the next day
                Matches = await MatchService!.GetMatchesForIntervalAsync(selectedDateInUtc.AddDays(-1), selectedDateInUtc.AddDays(1));

                FilterMatchesBasedOnClientDate();
                if (Matches != null && !Matches.Any())
                {
                    await LoadUpcomingMatchesAsync();
                }
            }
            catch (Exception)
            {
                NavigationManager?.NavigateTo("/error", true);
            }
        }

        protected async Task LoadUpcomingMatchesAsync()
        {
            try
            {
                UpcomingMatches = null;
                var upcomingMatches = await MatchService!.GetMatchesForYearAsync(DateTime.UtcNow.Year);
                UpcomingMatches = upcomingMatches
                    .Where(m => m.Date > DateTime.UtcNow)
                    .OrderBy(m => m.Date)
                    .Take(5)
                    .ToList();
            }
            catch (Exception)
            {
                NavigationManager?.NavigateTo("/error", true);
            }
        }

        protected override async void OnUpdateMessageReceivedAsync(object? sender, UpdateMessageType notificationType)
        {
            if (notificationType == UpdateMessageType.MatchesUpdated)
            {
                await LoadMatchesAsync();
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}
