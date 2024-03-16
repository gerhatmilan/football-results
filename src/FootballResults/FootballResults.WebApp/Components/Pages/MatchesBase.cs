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

        protected DateTime SelectedDate { get; set; } = DateTime.Now;

        protected ICollection<LeagueWithMatches>? LeaguesWithMatches { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadMatches();   
        }

        protected async void OnSelectedDateChangedInCalendar(DateTime newDate)
        {
            SelectedDate = newDate;
            await LoadMatches();
            StateHasChanged();
        }

        protected async Task LoadMatches()
        {
            try
            {
                LeaguesWithMatches = null;
                var cache = new List<LeagueWithMatches>();

                var leagues = await LeagueService!.GetLeagues();
                foreach (League league in leagues)
                {
                    var matches = await MatchService!.GetMatchesForLeagueAndDate(league.Name, SelectedDate);
                    if (matches.Any())
                    {
                        cache.Add(new LeagueWithMatches { league = league, matches = matches });
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
