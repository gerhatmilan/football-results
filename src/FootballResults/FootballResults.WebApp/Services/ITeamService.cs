using FootballResults.Models;

namespace FootballResults.WebApp.Services
{
    public interface ITeamService
    {
        Task<Team?> GetTeamByName(string teamName);
        Task<IEnumerable<Player>> GetSquadForTeam(string teamName);
    }
}
