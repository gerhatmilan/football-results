using Microsoft.AspNetCore.Components;
using FootballResults.WebApp.Services;
using FootballResults.Models;

namespace FootballResults.WebApp.Components.Pages.MainMenu
{
    public partial class LeaguesBase : ComponentBase
    {
        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Inject]
        protected ILeagueService? LeagueService { get; set; }

        protected IEnumerable<Country>? CountriesWithLeagues { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadLeaguesAsync();
        }

        protected async Task LoadLeaguesAsync()
        {
            try
            {
                CountriesWithLeagues = await LeagueService!.GetCountriesWithLeaguesAsync();
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }
        }
    }
}
