using FootballResults.Models.Users;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Forms
{
    public class LoginFormBase : ComponentBase
    {
        [Inject]
        protected ILoginService? LoginService { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        protected LoginResult LoginResult { get; set; }

        [SupplyParameterFromForm(FormName = "LoginForm")]
        public LoginModel LoginModel { get; set; } = new LoginModel();

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }


        protected async Task AuthenticateUserAsync()
        {
            try
            {
                var user = new User()
                {
                    Username = LoginModel.Username,
                    Password = LoginModel.Password
                };

                (User? userInDatabase, LoginResult) = await LoginService!.AuthenticateUserAsync(user);

                if (LoginResult == LoginResult.Success)
                {
                    NavigationManager!.NavigateTo($"/user/login/{userInDatabase!.UserID}", true);
                }
            }
            catch (Exception)
            {
                NavigationManager!.NavigateTo("/Error", true);
            }
        }
    }
}
