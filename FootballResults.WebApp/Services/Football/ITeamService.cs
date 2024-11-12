using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;

namespace FootballResults.WebApp.Services.Football
{
    public interface ITeamService
    {
        Task<Team?> GetTeamByNameAsync(string teamName, bool tracking = true);
        Task<IEnumerable<Country>> GetCountriesWithTeamsAsync(bool tracking = true);
        Task<IEnumerable<Player>> GetSquadForTeamAsync(string teamName, bool tracking = true);
        Task<IEnumerable<Team>> SearchAsync(string teamName, bool tracking = true);
    }
}
