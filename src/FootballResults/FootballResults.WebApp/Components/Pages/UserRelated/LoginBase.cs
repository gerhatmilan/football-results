using FootballResults.WebApp.Models;
using FootballResults.WebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace FootballResults.WebApp.Components.Pages.UserRelated
{
    public partial class LoginBase : ComponentBase
    {
        [CascadingParameter]
        public HttpContext? HttpContext { get; set; }

        [Inject]
        public ILoginService? LoginService { get; set; }

        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        protected LoginResult LoginResult { get; set; }

        [SupplyParameterFromForm(FormName = "LoginForm")]
        public LoginModel LoginModel { get; set; } = new LoginModel();

        protected async Task AuthenticateUserAsync()
        {
            try
            {
                var user = new User()
                {
                    Username = LoginModel.Username,
                    Password = LoginModel.Password
                };

                LoginResult = await LoginService!.AuthenticateUserAsync(user);

                if (LoginResult == LoginResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username!),
                        new Claim(ClaimTypes.Role, "User")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext!.SignInAsync(claimsPrincipal);
                }
            }
            catch (Exception)
            {
                NavigationManager!.NavigateTo("/Error", true);
            }
        }
    }
}
