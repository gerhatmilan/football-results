using FootballResults.Models.Football;

namespace FootballResults.API.Models
{
    public interface IMatchRepository
    {
        Task<Match?> GetMatchByID(int id);
        Task<IEnumerable<Match>> GetHeadToHead(string teamName1, string teamName2);
        Task<IEnumerable<Match>> Search(DateTime? date, int? year, int? month, int? day, string? teamName, string? leagueName, int? season, string? round);
    }
}