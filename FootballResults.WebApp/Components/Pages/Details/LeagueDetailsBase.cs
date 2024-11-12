using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.WebApp.Components.Utilities;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Time;

namespace FootballResults.WebApp.Components.Pages.Details
{
    public class LeagueDetailsBase : MatchFilterablePageBase
    {
        protected enum LeagueDetailsSubMenu
        {
            Matches,
            Standings,
            TopScorers
        }

        [Inject]
        protected ILeagueService? LeagueService { get; set; }

        [Parameter]
        public string? LeagueName { get; set; }

        protected League? League { get; set; }

        protected LeagueDetailsSubMenu? ActiveSubMenu { get; set; } = LeagueDetailsSubMenu.Matches;

        protected IEnumerable<LeagueStanding>? Standings { get; set; }
        protected IEnumerable<TopScorer>? TopScorers { get; set; }

        protected override MatchOrderOption MatchOrderOption { get; set; } = MatchOrderOption.RoundThenDateAsc;

        protected override async Task OnInitializedAsync()
        {
            await LoadLeagueAsync();
            await base.OnInitializedAsync();
        }

        protected async Task LoadLeagueAsync()
        {
            League = await LeagueService!.GetLeagueByNameAsync(LeagueName!);

            if (League == null)
            {
                NavigationManager.NavigateTo("/404", true);
            }
        }

        protected override void InitializeMatchFilters()
        {
            MatchFilterParameters = new MatchFilterParameters()
            {
                LeagueFilter = LeagueName,
                SeasonFilter = League!.CurrentSeason?.Year ?? League!.LeagueSeasons.Max(ls => ls.Year)
            };
        }

        protected async Task LoadStandingsAsync()
        {
            if (MatchFilterParameters?.SeasonFilter != null)
            {
                Standings = await LeagueService!.GetStandingsForLeagueAndSeasonAsync(LeagueName!, (int)MatchFilterParameters.SeasonFilter);
            }
        }

        protected async Task LoadTopScorersAsync()
        {
            if (MatchFilterParameters?.SeasonFilter != null)
            {
                TopScorers = await LeagueService!.GetTopScorersForLeagueAndSeasonAsync(LeagueName!, (int)MatchFilterParameters.SeasonFilter);
            }  
        }

        protected void OnMatchesSelected()
        {
            ActiveSubMenu = LeagueDetailsSubMenu.Matches;
        }

        protected async Task OnStandingsSelected()
        {
            ActiveSubMenu = LeagueDetailsSubMenu.Standings;
            await LoadStandingsAsync();
        }

        protected async Task OnTopScorersSelected()
        {
            ActiveSubMenu = LeagueDetailsSubMenu.TopScorers;
            await LoadTopScorersAsync();
        }

        protected void OnBookmarkClicked()
        {
            StateHasChanged();
        }
    }
}
