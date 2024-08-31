using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.Users;

namespace FootballResults.WebApp.Services.Users
{
    public enum ModifyUserResult
    {
        None,
        Success,
        UsernameAlreadyInUse,
        Error
    }

    public interface IUserService
    {
        Task<User?> GetUserAsync(int userID);

        Task<ModifyUserResult> ModifyUserAsync(User user, SettingsModel settingsModel);
        Task AddToFavoriteLeaguesAsync(User user, League league);
        Task AddToFavoriteTeamsAsync(User user, Team team);
        Task RemoveFromFavoriteLeaguesAsync(User user, League League);
        Task RemoveFromFavoriteTeamsAsync(User user, Team team);
        Task GetGameDataForUserAsync(User user);
    }
}
