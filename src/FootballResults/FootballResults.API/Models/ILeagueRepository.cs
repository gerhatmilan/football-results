using FootballResults.Models.Football;

namespace FootballResults.API.Models
{
    public interface ILeagueRepository
    {
        Task<IEnumerable<League>> GetLeagues();
        Task<League?> GetLeagueByName(string name);
        Task<IEnumerable<int>> GetSeasonsForLeague(string leagueName);
        Task<IEnumerable<Team>> GetTeamsForLeague(string leagueName, int? season);
        Task<IEnumerable<string>> GetRoundsForLeagueAndSeason(string leagueName, int season);
        Task<IEnumerable<Match>> GetMatchesForLeague(string leagueName, int? season, string? round);
        Task<IEnumerable<Standing>> GetStandingsForLeagueAndSeason(string leagueName, int season);
        Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeason(string leagueName, int season);
        Task<IEnumerable<League>> Search(string? leagueName, string? country, string? type, int? currentSeason);
    }
}