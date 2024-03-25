using Microsoft.AspNetCore.Components;
using FootballResults.Models;
using FootballResults.WebApp.Services;

namespace FootballResults.WebApp.Components.Pages.MainMenu
{
    public partial class MatchesBase : ComponentBase
    {
        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Inject]
        protected IMatchService? MatchService { get; set; }

        [Inject]
        protected ILeagueService? LeagueService { get; set; }

        protected DateTime SelectedDate { get; set; }

        protected TimeSpan ClientUtcDiff { get; set; }

        protected IEnumerable<Match>? Matches { get; set; }

        protected IEnumerable<Match>? UpcomingMatches { get; set; }

        protected void FilterMatchesBasedOnClientDate()
        {
            Matches = Matches!
                .Where(m => m.Date.GetValueOrDefault().Add(ClientUtcDiff).Day == SelectedDate.Date.Day)
                .ToList();
        }

        protected async void OnSelectedDateChangedInCalendar(DateTime newDate)
        {
            if (newDate.Date != SelectedDate.Date)
            {
                SelectedDate = newDate;
                await LoadMatchesAsync();
                FilterMatchesBasedOnClientDate();
                StateHasChanged();
            }
        }

        protected List<(int leagueID, List<Match> matches)> GetMatchesByLeague(IEnumerable<Match> matches)
        {
            return matches!
            .GroupBy(
                m => m.LeagueID,
                (leagueID, matches) => (leagueID, matches!.Where(m => m.LeagueID.Equals(leagueID)).OrderBy(m => m.Date).ToList())
            )
            .ToList();
        }

        protected async Task LoadMatchesAsync()
        {
            try
            {
                Matches = null;
                var matches = await MatchService!.GetMatchesForDate(SelectedDate);
                matches = matches.Concat(await MatchService!.GetMatchesForDate(SelectedDate.AddDays(-1).Date));
                matches = matches.Concat(await MatchService!.GetMatchesForDate(SelectedDate.AddDays(1).Date));
                Matches = matches.ToList();
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }

        protected async Task LoadUpcomingMatchesAsync()
        {
            try
            {
                UpcomingMatches = null;
                var upcomingMatches = await MatchService!.GetMatchesForYear(DateTime.Now.Year);
                UpcomingMatches = upcomingMatches
                    .Where(m => m.Date > DateTime.UtcNow)
                    .OrderBy(m => m.Date)
                    .Take(5)
                    .ToList();
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }
    }
}
