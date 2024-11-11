using FootballResults.Models.Authentication;
using FootballResults.Models.ViewModels.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;

namespace FootballResults.WebApp.Components.Forms
{
    public class LoginFormBase : FormBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected HttpClient HttpClient { get; set; } = default!;

        [Inject] IJSRuntime JSRuntime { get; set; } = default!;

        [SupplyParameterFromForm(FormName = "LoginForm")]
        public LoginModel LoginModel { get; set; } = new LoginModel();

        protected override void ResetErrorMessages()
        {
            LoginModel.ResetMessages();
        }

        protected async Task AuthenticateUserAsync()
        {
            ResetErrorMessages();
            DisableForm();

            try
            {
                LoginRequest request = new LoginRequest
                {
                    Username = LoginModel.Username,
                    Password = LoginModel.Password
                };

                int responseStatusCode = await JSRuntime.InvokeAsync<int>("postRequest", NavigationManager.BaseUri + "api/authentication/login", request);

                switch (responseStatusCode)
                {
                    case (int)HttpStatusCode.OK:
                        NavigationManager.NavigateTo("/", forceLoad: true);
                        break;
                    case (int)HttpStatusCode.Unauthorized:
                        LoginModel.InvalidCredentialsError = true;
                        break;
                    default:
                        LoginModel.Error = true;
                        break;
                }
            }
            catch (Exception)
            {
                LoginModel.Error = true;
            }
        }
    }
}
