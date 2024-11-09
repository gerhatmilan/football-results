using FootballResults.DataAccess.Entities.Football;

namespace FootballResults.DataAccess.Repositories.Football
{
    public interface ITeamRepository : IGenericRepository<Team>
    {
        Task<Team> GetTeamByName(string teamName);
        Task<IEnumerable<Player>> GetSquadForTeam(string teamName);
        Task<IEnumerable<Match>> GetMatchesForTeamAndLeagueAndSeason(string teamName, string leagueName, int season);
        Task<IEnumerable<Team>> Search(string teamName, string country, bool? national);
    }
}
