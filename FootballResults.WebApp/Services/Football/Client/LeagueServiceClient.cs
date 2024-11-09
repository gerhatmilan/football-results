using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using System.Text.Json;

namespace FootballResults.WebApp.Services.Football.Client
{
    public class LeagueServiceClient : ILeagueService
    {
        private readonly HttpClient _httpClient;

        public LeagueServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<T?> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    throw new HttpRequestException(null, null, System.Net.HttpStatusCode.NotFound);
                case System.Net.HttpStatusCode.InternalServerError:
                    throw new HttpRequestException(null, null, System.Net.HttpStatusCode.InternalServerError);
                case System.Net.HttpStatusCode.OK:
                    return await response.Content.ReadFromJsonAsync<T>();
                default:
                    return default;
            }
        }

        public async Task<League?> GetLeagueByNameAsync(string leagueName)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/leagues/{leagueName}");
            return await HandleResponseAsync<League?>(response);
        }

        public async Task<IEnumerable<League>> GetLeaguesAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/leagues");
            return await HandleResponseAsync<IEnumerable<League>>(response) ?? Enumerable.Empty<League>();
        }

        public async Task<IEnumerable<Country>> GetCountriesWithLeaguesAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("api/countries/leagues");
            return await HandleResponseAsync<IEnumerable<Country>>(response) ?? Enumerable.Empty<Country>();
        }

        public async Task<IEnumerable<LeagueStanding>> GetStandingsForLeagueAndSeasonAsync(string leagueName, int season)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/leagues/{leagueName}/standings?season={season}");
            return await HandleResponseAsync<IEnumerable<LeagueStanding>>(response) ?? Enumerable.Empty<LeagueStanding>();
        }

        public async Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeasonAsync(string leagueName, int season)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/leagues/{leagueName}/topscorers?season={season}");
            return await HandleResponseAsync<IEnumerable<TopScorer>>(response) ?? Enumerable.Empty<TopScorer>();
        }

        public async Task<IEnumerable<League>> SearchAsync(string leagueName)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/leagues/search?name={leagueName}");
            return await HandleResponseAsync<IEnumerable<League>>(response) ?? Enumerable.Empty<League>();
        }
    }
}