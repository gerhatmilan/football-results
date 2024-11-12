using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.WebApp.Components.Utilities;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.LiveUpdates;
using FootballResults.DataAccess.Entities.Users;

namespace FootballResults.WebApp.Components.Pages.Details
{
    public class MatchDetailsBase : MatchFilterablePageBase
    {
        [CascadingParameter(Name = "User")]
        public User User { get; set; } = default!;

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
            Match = await MatchService!.GetMatchByIDAsync(int.Parse(MatchID!));
                
            if (Match == null)
            {
                NavigationManager.NavigateTo("/404", true);
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

        protected override async void OnUpdateMessageReceivedAsync(object? sender, UpdateMessageType notificationType)
        {
            base.OnUpdateMessageReceivedAsync(sender, notificationType);
            await LoadMatchAsync();
        }
    }
}
