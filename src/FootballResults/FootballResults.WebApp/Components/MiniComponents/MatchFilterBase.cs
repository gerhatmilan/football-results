using Microsoft.AspNetCore.Components;
using FootballResults.WebApp.Services;
using FootballResults.Models;
using FootballResults.WebApp.Components.Other;

namespace FootballResults.WebApp.Components.MiniComponents
{
    public class MatchFilterBase : ComponentBase
    {
        [Inject]
        protected IMatchService? MatchService { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Parameter]
        public EventCallback<IEnumerable<Match>> FilterSubmitted { get; set; }

        [Parameter]
        public MatchFilterParameters? FilterParameters { get; set; }

        protected IEnumerable<Match>? Matches { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await FilterMatchesAsync();
        }

        protected async Task FilterMatchesAsync()
        {
            try
            {
                var result = await MatchService!.SearchForMatch(FilterParameters!.DateFilter, FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter);

                if (FilterParameters.HomeAwayFilter == "Home")
                    result = result.Where(m => m.HomeTeam.Name == FilterParameters.TeamFilter);
                else if (FilterParameters.HomeAwayFilter == "Away")
                    result = result.Where(m => m.AwayTeam.Name == FilterParameters.TeamFilter);

                Matches = result.ToList();
                await FilterSubmitted.InvokeAsync(Matches);
            }
            catch (HttpRequestException)
            {
                NavigationManager!.NavigateTo("Error", true);
            }
        }

        protected async void SortByDateAscending()
        {
            Matches = Matches!.OrderBy(m => m.Date).ToList();
            await FilterSubmitted.InvokeAsync(Matches);
        }

        protected async void SortByDateDescending()
        {
            Matches = Matches!.OrderByDescending(m => m.Date).ToList();
            await FilterSubmitted.InvokeAsync(Matches);
        }
    }
}
