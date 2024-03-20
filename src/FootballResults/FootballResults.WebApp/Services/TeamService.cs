using FootballResults.Models;

namespace FootballResults.WebApp.Services
{
    public class TeamService : ITeamService
    {
        private readonly HttpClient _httpClient;

        public TeamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Team?> GetTeamByName(string teamName)
        {
            return await _httpClient.GetFromJsonAsync<Team>($"api/teams/{teamName}");
        }

        public async Task<IEnumerable<Player>> GetSquadForTeam(string teamName)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Player>>($"api/teams/{teamName}/squad");
            return result ?? Enumerable.Empty<Player>();
        }
    }
}
