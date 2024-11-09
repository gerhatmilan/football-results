using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;

namespace FootballResults.WebApp.Services.Football.Client
{
    public class MatchServiceClient : IMatchService
    {
        private readonly HttpClient _httpClient;

        public MatchServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Match?> GetMatchByIDAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Match>($"api/matches/{id}");
        }
        public async Task<IEnumerable<Match>> GetHeadToHeadAsync(string teamName1, string teamName2)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Match>>($"api/matches/head2head?team1={teamName1}&team2={teamName2}");

            return result ?? Enumerable.Empty<Match>();
        }

        public async Task<IEnumerable<Match>> GetMatchesForDateAsync(DateTime date)
        {
            var result = await SearchForMatchAsync(date: date);
            return result ?? Enumerable.Empty<Match>();
        }

        public async Task<IEnumerable<Match>> GetMatchesForIntervalAsync(DateTime from, DateTime to)
        {
            var result = await SearchForMatchAsync(from: from, to: to);
            return result ?? Enumerable.Empty<Match>();
        }


        public async Task<IEnumerable<Match>> GetMatchesForYearAsync(int year)
        {
            var result = await SearchForMatchAsync(year: year);
            return result ?? Enumerable.Empty<Match>();
        }

        public async Task<IEnumerable<Match>> GetMatchesForLeagueAndDateAsync(string leagueName, DateTime date)
        {
            var result = await SearchForMatchAsync(leagueName: leagueName, date: date);
            return result ?? Enumerable.Empty<Match>();
        }

        public async Task<IEnumerable<Match>> GetMatchesForLeagueAndSeasonAsync(string leagueName, int season)
        {
            var result = await SearchForMatchAsync(leagueName: leagueName, season: season);
            return result ?? Enumerable.Empty<Match>();
        }

        public async Task<IEnumerable<Match>> GetMatchesForTeamAndSeasonAsync(string teamName, int season)
        {
            var result = await SearchForMatchAsync(teamName: teamName, season: season);
            return result ?? Enumerable.Empty<Match>();
        }

        public async Task<IEnumerable<Match>> SearchForMatchAsync(DateTime? date = null, DateTime? from = null, DateTime? to = null,
            int? year = null, int? month = null, int? day = null, string? teamName = null, string? leagueName = null, int? season = null, string? round = null)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Match>>($"api/matches/search?date={date}&from={from}&to={to}&year={year}&month={month}&day={day}&team={teamName}&league={leagueName}&season={season}&round={round}");
            return result ?? Enumerable.Empty<Match>();
        }
    }
}