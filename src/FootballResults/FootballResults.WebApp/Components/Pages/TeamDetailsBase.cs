using Microsoft.AspNetCore.Components;
using FootballResults.Models;
using FootballResults.WebApp.Services;

namespace FootballResults.WebApp.Components.Pages
{
    public class TeamDetailsBase : ComponentBase
    {
        [Inject]
        protected ITeamService? TeamService { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Parameter]
        public string? TeamName { get; set; }
        protected Team? Team { get; set; }
        protected IEnumerable<Match>? Matches { get;  set; }
        protected IEnumerable<Player>? Squad { get; set; }
        protected string? ActiveSubMenu { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadTeam();
            await LoadMatches();
        }

        protected async Task LoadTeam()
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

        protected async Task LoadMatches()
        {
            ActiveSubMenu = "matches";
            if (Matches != null) return; // already loaded        

            try
            {
                Matches = null;
                /* TODO */

                
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }

        protected async Task LoadSquad()
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
    }
}
