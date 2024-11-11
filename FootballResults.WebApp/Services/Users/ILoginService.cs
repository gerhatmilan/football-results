using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.ViewModels.Users;

namespace FootballResults.WebApp.Services.Users
{
    public interface ILoginService
    {
        Task<User?> AuthenticateUserAsync(string username, string password);
    }
}
