using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.WebApp.Components.Utilities;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.LiveUpdates;

namespace FootballResults.WebApp.Components.Pages.Details
{
    public class MatchDetailsBase : MatchFilterablePageBase
    {
        [Parameter]
        public string? MatchID { get; set; }
        protected Match? Match { get; set; }

        protected override MatchOrderOption MatchOrderOption { get; set; } = MatchOrderOption.DateDesc;

        protected override async Task OnInitializedAsync()
        {
            await LoadMatchAsync();
            await base.OnInitializedAsync();
        }

        protected async Task LoadMatchAsync()
        {
            try
            {
                Match = await MatchService!.GetMatchByIDAsync(int.Parse(MatchID!));
                
                if (Match == null)
                {
                    NavigationManager.NavigateTo("/404", true);
                }
            }
            catch (Exception)
            {
                NavigationManager?.NavigateTo("/error", true);
            }
        }

        protected override void InitializeMatchFilters()
        {
            MatchFilterParameters = new MatchFilterParameters()
            {
                TeamFilter = Match!.HomeTeam.Name,
                OpponentNameFilter = Match!.AwayTeam.Name,
                SeasonFilter = DateTime.Now.ToLocalTime().Month >= 8 ? DateTime.Now.ToLocalTime().Year : DateTime.Now.ToLocalTime().Year - 1
            };
        }

        protected List<(int leagueID, List<Match> matches)> GetMatchesByLeague()
        {
            return Matches!
                .GroupBy(
                    m => m.League.ID,
                    (leagueID, matches) => (leagueID, Matches!.Where(m => m.League.ID.Equals(leagueID)).ToList())
                )
                .ToList();
        }


        protected override async void OnUpdateMessageReceivedAsync(object? sender, UpdateMessageType notificationType)
        {
            base.OnUpdateMessageReceivedAsync(sender, notificationType);
            await LoadMatchAsync();
        }
    }
}
