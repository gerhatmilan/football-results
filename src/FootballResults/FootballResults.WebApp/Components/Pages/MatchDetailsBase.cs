using Microsoft.AspNetCore.Components;
using FootballResults.Models;
using FootballResults.WebApp.Services;
using FootballResults.WebApp.Components.Other;
using System.Numerics;

namespace FootballResults.WebApp.Components.Pages
{
    public class MatchDetailsBase : ComponentBase, IMatchFilterable
    {
        [Inject]
        protected IMatchService? MatchService { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Parameter]
        public String? MatchID { get; set; }
        protected Match? Match { get; set; }
        public IEnumerable<Match>? Matches { get; set; }

        protected MatchFilterParameters? MatchFilterParameters { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadMatchAsync();
            InitializeFilters();
        }

        protected async Task LoadMatchAsync()
        {
            try
            {
                Match = await MatchService!.GetMatchByID(Int32.Parse(MatchID!));
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
            MatchFilterParameters.SeasonFilter = (DateTime.Now.ToLocalTime().Month >= 8) ? DateTime.Now.ToLocalTime().Year : DateTime.Now.ToLocalTime().Year - 1;
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
    }
}
