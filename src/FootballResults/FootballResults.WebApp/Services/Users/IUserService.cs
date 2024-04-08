using FootballResults.Models.Football;
using FootballResults.Models.Users;

namespace FootballResults.WebApp.Services.Users
{
    public interface IUserService
    {
        Task<User?> GetUserAsync(int userID);

        Task<bool> ModifyUserAsync(User user, SettingsModel settingsModel);
        Task AddToFavoriteLeaguesAsync(User user, League league);
        Task AddToFavoriteTeamsAsync(User user, Team team);
        Task RemoveFromFavoriteLeaguesAsync(User user, League League);
        Task RemoveFromFavoriteTeamsAsync(User user, Team team);
    }
}
