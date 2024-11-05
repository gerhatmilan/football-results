using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.ViewModels.Football;

namespace FootballResults.WebApp.Services.Football
{
    public interface IMatchService
    {
        Task<Match?> GetMatchByIDAsync(int id);
        Task<IEnumerable<Match>> GetHeadToHeadAsync(string teamName1, string teamName2);
        Task<IEnumerable<Match>> GetMatchesForDateAsync(DateTime date);
        Task<IEnumerable<Match>> GetMatchesForIntervalAsync(DateTime from, DateTime to);
        Task<IEnumerable<Match>> GetMatchesForYearAsync(int year);
        Task<IEnumerable<Match>> GetMatchesForLeagueAndDateAsync(string leagueName, DateTime date);
        Task<IEnumerable<Match>> GetMatchesForTeamAndSeasonAsync(string teamName, int season);
        Task<IEnumerable<Match>> GetMatchesForLeagueAndSeasonAsync(string leagueName, int season);
        Task<IEnumerable<Match>> SearchForMatchAsync(DateTime? date = null, DateTime? from = null, DateTime? to = null,
            int? year = null, int? month = null, int? day = null, string? teamName = null, string? leagueName = null, int? season = null, string? round = null);
    }
}
