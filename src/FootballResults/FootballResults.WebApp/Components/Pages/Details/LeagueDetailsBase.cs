using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.WebApp.Components.Utilities;
using FootballResults.WebApp.Services.Football;

namespace FootballResults.WebApp.Components.Pages.Details
{
    public class LeagueDetailsBase : ComponentBase, IMatchFilterable
    {
        protected enum LeagueDetailsSubMenu
        {
            Matches,
            Standings,
            TopScorers
        }

        [Inject]
        protected ILeagueService? LeagueService { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Parameter]
        public string? LeagueName { get; set; }
        protected League? League { get; set; }
        public IEnumerable<Match>? Matches { get; set; }
        protected IEnumerable<LeagueStanding>? Standings { get; set; }
        protected IEnumerable<TopScorer>? TopScorers { get; set; }
        protected LeagueDetailsSubMenu? ActiveSubMenu { get; set; } = LeagueDetailsSubMenu.Matches;

        protected MatchFilterParameters? MatchFilterParameters { get; set; }
        protected int? SeasonFilter => MatchFilterParameters?.SeasonFilter;

        protected MatchOrderOption MatchOrderOption { get; set; } = MatchOrderOption.RoundThenDateAsc;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await LoadLeagueAsync();
                InitializeFilters();
            }
            catch (HttpRequestException ex)
            {
                switch (ex.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        NavigationManager?.NavigateTo("/notfound", true);
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        NavigationManager?.NavigateTo("/error", true);
                        break;
                }
            }
        }

        protected async Task LoadLeagueAsync()
        {
            League = await LeagueService!.GetLeagueByNameAsync(LeagueName!);
        }

        protected void InitializeFilters()
        {
            MatchFilterParameters = new MatchFilterParameters()
            {
                LeagueFilter = LeagueName,
                SeasonFilter = League!.CurrentSeason?.Year ?? League!.LeagueSeasons.Max(ls => ls.Year)
            };
        }

        protected async Task LoadStandingsAsync()
        {
            if (Standings == null && SeasonFilter != null)
            {
                try
                {
                    Standings = await LeagueService!.GetStandingsForLeagueAndSeasonAsync(LeagueName!, (int)SeasonFilter);
                }
                catch (HttpRequestException)
                {
                    NavigationManager?.NavigateTo("/error", true);
                }
            }
        }

        protected async Task LoadTopScorersAsync()
        {
            if (TopScorers == null && SeasonFilter != null)
            {
                try
                {
                    TopScorers = await LeagueService!.GetTopScorersForLeagueAndSeasonAsync(LeagueName!, (int)SeasonFilter);
                }
                catch (HttpRequestException)
                {
                    NavigationManager?.NavigateTo("/error", true);
                }
            }  
        }

        protected void OnMatchFilterSubmitted(IEnumerable<Match> matches)
        {
            Matches = matches.ToList();
            StateHasChanged();
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

        protected void OnSeasonChanged(int? season)
        {
            Standings = null;
            TopScorers = null;
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
