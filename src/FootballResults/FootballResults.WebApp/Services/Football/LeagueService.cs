using FootballResults.Models.Football;

namespace FootballResults.WebApp.Services.Football
{
    public class LeagueService : ILeagueService
    {
        private readonly HttpClient _httpClient;

        public LeagueService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<League?> GetLeagueByNameAsync(string leagueName)
        {
            return await _httpClient.GetFromJsonAsync<League>($"api/leagues/{leagueName}");
        }

        public async Task<IEnumerable<League>> GetLeaguesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<League>>($"api/leagues");

            return result ?? Enumerable.Empty<League>();
        }

        public async Task<IEnumerable<Country>> GetCountriesWithLeaguesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Country>>("api/countries/leagues");

            return result ?? Enumerable.Empty<Country>();
        }

        public async Task<IEnumerable<Standing>> GetStandingsForLeagueAndSeasonAsync(string leagueName, int season)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Standing>>($"api/leagues/{leagueName}/standings?season={season}");

            return result ?? Enumerable.Empty<Standing>();
        }

        public async Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeasonAsync(string leagueName, int season)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<TopScorer>>($"api/leagues/{leagueName}/topscorers?season={season}");

            return result ?? Enumerable.Empty<TopScorer>();
        }

        public async Task<IEnumerable<League>> SearchAsync(string leagueName)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<League>>($"api/leagues/search?name={leagueName}");
            return result ?? Enumerable.Empty<League>();
        }
    }
}