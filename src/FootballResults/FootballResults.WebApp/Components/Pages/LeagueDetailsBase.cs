using Microsoft.AspNetCore.Components;
using FootballResults.Models;
using FootballResults.WebApp.Services;
using FootballResults.WebApp.Components.Other;

namespace FootballResults.WebApp.Components.Pages
{
    public class LeagueDetailsBase : ComponentBase, IMatchFilterable
    {
        [Inject]
        protected ILeagueService? LeagueService { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Parameter]
        public string? LeagueName { get; set; }
        protected League? League { get; set; }
        public IEnumerable<Match>? Matches { get; set; }

        protected IEnumerable<Standing>? Standings { get; set; }
        protected IEnumerable<TopScorer>? TopScorers { get; set; }
        protected string? ActiveSubMenu { get; set; } = "matches";
        protected int? SeasonFilter { get; set; }

        protected MatchFilterParameters? MatchFilterParameters { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadLeagueAsync();
            InitializeFilters();
            await LoadStandingsAsync();
            await LoadTopScorersAsync();
        }

        protected async Task LoadLeagueAsync()
        {
            try
            {
                League = await LeagueService!.GetLeagueByName(LeagueName!);
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }

        protected void InitializeFilters()
        {
            MatchFilterParameters = new MatchFilterParameters();
            MatchFilterParameters.LeagueFilter = LeagueName;
            SeasonFilter = League!.CurrentSeason;
            MatchFilterParameters.SeasonFilter = SeasonFilter;
        }

        protected async Task LoadStandingsAsync()
        {
            try
            {
                Standings = await LeagueService!.GetStandingsForLeagueAndSeason(LeagueName!, (int)SeasonFilter!);
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }

        protected async Task LoadTopScorersAsync()
        {
            try
            {
                TopScorers = await LeagueService!.GetTopScorersForLeagueAndSeason(LeagueName!, (int)SeasonFilter!);
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }

        protected List<(string round, List<Match> matches)> GetMatchesByRound()
        {
            return Matches!
            .GroupBy(
                m => m.Round,
                (round, matches) => (round, Matches!.Where(m => m.Round.Equals(round)).OrderBy(m => m.Date).ToList())
            )
            .ToList();
        }

        protected void OnMatchFilterSubmitted(IEnumerable<Match> matches)
        {
            Matches = matches.ToList();
            StateHasChanged();
        }

        protected async void OnSeasonChanged(int? season)
        {
            if (season != null)
            {
                SeasonFilter = season;
                await LoadStandingsAsync();
                await LoadTopScorersAsync();
                StateHasChanged();
            }            
        }
    }
}
