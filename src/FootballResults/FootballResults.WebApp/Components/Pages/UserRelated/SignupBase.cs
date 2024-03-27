using FootballResults.WebApp.Models;
using FootballResults.WebApp.Services;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages.UserRelated
{
    public partial class SignupBase : ComponentBase
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
                NavigationManager!.NavigateTo("/Error", true);
            }
        }
    }
}
