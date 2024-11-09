using FootballResults.DataAccess.Entities.Users;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components;
using FootballResults.Models.ViewModels.Users;

namespace FootballResults.WebApp.Components.Forms
{
    public partial class SignupFormBase : ComponentBase
    {
        [SupplyParameterFromForm(FormName = "SignUpForm")]
        public SignupModel SignupModel { get; set; } = new SignupModel();

        [Inject]
        public ISignupService? SignupService { get; set; }

        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        protected SignUpResult SignUpResult { get; set; }

        protected async Task RegisterUserAsync()
        {
            User user = new User
            {
                Email = SignupModel.Email,
                Username = SignupModel.Username,
                Password = SignupModel.Password
            };

            try
            {
                SignUpResult = await SignupService!.RegisterUserAsync(user);
            }
            catch (Exception)
            {
                NavigationManager!.NavigateTo("/error", true);
            }
        }
    }
}
