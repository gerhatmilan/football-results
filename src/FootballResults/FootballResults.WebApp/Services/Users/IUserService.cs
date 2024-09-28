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
        Task AddToFavoriteLeaguesAsync(int userID, int leagueID);
        Task AddToFavoriteTeamsAsync(int userID, int teamID);
        Task RemoveFromFavoriteLeaguesAsync(int userID, int leagueID);
        Task RemoveFromFavoriteTeamsAsync(int userID, int teamID);
    }
}
