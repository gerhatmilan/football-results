using Microsoft.AspNetCore.Components;
using FootballResults.WebApp.Services;
using FootballResults.Models;

namespace FootballResults.WebApp.Components.Pages
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
                CountriesWithTeams = await TeamService!.GetCountriesWithTeams();
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }
    }
}
