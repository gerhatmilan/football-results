using FootballResults.DataAccess.Entities.Football;

namespace FootballResults.DataAccess.Repositories.Football
{
    public interface ILeagueRepository : IGenericRepository<League>
    {
        Task<League> GetLeagueByName(string name, bool tracking = true);
        Task<IEnumerable<int>> GetSeasonsForLeague(string leagueName, bool tracking = true);
        Task<IEnumerable<Team>> GetTeamsForLeague(string leagueName, int? season, bool tracking = true);
        Task<IEnumerable<string>> GetRoundsForLeagueAndSeason(string leagueName, int season, bool tracking = true);
        Task<IEnumerable<Match>> GetMatchesForLeague(string leagueName, int? season, string round, bool tracking = true);
        Task<IEnumerable<LeagueStanding>> GetStandingsForLeagueAndSeason(string leagueName, int season, bool tracking = true);
        Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeason(string leagueName, int season, bool tracking = true);
        Task<IEnumerable<League>> Search(string leagueName, string country, string type, int? currentSeason, bool tracking = true);
    }
}
