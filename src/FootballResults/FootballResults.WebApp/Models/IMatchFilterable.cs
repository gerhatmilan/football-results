using FootballResults.Models;

namespace FootballResults.WebApp.Models
{
    public interface IMatchFilterable
    {
        IEnumerable<Match>? Matches { get; set; }
    }
}
