using FootballResults.Models;
using FootballResults.WebApp.Components.Other;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages
{
    public class LeagueDetailsBase : ComponentBase, IMatchFilterable
    {
        public IEnumerable<Match>? Matches { get; set; }
    }
}
