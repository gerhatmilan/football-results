using FootballResults.Models;

namespace FootballResults.WebApp.Components.Utilities
{
    public interface IMatchFilterable
    {
        IEnumerable<Match>? Matches { get; set; }
    }
}
