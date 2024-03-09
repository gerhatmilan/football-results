using FootballResults.Models;

namespace FootballResults.API.Models
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetTeams();
        Task<Team?> GetTeamByName(string teamName);
        Task<IEnumerable<Team>> Search(string? teamName, string? country, bool? national);
    }
}