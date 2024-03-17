using FootballResults.Models;

namespace FootballResults.WebApp.Components.Other
{
    public interface IMatchFilterable
    {
        IEnumerable<Match>? Matches { get; set; }
    }
}
