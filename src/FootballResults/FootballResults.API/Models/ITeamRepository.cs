using FootballResults.Models;

namespace FootballResults.API.Models
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetTeams();
        Task<Team?> GetTeamByName(string teamName);
        Task<IEnumerable<Player>> GetSquadForTeam(string teamName);
        Task<IEnumerable<Match>> GetMatchesForTeamAndLeagueAndSeason(string teamName, string leagueName, int season);
        Task<IEnumerable<Team>> Search(string? teamName, string? country, bool? national);
    }
}