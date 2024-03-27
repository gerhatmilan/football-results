using FootballResults.WebApp.Models;

namespace FootballResults.WebApp.Services
{
    public enum SignUpResult
    {
        None,
        Success,
        EmailAlreadyInUse,
        UsernameAlreadyInUse,
    }

    public interface ISignupService
    {
        Task<SignUpResult> RegisterUserAsync(User user);
    }
}
