using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.WebApp.Services.Football;
using FootballResults.DataAccess.Entities.Users;

namespace FootballResults.WebApp.Components.Pages.MainMenu
{
    public partial class TeamsBase : ComponentBase
    {
        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Inject]
        protected ITeamService? TeamService { get; set; }

        [CascadingParameter(Name = "User")]
        protected User User { get; set; } = default!;

        protected IEnumerable<Country>? CountriesWithTeams { get; set; }

        protected string Filter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadTeamsAsync();
        }

        protected async Task LoadTeamsAsync()
        {
            CountriesWithTeams = await TeamService!.GetCountriesWithTeamsAsync();
        }
    }
}
