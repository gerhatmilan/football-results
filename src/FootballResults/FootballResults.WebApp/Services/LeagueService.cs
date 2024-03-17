using FootballResults.Models;

namespace FootballResults.WebApp.Services
{
    public class LeagueService : ILeagueService
    {
        private readonly HttpClient _httpClient;

        public LeagueService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<League?> GetLeagueByName(string leagueName)
        {
            return await _httpClient.GetFromJsonAsync<League>($"api/leagues/{leagueName}");
        }

        public async Task<IEnumerable<League>> GetLeagues()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<League>>($"api/leagues");

            return result ?? Enumerable.Empty<League>();
        }

        public async Task<IEnumerable<Country>> GetCountriesWithLeagues()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Country>>("api/countries/leagues");

            return result ?? Enumerable.Empty<Country>();
        }
    }
}