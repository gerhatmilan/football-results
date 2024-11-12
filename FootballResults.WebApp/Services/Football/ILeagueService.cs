using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;

namespace FootballResults.WebApp.Services.Football
{
    public interface ILeagueService
    {
        Task<League?> GetLeagueByNameAsync(string leagueName, bool tracking = true);
        Task<IEnumerable<League>> GetLeaguesAsync(bool tracking = true);
        Task<IEnumerable<Country>> GetCountriesWithLeaguesAsync(bool tracking = true);
        Task<IEnumerable<LeagueStanding>> GetStandingsForLeagueAndSeasonAsync(string leagueName, int season, bool tracking = true);
        Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeasonAsync(string leagueName, int season, bool tracking = true);
        Task<IEnumerable<League>> SearchAsync(string leagueName, bool tracking = true);
    }
}
