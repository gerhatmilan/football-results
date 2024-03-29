using FootballResults.Models;
using FootballResults.WebApp.Models;

namespace FootballResults.WebApp.Services
{
    public interface ILeagueService
    {
        Task<League?> GetLeagueByNameAsync(string leagueName);

        Task<IEnumerable<League>> GetLeaguesAsync();
        Task<IEnumerable<Country>> GetCountriesWithLeaguesAsync();
        Task<IEnumerable<Standing>> GetStandingsForLeagueAndSeasonAsync(string leagueName, int season);
        Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeasonAsync(string leagueName, int season);
        Task<IEnumerable<League>> SearchAsync(string leagueName);
        Task<IEnumerable<League>> GetFavoriteLeaguesAsync(User? user);
    }
}
