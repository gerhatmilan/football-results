using FootballResults.DataAccess.Entities.Football;

namespace FootballResults.DataAccess.Repositories.Football
{
    public interface IMatchRepository : IGenericRepository<Match>
    {
        Task<IEnumerable<Match>> GetHeadToHead(string teamName1, string teamName2, bool tracking = true);
        Task<IEnumerable<Match>> Search(DateTime? date, DateTime? from, DateTime? to, int? year, int? month, int? day, string teamName, string leagueName, int? season, string round, bool tracking = true);
    }
}
