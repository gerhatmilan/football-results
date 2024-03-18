using Microsoft.AspNetCore.Components;
using FootballResults.Models;
using FootballResults.WebApp.Services;
using FootballResults.WebApp.Components.Other;

namespace FootballResults.WebApp.Components.Pages
{
    public class TeamDetailsBase : ComponentBase, IMatchFilterable
    {
        [Inject]
        protected ITeamService? TeamService { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Parameter]
        public string? TeamName { get; set; }
        protected Team? Team { get; set; }
        public IEnumerable<Match>? Matches { get;  set; }
        protected IEnumerable<Player>? Squad { get; set; }
        protected string? ActiveSubMenu { get; set; } = "matches";

        protected MatchFilterParameters? MatchFilterParameters { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadTeamAsync();
            InitializeFilters();
        }

        protected async Task LoadTeamAsync()
        {
            try
            {
                Team = await TeamService!.GetTeamByName(TeamName!);
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }

        protected void InitializeFilters()
        {
            MatchFilterParameters = new MatchFilterParameters();
            MatchFilterParameters.TeamFilter = TeamName;
            MatchFilterParameters.SeasonFilter = (DateTime.Now.ToLocalTime().Month >= 8) ? DateTime.Now.ToLocalTime().Year : DateTime.Now.ToLocalTime().Year - 1;
        }

        protected async Task LoadSquadAsync()
        {
            ActiveSubMenu = "squad";
            if (Squad != null) return; // already loaded

            try
            {
                Squad = null;
                var players = await TeamService!.GetSquadForTeam(Team!.Name);
                Squad = players.ToList();
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
                (leagueID, matches) => (leagueID, Matches!.Where(m => m.LeagueID.Equals(leagueID)).OrderBy(m => m.Date).ToList())
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
