using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;

namespace FootballResults.Models.ViewModels.Football
{
    public class LeagueMatchGroup
    {
        public League League { get; set; }
        public IEnumerable<Match> Matches { get; set; }
    }
}
