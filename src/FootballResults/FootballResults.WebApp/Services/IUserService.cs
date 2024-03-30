using FootballResults.WebApp.Models;

namespace FootballResults.WebApp.Services
{
    public interface IUserService
    {
        Task<User?> GetUserAsync(int userID);
        Task AddToFavoriteLeaguesAsync(int userID, int leagueID);
        Task AddToFavoriteTeamsAsync(int userID, int teamID);
        Task RemoveFromFavoriteLeaguesAsync(int userID, int leagueID);
        Task RemoveFromFavoriteTeamsAsync(int userID, int teamID);
    }
}
