using FootballResults.DataAccess.Entities.Users;

namespace FootballResults.WebApp.Services.Users
{
    public enum LoginResult
    {
        None,
        Success,
        UserNotFound,
        InvalidPassword
    }

    public interface ILoginService
    {
        Task<Tuple<User?, LoginResult>> AuthenticateUserAsync(User user);
        Task<User?> GetUserAsync(User user);
    }
}
