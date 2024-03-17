using FootballResults.Models;

namespace FootballResults.WebApp.Services
{
    public class MatchService : IMatchService
    {
        private readonly HttpClient _httpClient;

        public MatchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Match?> GetMatchByID(int id)
        {
            return await _httpClient.GetFromJsonAsync<Match>($"api/matches/{id}");
        }
        public async Task<IEnumerable<Match>> GetHeadToHead(string teamName1, string teamName2)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Match>>($"api/matches/head2head?team1={teamName1}&team2={teamName2}");

            return result ?? Enumerable.Empty<Match>();
        }


        public async Task<IEnumerable<Match>> GetMatchesForToday()
        {
            return await GetMatchesForDate(DateTime.Now);
        }

        public async Task<IEnumerable<Match>> GetMatchesForDate(DateTime date)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Match>>($"api/matches/search?date={date.Date}");
            return result ?? Enumerable.Empty<Match>();
        }

        public async Task<IEnumerable<Match>> GetMatchesForLeagueAndDate(string leagueName, DateTime date)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Match>>($"api/matches/search?league={leagueName}&date={date.Date}");
            return result ?? Enumerable.Empty<Match>();
        }

        public async Task<IEnumerable<Match>> GetMatchesForLeagueAndSeason(string leagueName, int season)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Match>>($"api/matches/search?league={leagueName}&season={season}");
            return result ?? Enumerable.Empty<Match>();
        }

        public async Task<IEnumerable<Match>> GetMatchesForTeamAndSeason(string teamName, int season)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Match>>($"api/matches/search?team={teamName}&season={season}");
            return result ?? Enumerable.Empty<Match>();
        }
    
        public async Task<IEnumerable<Match>> SearchForMatch(int? year, int? month, int? day, string? teamName, string? leagueName, int? season, string? round)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Match>>($"api/matches/search?year={year}&month={month}&day={day}&team={teamName}&league={leagueName}&season={season}&round={round}");
            return result ?? Enumerable.Empty<Match>();
        }
    }
}
