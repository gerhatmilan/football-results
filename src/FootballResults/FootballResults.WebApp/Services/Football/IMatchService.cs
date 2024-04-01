using FootballResults.Models.Football;

namespace FootballResults.WebApp.Services.Football
{
    public interface IMatchService
    {
        Task<Match?> GetMatchByIDAsync(int id);
        Task<IEnumerable<Match>> GetHeadToHeadAsync(string teamName1, string teamName2);
        Task<IEnumerable<Match>> GetMatchesForDateAsync(DateTime date);
        Task<IEnumerable<Match>> GetMatchesForYearAsync(int year);
        Task<IEnumerable<Match>> GetMatchesForLeagueAndDateAsync(string leagueName, DateTime date);
        Task<IEnumerable<Match>> GetMatchesForTeamAndSeasonAsync(string teamName, int season);
        Task<IEnumerable<Match>> GetMatchesForLeagueAndSeasonAsync(string leagueName, int season);
        Task<IEnumerable<Match>> SearchForMatchAsync(int? year, int? month, int? day, string? teamName, string? leagueName, int? season, string? round);
    }
}
