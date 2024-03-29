using FootballResults.WebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace FootballResults.WebApp.Services
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
