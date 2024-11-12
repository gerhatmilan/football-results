using FootballResults.DataAccess.Entities.Football;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.DataAccess.Repositories.Football
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        Task<IEnumerable<Country>> GetLeaguesByCountry(bool tracking = true);
        Task<IEnumerable<Country>> GetTeamsByCountry(bool tracking = true);
        Task<IEnumerable<Country>> GetVenuesByCountry(bool tracking = true);
        Task<Country> GetCountryByName(string countryName, bool tracking = true);
        Task<IEnumerable<League>> GetLeaguesForCountry(string countryName, bool tracking = true);
        Task<IEnumerable<Team>> GetTeamsForCountry(string countryName, bool tracking = true);
        Task<IEnumerable<Venue>> GetVenuesForCountry(string countryName, bool tracking = true);
        Task<IEnumerable<Country>> Search(string countryName, bool tracking = true);
    }
}
