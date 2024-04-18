using FootballResults.Models.Football;
using FootballResults.Models.Users;

namespace FootballResults.WebApp.Services.Football
{
    public class TeamService : ITeamService
    {
        private readonly HttpClient _httpClient;

        public TeamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Team?> GetTeamByNameAsync(string teamName)
        {
            return await _httpClient.GetFromJsonAsync<Team>($"api/teams/{teamName}");
        }

        public async Task<IEnumerable<Country>> GetCountriesWithTeamsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Country>>("api/countries/teams");
            return result ?? Enumerable.Empty<Country>();
        }


        public async Task<IEnumerable<Player>> GetSquadForTeamAsync(string teamName)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Player>>($"api/teams/{teamName}/squad");
            return result ?? Enumerable.Empty<Player>();
        }

        public async Task<IEnumerable<Team>> SearchAsync(string teamName)
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<Team>>($"api/teams/search?name={teamName}");
            return result ?? Enumerable.Empty<Team>();
        }

        public IEnumerable<Team> GetTeamsFavoritesFirst(User user, IEnumerable<Team> teams)
        {
            return teams.OrderByDescending(t => user.FavoriteTeams.Select(ft => ft.TeamID).Contains(t.TeamID)).ThenBy(t => t.Name);
        }
    }
}
