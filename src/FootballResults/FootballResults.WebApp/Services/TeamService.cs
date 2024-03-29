using FootballResults.Models;
using FootballResults.WebApp.Models;

namespace FootballResults.WebApp.Services
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

        public async Task<IEnumerable<Team>> GetFavoriteTeamsAsync(User? user)
        {
            var teams = await _httpClient.GetFromJsonAsync<IEnumerable<Team>>("api/teams");

            if (teams != null && user?.FavoriteTeams != null)
            {
                var userFavorites = user.FavoriteTeams.Select(ft => ft.TeamID);
                return teams.Where(t => userFavorites.Contains(t.TeamID));
            }
            else
            {
                return Enumerable.Empty<Team>();
            }
        }
    }
}
