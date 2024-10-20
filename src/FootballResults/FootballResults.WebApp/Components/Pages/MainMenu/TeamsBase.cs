using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.WebApp.Services.Football;

namespace FootballResults.WebApp.Components.Pages.MainMenu
{
    public partial class TeamsBase : ComponentBase
    {
        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Inject]
        protected ITeamService? TeamService { get; set; }

        protected IEnumerable<Country>? CountriesWithTeams { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadTeamsAsync();
        }

        protected async Task LoadTeamsAsync()
        {
            try
            {
                CountriesWithTeams = await TeamService!.GetCountriesWithTeamsAsync();
            }
            catch (Exception)
            {
                NavigationManager?.NavigateTo("/error", true);
            }
        }
    }
}
