using FootballResults.Models;
using FootballResults.WebApp.Services;
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
                    Leagues = await LeagueService!.Search(SearchValue!);
                    Teams = await TeamService!.Search(SearchValue!);
                }
            }
            catch (HttpRequestException)
            {
                NavigationManager!.NavigateTo("/Error");
            }
        }
    }
}
