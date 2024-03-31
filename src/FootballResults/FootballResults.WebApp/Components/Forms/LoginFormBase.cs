using FootballResults.Models.Users;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace FootballResults.WebApp.Components.Forms
{
    public class LoginFormBase : ComponentBase
    {
        [Parameter]
        public HttpContext? HttpContext { get; set; }

        [Inject]
        protected ILoginService? LoginService { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

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

                (User? userInDatabase, LoginResult) = await LoginService!.AuthenticateUserAsync(user);

                if (LoginResult == LoginResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim("UserID", userInDatabase!.UserID!.ToString()),
                        new Claim(ClaimTypes.Name, userInDatabase!.Username!),
                        new Claim(ClaimTypes.Role, "User")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext!.SignInAsync(claimsPrincipal);

                    NavigationManager!.NavigateTo("/", true);
                }
            }
            catch (NavigationException)
            {
                NavigationManager!.NavigateTo("/", true);
            }
            catch (Exception)
            {
                NavigationManager!.NavigateTo("/Error", true);
            }
        }
    }
}
