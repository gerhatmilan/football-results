using FootballResults.Models.ViewModels.Users;

namespace FootballResults.WebApp.Services.Users
{
    public interface ISignupService
    {
        Task RegisterUserAsync(SignupModel model);
    }
}
