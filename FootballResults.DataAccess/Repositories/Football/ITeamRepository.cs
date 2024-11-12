using FootballResults.DataAccess.Entities.Football;

namespace FootballResults.DataAccess.Repositories.Football
{
    public interface ITeamRepository : IGenericRepository<Team>
    {
        Task<Team> GetTeamByName(string teamName, bool tracking = true);
        Task<IEnumerable<Player>> GetSquadForTeam(string teamName, bool tracking = true);
        Task<IEnumerable<Match>> GetMatchesForTeamAndLeagueAndSeason(string teamName, string leagueName, int season, bool tracking = true);
        Task<IEnumerable<Team>> Search(string teamName, string country, bool? national, bool tracking = true);
    }
}
