using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.WebApp.Services.Football;

namespace FootballResults.WebApp.Components.Pages.MainMenu
{
    public partial class LeaguesBase : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected ILeagueService LeagueService { get; set; } = default!;

        [CascadingParameter(Name = "User")]
        protected User User { get; set; } = default!;

        protected IEnumerable<Country>? CountriesWithLeagues { get; set; }

        protected string Filter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadLeaguesAsync();
        }

        protected async Task LoadLeaguesAsync()
        {
            CountriesWithLeagues = await LeagueService!.GetCountriesWithLeaguesAsync();
        }
    }
}
