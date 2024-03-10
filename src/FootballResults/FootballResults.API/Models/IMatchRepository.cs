using FootballResults.Models;

namespace FootballResults.API.Models
{
    public interface IMatchRepository
    {
        Task<Match?> GetMatchByID(int id);
        Task<IEnumerable<Match>> GetHeadToHead(string teamName1, string teamName2);
        Task<IEnumerable<Match>> Search(DateTime? date, string? teamName, string? leagueName, int? season, string? round);
    }
}