using FootballResults.Models.Football;
using FootballResults.Models.Users;

namespace FootballResults.WebApp.Services.Football
{
    public interface ILeagueService
    {
        Task<League?> GetLeagueByNameAsync(string leagueName);
        Task<IEnumerable<League>> GetLeaguesAsync();
        Task<IEnumerable<Country>> GetCountriesWithLeaguesAsync();
        Task<IEnumerable<LeagueStanding>> GetStandingsForLeagueAndSeasonAsync(string leagueName, int season);
        Task<IEnumerable<TopScorer>> GetTopScorersForLeagueAndSeasonAsync(string leagueName, int season);
        Task<IEnumerable<League>> SearchAsync(string leagueName);
        IEnumerable<League> GetLeaguesFavoritesFirst(User user, IEnumerable<League> leagues);
    }
}
