using FootballResults.DataAccess.Entities.Football;
using FootballResults.WebApp.Components.Pages;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.LiveUpdates;
using FootballResults.WebApp.Services.Time;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Utilities
{
    public abstract class MatchFilterablePageBase : LiveUpdatePageBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected IMatchService? MatchService { get; set; } = default!;

        [Inject]
        protected IClientTimeService? ClientTimeService { get; set; } = default!;

        protected IEnumerable<Match>? Matches { get; set; }

        protected MatchFilterParameters? MatchFilterParameters { get; set; }
        protected abstract MatchOrderOption MatchOrderOption { get; set; }
        protected TimeSpan ClientUtcDiff { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            ClientUtcDiff = await ClientTimeService!.GetClientUtcDiffAsync();

            InitializeMatchFilters();
            await LoadMatchesAsync();
        }

        protected abstract void InitializeMatchFilters();

        protected virtual async Task LoadMatchesAsync()
        {
            try
            {
                var result = await MatchService!.SearchForMatchAsync(year: MatchFilterParameters!.YearFilter, month: MatchFilterParameters!.MonthFilter, day: MatchFilterParameters!.DayFilter
                    , teamName: MatchFilterParameters!.TeamFilter, leagueName: MatchFilterParameters!.LeagueFilter, season: MatchFilterParameters!.SeasonFilter, round: MatchFilterParameters!.RoundFilter);

                // in case the matches based on client's date extends to the next or previous month or day
                if (MatchFilterParameters?.MonthFilter != null)
                {
                    // client's time is behind of UTC time --> load matches for the next month too
                    if (ClientUtcDiff < TimeSpan.Zero)
                    {
                        int? nextMonth = MatchFilterParameters.MonthFilter == 12 ? 1 : MatchFilterParameters.MonthFilter + 1;

                        result = result.Concat(await MatchService!.SearchForMatchAsync(year: MatchFilterParameters!.YearFilter, month: nextMonth, day: MatchFilterParameters!.DayFilter
                            , teamName: MatchFilterParameters!.TeamFilter, leagueName: MatchFilterParameters!.LeagueFilter, season: MatchFilterParameters!.SeasonFilter, round: MatchFilterParameters!.RoundFilter));
                    }
                    // client's date is ahead UTC time --> load matches for the previous month too
                    else if (ClientUtcDiff > TimeSpan.Zero)
                    {
                        int? previousMonth = MatchFilterParameters.MonthFilter == 1 ? 12 : MatchFilterParameters.MonthFilter - 1;

                        result = result.Concat(await MatchService!.SearchForMatchAsync(year: MatchFilterParameters!.YearFilter, month: previousMonth, day: MatchFilterParameters!.DayFilter
                            , teamName: MatchFilterParameters!.TeamFilter, leagueName: MatchFilterParameters!.LeagueFilter, season: MatchFilterParameters!.SeasonFilter, round: MatchFilterParameters!.RoundFilter));
                    }
                }

                if (MatchFilterParameters?.DayFilter != null)
                {
                    // client's time is behind of UTC time --> load matches for the next day too
                    if (ClientUtcDiff < TimeSpan.Zero)
                    {
                        if (MatchFilterParameters.DayFilter == 30)
                        {
                            result = result.Concat(await MatchService!.SearchForMatchAsync(year: MatchFilterParameters!.YearFilter, month: MatchFilterParameters!.MonthFilter, day: 31
                            , teamName: MatchFilterParameters!.TeamFilter, leagueName: MatchFilterParameters!.LeagueFilter, season: MatchFilterParameters!.SeasonFilter, round: MatchFilterParameters!.RoundFilter));
                            result = result.Concat(await MatchService!.SearchForMatchAsync(year: MatchFilterParameters!.YearFilter, month: MatchFilterParameters!.MonthFilter, day: 1
                            , teamName: MatchFilterParameters!.TeamFilter, leagueName: MatchFilterParameters!.LeagueFilter, season: MatchFilterParameters!.SeasonFilter, round: MatchFilterParameters!.RoundFilter));
                        }
                        else if (MatchFilterParameters.DayFilter == 31)
                        {
                            result = result.Concat(await MatchService!.SearchForMatchAsync(year: MatchFilterParameters!.YearFilter, month: MatchFilterParameters!.MonthFilter, day: 1
                            , teamName: MatchFilterParameters!.TeamFilter, leagueName: MatchFilterParameters!.LeagueFilter, season: MatchFilterParameters!.SeasonFilter, round: MatchFilterParameters!.RoundFilter));
                        }
                        else
                        {
                            result = result.Concat(await MatchService!.SearchForMatchAsync(year: MatchFilterParameters!.YearFilter, month: MatchFilterParameters!.MonthFilter, day: MatchFilterParameters!.DayFilter + 1
                            , teamName: MatchFilterParameters!.TeamFilter, leagueName: MatchFilterParameters!.LeagueFilter, season: MatchFilterParameters!.SeasonFilter, round: MatchFilterParameters!.RoundFilter));
                        }
                    }
                    // client's date is ahead of UTC time --> load matches for the previous day too
                    else if (ClientUtcDiff > TimeSpan.Zero)
                    {
                        if (MatchFilterParameters.DayFilter != 1)
                        {
                            result = result.Concat(await MatchService!.SearchForMatchAsync(year: MatchFilterParameters!.YearFilter, month: MatchFilterParameters!.MonthFilter, day: MatchFilterParameters!.DayFilter - 1
                           , teamName: MatchFilterParameters!.TeamFilter, leagueName: MatchFilterParameters!.LeagueFilter, season: MatchFilterParameters!.SeasonFilter, round: MatchFilterParameters!.RoundFilter));
                        }
                        else
                        {
                            result = result.Concat(await MatchService!.SearchForMatchAsync(year: MatchFilterParameters!.YearFilter, month: MatchFilterParameters!.MonthFilter, day: 31
                           , teamName: MatchFilterParameters!.TeamFilter, leagueName: MatchFilterParameters!.LeagueFilter, season: MatchFilterParameters!.SeasonFilter, round: MatchFilterParameters!.RoundFilter));

                            result = result.Concat(await MatchService!.SearchForMatchAsync(year: MatchFilterParameters!.YearFilter, month: MatchFilterParameters!.MonthFilter, day: 30
                           , teamName: MatchFilterParameters!.TeamFilter, leagueName: MatchFilterParameters!.LeagueFilter, season: MatchFilterParameters!.SeasonFilter, round: MatchFilterParameters!.RoundFilter));
                        }

                    }
                }


                if (!string.IsNullOrEmpty(MatchFilterParameters?.OpponentNameFilter))
                    result = result.Where(m => m.HomeTeam.Name.ToLower().Equals(MatchFilterParameters.OpponentNameFilter.ToLower()) || m.AwayTeam.Name.ToLower().Equals(MatchFilterParameters.OpponentNameFilter.ToLower()));
                if (MatchFilterParameters?.HomeAwayFilter == "Home")
                    result = result.Where(m => m.HomeTeam.Name == MatchFilterParameters.TeamFilter);
                else if (MatchFilterParameters?.HomeAwayFilter == "Away")
                    result = result.Where(m => m.AwayTeam.Name == MatchFilterParameters.TeamFilter);

                Matches = result;
                FilterMatchesBasedOnClientDate();
            }
            catch (Exception)
            {
                NavigationManager!.NavigateTo("/error", true);
            }
        }

        protected void FilterMatchesBasedOnClientDate()
        {
            if (MatchFilterParameters?.MonthFilter != null)
                Matches = Matches!
                .Where(m => m.Date.GetValueOrDefault().Add(ClientUtcDiff).Month == MatchFilterParameters.MonthFilter)
                .ToList();

            if (MatchFilterParameters?.DayFilter != null)
                Matches = Matches!
                .Where(m => m.Date.GetValueOrDefault().Add(ClientUtcDiff).Day == MatchFilterParameters.DayFilter)
                .ToList();
        }

        protected async Task OnMatchFilterSubmitted(IEnumerable<Match> matches)
        {
            await LoadMatchesAsync();
        }

        protected void OnMatchOrderChanged(MatchOrderOption newOrderOption)
        {
            if (newOrderOption != MatchOrderOption)
            {
                MatchOrderOption = newOrderOption;
                StateHasChanged();
            }
        }

        protected override async void OnUpdateMessageReceivedAsync(object? sender, UpdateMessageType notificationType)
        {
            if (notificationType == UpdateMessageType.MatchesUpdated)
            {
                await LoadMatchesAsync();
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}
