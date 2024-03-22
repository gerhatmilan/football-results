using FootballResults.WebApp.Models;

namespace FootballResults.WebApp.Services
{
    public interface ISignupService
    {
        Task<SignUpResult> RegisterUserAsync(User user);
        bool IsDupliateEmail(User user);
        bool IsDuplicateUsername(User user);
        string GetHashedPassword(User user);
    }
}
