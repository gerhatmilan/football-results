using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.ViewModels.Users;

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
        Task<bool> ModifyUserAsync(User user, SettingsModel settingsModel);
        Task AddToFavoriteLeaguesAsync(User user, int leagueID);
        Task AddToFavoriteTeamsAsync(User user, int teamID);
        Task RemoveFromFavoriteLeaguesAsync(User user, int leagueID);
        Task RemoveFromFavoriteTeamsAsync(User user, int teamID);
    }
}
