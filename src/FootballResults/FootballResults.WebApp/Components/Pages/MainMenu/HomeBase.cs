using Microsoft.AspNetCore.Components;
using FootballResults.Models.Football;
using FootballResults.WebApp.Services.Football;

namespace FootballResults.WebApp.Components.Pages.MainMenu
{
    public partial class HomeBase : ComponentBase
    {
        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Inject]
        protected IMatchService? MatchService { get; set; }

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
            var selectedDateInUtc = SelectedDate.Add(ClientUtcDiff.Negate());

            try
            {
                Matches = null;
                var matches = await MatchService!.GetMatchesForDateAsync(selectedDateInUtc);

                // in case the matches based on client's date extends to the next or previous day according to UTC time
                // e.g if the client time is UTC+5, and the match is at 3:00 at client's time, then the match starts at 22:00 UTC, but
                // if a match starts at 5:00 at client's time, then the match starts at 0:00 UTC, which extends to the next day

                matches = matches.Concat(await MatchService!.GetMatchesForDateAsync(selectedDateInUtc.AddDays(1).Date));
                matches = matches.Concat(await MatchService!.GetMatchesForDateAsync(selectedDateInUtc.AddDays(-1).Date));

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
                var upcomingMatches = await MatchService!.GetMatchesForYearAsync(DateTime.UtcNow.Year);
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
