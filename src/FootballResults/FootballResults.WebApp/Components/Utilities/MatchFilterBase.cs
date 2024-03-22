using Microsoft.AspNetCore.Components;
using FootballResults.WebApp.Services;
using FootballResults.Models;

namespace FootballResults.WebApp.Components.Utilities
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
        public EventCallback<int?> SeasonChanged { get; set; }

        [Parameter]
        public EventCallback<MatchOrderOption> MatchOrderChanged { get; set; }

        [Parameter]
        public MatchFilterParameters? FilterParameters { get; set; }

        [Parameter]
        public IMatchFilterable? FilterTarget { get; set; }

        protected IEnumerable<Match>? Matches { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await FilterMatchesAsync();
        }

        protected async Task FilterMatchesAsync()
        {
            try
            {
                var result = await MatchService!.SearchForMatch(FilterParameters!.YearFilter, FilterParameters!.MonthFilter, FilterParameters!.DayFilter
                    , FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter);

                if (!string.IsNullOrEmpty(FilterParameters.OpponentNameFilter))
                    result = result.Where(m => m.HomeTeam.Name.ToLower().Equals(FilterParameters.OpponentNameFilter.ToLower()) || m.AwayTeam.Name.ToLower().Equals(FilterParameters.OpponentNameFilter.ToLower()));
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

        protected async Task OnSeasonChanged()
        {
            await SeasonChanged.InvokeAsync(FilterParameters!.SeasonFilter);
        }

        protected async Task OnMatchOrderChanged(MatchOrderOption newOrderOption)
        {
            await MatchOrderChanged.InvokeAsync(newOrderOption);
        }
    }
}
