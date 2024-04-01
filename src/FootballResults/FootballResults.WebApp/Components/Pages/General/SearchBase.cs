using FootballResults.Models.Football;
using FootballResults.WebApp.Services.Football;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages.General
{
    public class SearchBase : ComponentBase
    {
        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Inject]
        protected ILeagueService? LeagueService { get; set; }

        [Inject]
        protected ITeamService? TeamService { get; set; }

        [SupplyParameterFromQuery(Name = "value")]
        protected string? SearchValue { get; set; }

        protected IEnumerable<League>? Leagues { get; set; }
        protected IEnumerable<Team>? Teams { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (String.IsNullOrEmpty(SearchValue))
                {
                    NavigationManager!.NavigateTo("/", true);
                }
                else
                {
                    Leagues = await LeagueService!.SearchAsync(SearchValue!);
                    Teams = await TeamService!.SearchAsync(SearchValue!);
                }
            }
            catch (HttpRequestException)
            {
                NavigationManager!.NavigateTo("/Error");
            }
        }
    }
}
