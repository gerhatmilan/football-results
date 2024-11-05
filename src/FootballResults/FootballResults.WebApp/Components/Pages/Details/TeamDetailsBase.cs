using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.WebApp.Components.Utilities;
using FootballResults.WebApp.Services.Football;
using FootballResults.DataAccess.Entities.Users;

namespace FootballResults.WebApp.Components.Pages.Details
{
    public class TeamDetailsBase : MatchFilterablePageBase
    {
        [Inject]
        protected ITeamService? TeamService { get; set; }

        [CascadingParameter(Name = "User")]
        public User User { get; set; } = default!;

        [Parameter]
        public string? TeamName { get; set; }
        protected Team? Team { get; set; }
        protected IEnumerable<Player>? Squad { get; set; }
        protected string? ActiveSubMenu { get; set; } = "matches";

        protected override MatchOrderOption MatchOrderOption { get; set; } = MatchOrderOption.DateAsc;

        protected override async Task OnInitializedAsync()
        {
            await LoadTeamAsync();
            await base.OnInitializedAsync();
        }

        protected async Task LoadTeamAsync()
        {
            try
            {
                Team = await TeamService!.GetTeamByNameAsync(TeamName!);

                if (Team == null)
                {
                    NavigationManager.NavigateTo("/404", true);
                }
            }
            catch (Exception)
            {
                NavigationManager?.NavigateTo("/error", true);
            }
        }

        protected override void InitializeMatchFilters()
        {
            MatchFilterParameters = new MatchFilterParameters();
            MatchFilterParameters.TeamFilter = TeamName;
            MatchFilterParameters.SeasonFilter = DateTime.Now.ToLocalTime().Month >= 8 ? DateTime.Now.ToLocalTime().Year : DateTime.Now.ToLocalTime().Year - 1;
        }

        protected async Task LoadSquadAsync()
        {
            ActiveSubMenu = "squad";
            if (Squad == null)
            {
                try
                {
                    Squad = null;
                    Squad = await TeamService!.GetSquadForTeamAsync(Team!.Name);
                }
                catch (Exception)
                {
                    NavigationManager?.NavigateTo("/error", true);
                }
            }
        }
    }
}
