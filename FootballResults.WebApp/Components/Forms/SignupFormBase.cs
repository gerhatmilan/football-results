using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components;
using FootballResults.Models.ViewModels.Users;

namespace FootballResults.WebApp.Components.Forms
{
    public partial class SignupFormBase : FormBase
    {
        [SupplyParameterFromForm(FormName = "SignUpForm")]
        public SignupModel SignupModel { get; set; } = new SignupModel();

        [Inject]
        public ISignupService? SignupService { get; set; }

        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        protected override void ResetErrorMessages()
        {
            SignupModel.ResetMessages();
        }

        protected async Task RegisterUserAsync()
        {
            ResetErrorMessages();
            DisableForm();

            await SignupService!.RegisterUserAsync(SignupModel);

            await EnableForm();
        }
    }
}
