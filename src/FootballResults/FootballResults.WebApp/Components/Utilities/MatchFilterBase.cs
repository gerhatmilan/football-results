using Microsoft.AspNetCore.Components;
using FootballResults.WebApp.Services;
using FootballResults.Models;
using System.Linq;

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

        protected TimeSpan ClientUtcDiff { get; set; }

        protected void FilterMatchesBasedOnClientDate()
        {
            if (FilterParameters?.MonthFilter != null)
                Matches = Matches!
                .Where(m => m.Date.GetValueOrDefault().Add(ClientUtcDiff).Month == FilterParameters.MonthFilter)
                .ToList();

            if (FilterParameters?.DayFilter != null)
                Matches = Matches!
                .Where(m => m.Date.GetValueOrDefault().Add(ClientUtcDiff).Day == FilterParameters.DayFilter)
                .ToList();
        }

        protected async Task FilterMatchesAsync()
        {
            try
            {
                var result = await MatchService!.SearchForMatchAsync(FilterParameters!.YearFilter, FilterParameters!.MonthFilter, FilterParameters!.DayFilter
                    , FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter);

                // in case the matches based on client's date extends to the next or previous month or day
                if (FilterParameters?.MonthFilter != null)
                {
                    // client's time is behind of UTC time --> load matches for the next month too
                    if (ClientUtcDiff < TimeSpan.Zero)
                    {
                        int? nextMonth = FilterParameters.MonthFilter == 12 ? 1 : FilterParameters.MonthFilter + 1;

                        result = result.Concat(await MatchService!.SearchForMatchAsync(FilterParameters!.YearFilter, nextMonth, FilterParameters!.DayFilter
                            , FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter));
                    }
                    // client's date is ahead UTC time --> load matches for the previous month too
                    else if (ClientUtcDiff > TimeSpan.Zero)
                    {
                        int? previousMonth = FilterParameters.MonthFilter == 1 ? 12 : FilterParameters.MonthFilter - 1;

                        result = result.Concat(await MatchService!.SearchForMatchAsync(FilterParameters!.YearFilter, previousMonth, FilterParameters!.DayFilter 
                            , FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter));
                    }           
                }

                if (FilterParameters?.DayFilter != null)
                {
                    // client's time is behind of UTC time --> load matches for the next day too
                    if (ClientUtcDiff < TimeSpan.Zero)
                    {
                        if (FilterParameters.DayFilter == 30)
                        {
                            result = result.Concat(await MatchService!.SearchForMatchAsync(FilterParameters!.YearFilter, FilterParameters!.MonthFilter, 31
                            , FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter));
                            result = result.Concat(await MatchService!.SearchForMatchAsync(FilterParameters!.YearFilter, FilterParameters!.MonthFilter, 1
                            , FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter));
                        }
                        else if (FilterParameters.DayFilter == 31)
                        {
                            result = result.Concat(await MatchService!.SearchForMatchAsync(FilterParameters!.YearFilter, FilterParameters!.MonthFilter, 1
                            , FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter));
                        }
                        else
                        {
                            result = result.Concat(await MatchService!.SearchForMatchAsync(FilterParameters!.YearFilter, FilterParameters!.MonthFilter, FilterParameters!.DayFilter + 1
                            , FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter));
                        }                   
                    }
                    // client's date is ahead of UTC time --> load matches for the previous day too
                    else if (ClientUtcDiff > TimeSpan.Zero)
                    {
                        if (FilterParameters.DayFilter != 1)
                        {
                            result = result.Concat(await MatchService!.SearchForMatchAsync(FilterParameters!.YearFilter, FilterParameters!.MonthFilter, FilterParameters!.DayFilter - 1
                           , FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter));
                        }
                        else
                        {
                            result = result.Concat(await MatchService!.SearchForMatchAsync(FilterParameters!.YearFilter, FilterParameters!.MonthFilter, 31
                           , FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter));

                            result = result.Concat(await MatchService!.SearchForMatchAsync(FilterParameters!.YearFilter, FilterParameters!.MonthFilter, 30
                           , FilterParameters!.TeamFilter, FilterParameters!.LeagueFilter, FilterParameters!.SeasonFilter, FilterParameters!.RoundFilter));
                        }
                       
                    }
                }


                if (!string.IsNullOrEmpty(FilterParameters?.OpponentNameFilter))
                    result = result.Where(m => m.HomeTeam.Name.ToLower().Equals(FilterParameters.OpponentNameFilter.ToLower()) || m.AwayTeam.Name.ToLower().Equals(FilterParameters.OpponentNameFilter.ToLower()));
                if (FilterParameters?.HomeAwayFilter == "Home")
                    result = result.Where(m => m.HomeTeam.Name == FilterParameters.TeamFilter);
                else if (FilterParameters?.HomeAwayFilter == "Away")
                    result = result.Where(m => m.AwayTeam.Name == FilterParameters.TeamFilter);

                Matches = result.ToList();
                FilterMatchesBasedOnClientDate();

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
