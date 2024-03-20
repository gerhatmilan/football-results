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
        Task<IEnumerable<League>> GetLeaguesForCountry(string countryName);
        Task<IEnumerable<Team>> GetTeamsForCountry(string countryName);
        Task<IEnumerable<Venue>> GetVenuesForCountry(string countryName);
        Task<IEnumerable<Country>> Search(string? countryName);
    }
}