using Microsoft.AspNetCore.Components;
using FootballResults.Models.Football;
using FootballResults.WebApp.Components.Utilities;
using FootballResults.WebApp.Services.Football;

namespace FootballResults.WebApp.Components.Pages.Details
{
    public class MatchDetailsBase : ComponentBase, IMatchFilterable
    {
        [Inject]
        protected IMatchService? MatchService { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Parameter]
        public string? MatchID { get; set; }
        protected Match? Match { get; set; }
        public IEnumerable<Match>? Matches { get; set; }

        protected MatchFilterParameters? MatchFilterParameters { get; set; }

        protected MatchOrderOption MatchOrderOption { get; set; } = MatchOrderOption.DateDesc;

        protected override async Task OnInitializedAsync()
        {
            await LoadMatchAsync();
            InitializeFilters();
        }

        protected async Task LoadMatchAsync()
        {
            try
            {
                Match = await MatchService!.GetMatchByIDAsync(int.Parse(MatchID!));
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }

        protected void InitializeFilters()
        {
            MatchFilterParameters = new MatchFilterParameters();
            MatchFilterParameters.TeamFilter = Match!.HomeTeam.Name;
            MatchFilterParameters.OpponentNameFilter = Match!.AwayTeam.Name;
            MatchFilterParameters.SeasonFilter = DateTime.Now.ToLocalTime().Month >= 8 ? DateTime.Now.ToLocalTime().Year : DateTime.Now.ToLocalTime().Year - 1;
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

        protected void OnMatchOrderChanged(MatchOrderOption newOrderOption)
        {
            if (newOrderOption != MatchOrderOption)
            {
                MatchOrderOption = newOrderOption;
                StateHasChanged();
            }
        }
    }
}
