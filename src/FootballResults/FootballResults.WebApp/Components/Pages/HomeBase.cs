using FootballResults.WebApp.Services;
using Microsoft.AspNetCore.Components;
using FootballResults.Models;


namespace FootballResults.Components.Pages
{
    public partial class HomeBase : ComponentBase
    {
        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [Inject]
        protected IMatchService? MatchService { get; set; }

        protected IEnumerable<Match>? Matches { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadMatchesAsync();
        }

        protected async Task LoadMatchesAsync()
        {
            try
            {
                Matches = await MatchService!.GetMatchesForToday();
            }
            catch (HttpRequestException)
            {
                NavigationManager?.NavigateTo("/Error", true);
            }   
        }
    }
}
