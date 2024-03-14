using FootballResults.Models;

namespace FootballResults.WebApp.Services
{
    public interface ILeagueService
    {
        Task<League?> GetLeagueByName(string leagueName);

        Task<IEnumerable<League>> GetLeagues();
        Task<IEnumerable<Country>> GetCountriesWithLeagues();
    }
}
