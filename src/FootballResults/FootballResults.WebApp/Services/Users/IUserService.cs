using FootballResults.Models.Football;
using FootballResults.Models.Users;

namespace FootballResults.WebApp.Services.Users
{
    public interface IUserService
    {
        Task<User?> GetUserAsync(int userID);
        Task AddToFavoriteLeaguesAsync(int userID, int leagueID);
        Task AddToFavoriteTeamsAsync(int userID, int teamID);
        Task RemoveFromFavoriteLeaguesAsync(int userID, int leagueID);
        Task RemoveFromFavoriteTeamsAsync(int userID, int teamID);
        Task<IEnumerable<League>> GetFavoriteLeaguesAsync(User? user);
        Task<IEnumerable<Team>> GetFavoriteTeamsAsync(User? user);
    }
}
