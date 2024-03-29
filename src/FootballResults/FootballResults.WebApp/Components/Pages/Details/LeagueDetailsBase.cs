using Microsoft.AspNetCore.Components;
using FootballResults.Models;
using FootballResults.WebApp.Services;
using FootballResults.WebApp.Components.Utilities;

namespace FootballResults.WebApp.Components.Pages.Details
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

        protected MatchOrderOption MatchOrderOption { get; set; } = MatchOrderOption.RoundThenDateAsc;

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
                League = await LeagueService!.GetLeagueByNameAsync(LeagueName!);
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
                Standings = await LeagueService!.GetStandingsForLeagueAndSeasonAsync(LeagueName!, (int)SeasonFilter!);
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
                TopScorers = await LeagueService!.GetTopScorersForLeagueAndSeasonAsync(LeagueName!, (int)SeasonFilter!);
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }

        protected List<(int leagueID, List<Match> matches)> GetMatchesByLeague()
        {
            return Matches!
            .GroupBy(
                m => m.LeagueID,
                (leagueID, matches) => (leagueID, Matches!.Where(m => m.LeagueID.Equals(leagueID)).ToList())
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

        protected void OnMatchOrderChanged(MatchOrderOption newOrderOption)
        {
            if (newOrderOption != MatchOrderOption)
            {
                MatchOrderOption = newOrderOption;
                StateHasChanged();
            }
        }

        protected void OnBookmarkClicked()
        {
            StateHasChanged();
        }
    }
}
