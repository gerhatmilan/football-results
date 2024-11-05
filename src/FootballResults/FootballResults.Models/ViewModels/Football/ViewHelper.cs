using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;

namespace FootballResults.Models.ViewModels.Football
{
    public static class ViewHelper
    {
        public static IEnumerable<League> GetFavoriteLeaguesFirst(IEnumerable<League> leagues, User user = null)
        {
            if (user == null)
                return leagues.OrderBy(l => l.Name);

            return leagues.OrderByDescending(l => user.FavoriteLeagues.Select(fl => fl.ID).Contains(l.ID)).ThenBy(l => l.Name);
        }

        public static IEnumerable<Team> GetFavoriteTeamsFirst(IEnumerable<Team> teams, User user = null)
        {
            if (user == null)
                return teams.OrderBy(t => t.Name);

            return teams.OrderByDescending(t => user.FavoriteTeams.Select(ft => ft.ID).Contains(t.ID)).ThenBy(t => t.Name);
        }

        public static IEnumerable<LeagueMatchGroup> GetMatchesGroupedByLeague(IEnumerable<Match> matches, User user = null)
        {
            IEnumerable<LeagueMatchGroup> retval = matches
                .GroupBy(m => m.League.ID)
                .Select(group => new LeagueMatchGroup
                {
                    League = group.First().League,
                    Matches = group.OrderBy(m => m.Date).ToList()
                })
                .OrderBy(group => group.League.Country.Name) // order by country name of the league
                .ThenBy(group => group.League.Name); // then by league name

            if (user != null)
            {
                retval = retval
                    .OrderByDescending(pair => user.FavoriteLeagues.Select(fl => fl.ID).Contains(pair.League.ID)); // favorite leagues first
            }

            return retval;
        }
    }
}
