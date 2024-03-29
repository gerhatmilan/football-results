using FootballResults.Models;
using FootballResults.WebApp.Models;

namespace FootballResults.WebApp.Services
{
    public interface ITeamService
    {
        Task<Team?> GetTeamByNameAsync(string teamName);
        Task<IEnumerable<Country>> GetCountriesWithTeamsAsync();
        Task<IEnumerable<Player>> GetSquadForTeamAsync(string teamName);
        Task<IEnumerable<Team>> SearchAsync(string teamName);
        Task<IEnumerable<Team>> GetFavoriteTeamsAsync(User? user);

    }
}
