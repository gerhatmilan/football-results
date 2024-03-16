using Microsoft.AspNetCore.Components;
using FootballResults.Models;
using FootballResults.WebApp.Services;

namespace FootballResults.WebApp.Components.Pages
{
    public partial class MatchesBase : ComponentBase
    {
        protected struct LeagueWithMatches
        {
            public League league;
            public IEnumerable<Match> matches;
        }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Inject]
        protected IMatchService? MatchService { get; set; }

        [Inject]
        protected ILeagueService? LeagueService { get; set; }

        protected DateTime SelectedDate { get; set; }

        protected ICollection<LeagueWithMatches>? LeaguesWithMatches { get; set; }

        protected override async Task OnInitializedAsync()
        {
            SelectedDate = DateTime.Now.ToLocalTime();
            await LoadMatchesAsync();   
        }

        protected async void OnSelectedDateChangedInCalendar(DateTime newDate)
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
            try
            {
                LeaguesWithMatches = null;
                var cache = new List<LeagueWithMatches>();
                var leagues = await LeagueService!.GetLeagues();
                var matches = await MatchService!.GetMatchesForDate(SelectedDate);

                foreach (League league in leagues.OrderBy(l => l.Name))
                {
                    var matchesForLeagueAndDate = matches.Where(m => m.League.LeagueID == league.LeagueID).OrderBy(m => m.Date);
                    if (matchesForLeagueAndDate.Any())
                    {
                        cache.Add(new LeagueWithMatches { league = league, matches = matchesForLeagueAndDate.ToList() });
                    }
                }    

                LeaguesWithMatches = cache;
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }
    }
}
