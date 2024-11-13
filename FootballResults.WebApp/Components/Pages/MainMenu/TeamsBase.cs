using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.WebApp.Services.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.ViewModels.Football;

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

        protected IEnumerable<Team> FavoriteTeams { get; set; } = Enumerable.Empty<Team>();

        protected string Filter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadTeamsAsync();
        }

        protected override void OnParametersSet()
        {
            if (CountriesWithTeams != null)
            {
                FavoriteTeams = ViewHelper.GetFavoriteTeamsOnly(CountriesWithTeams.SelectMany(c => c.Teams), User);
            }
        }

        protected async Task LoadTeamsAsync()
        {
            CountriesWithTeams = await TeamService!.GetCountriesWithTeamsAsync();
        }

        protected void OnBookmarkClicked()
        {
            StateHasChanged();
        }
    }
}
