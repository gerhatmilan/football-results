using FootballResults.Models;
using System.Collections;

namespace FootballResults.API.Models
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetCountries();
        Task<IEnumerable<Country>> GetLeaguesByCountry();
        Task<IEnumerable<Country>> GetTeamsByCountry();
        Task<IEnumerable<Country>> GetVenuesByCountry();
        Task<Country?> GetCountryByName(string countryName);
        Task<IEnumerable<League>> GetLeaguesInCountry(string countryName);
        Task<IEnumerable<Team>> GetTeamsInCountry(string countryName);
        Task<IEnumerable<Venue>> GetVenuesInCountry(string countryName);
        Task<IEnumerable<Country>> Search(string? countryName);
    }
}