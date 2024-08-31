using FootballResults.DataAccess.Entities.Football;

namespace FootballResults.DataAccess.Repositories.Football
{
    public interface IMatchRepository : IGenericRepository<Match>
    {
        Task<IEnumerable<Match>> GetHeadToHead(string teamName1, string teamName2);
        Task<IEnumerable<Match>> Search(DateTime? date, int? year, int? month, int? day, string teamName, string leagueName, int? season, string round);
    }
}
