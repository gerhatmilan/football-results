using FootballResults.WebApp.Models;
using FootballResults.WebApp.Services;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages.UserRelated
{
    public class SignupBase : ComponentBase
    {
        [SupplyParameterFromForm]
        public User User { get; set; } = new User();

        [Inject]
        public ISignupService? SignupService { get; set; }

        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        protected String? ErrorMessage { get; set; }

        protected SignUpResult SignUpResult { get; set; }

        protected async Task RegisterUserAsync()
        {
            SignUpResult = await SignupService!.RegisterUserAsync(User);

            if (SignUpResult == SignUpResult.Error)
                NavigationManager!.NavigateTo("Error", true);
        }
    }
}
