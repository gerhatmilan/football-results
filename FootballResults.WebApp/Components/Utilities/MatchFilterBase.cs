using Microsoft.AspNetCore.Components;
using FootballResults.DataAccess.Entities.Football;
using System.Linq;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Time;

namespace FootballResults.WebApp.Components.Utilities
{
    public class MatchFilterBase : ComponentBase
    {
        [Parameter]
        public EventCallback<IEnumerable<Match>> FilterSubmitted { get; set; }

        [Parameter]
        public EventCallback<MatchOrderOption> MatchOrderChanged { get; set; }

        [Parameter]
        public MatchFilterParameters? FilterParameters { get; set; }

        [Parameter]
        public MatchFilterablePageBase? FilterTargetPage { get; set; }

        protected async Task OnMatchFilterSubmitted()
        {
            await FilterSubmitted.InvokeAsync();
        }

        protected async Task OnMatchOrderChanged(MatchOrderOption newOrderOption)
        {
            await MatchOrderChanged.InvokeAsync(newOrderOption);
        }
    }
}
