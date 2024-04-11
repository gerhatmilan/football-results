using FootballResults.Models.Users;

namespace FootballResults.WebApp.Services.Users
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
