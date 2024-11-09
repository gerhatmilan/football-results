using FootballResults.DataAccess.Entities.Football;

namespace FootballResults.DataAccess.Repositories.Football
{
    public interface ILeagueRepository : IGenericRepository<League>
    {
        Task<League> GetLeagueByName(string name);
        Task<IEnumerable<int>> GetSeasonsForLeague(string leagueName);
        Task<IEnumerable<Team>> GetTeamsForLeague(string leagueName, int? season);
        Task<IEnumerable<string>> GetRoundsForLeagueAndSeason(string leagueName, int season);
        Task<IEnumerable<Match>> GetMatchesForLeague(string leagueName, int? season, string round);
        Task<IEnumerable<LeagueStanding>> GetStandingsForLeagueAndSeason(string leagueName, int season);
        Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeason(string leagueName, int season);
        Task<IEnumerable<League>> Search(string leagueName, string country, string type, int? currentSeason);
    }
}
