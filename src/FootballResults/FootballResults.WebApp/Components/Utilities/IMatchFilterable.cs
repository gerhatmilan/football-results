using FootballResults.Models.Football;

namespace FootballResults.WebApp.Components.Utilities
{
    public interface IMatchFilterable
    {
        IEnumerable<Match>? Matches { get; set; }
    }
}
