using FootballResults.Models;

namespace FootballResults.WebApp.Services
{
    public interface ILeagueService
    {
        Task<League?> GetLeagueByName(string leagueName);

        Task<IEnumerable<League>> GetLeagues();
        Task<IEnumerable<Country>> GetCountriesWithLeagues();
        Task<IEnumerable<Standing>> GetStandingsForLeagueAndSeason(string leagueName, int season);
        Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeason(string leagueName, int season);
    }
}
