using FootballResults.Models.Users;
using FootballResults.WebApp.Services.Users;
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

        protected override void OnInitialized()
        {
            if (HttpContext == null)
            {
                NavigationManager!.NavigateTo("/login", true);
            }
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
                    var claims = new List<Claim>
                    {
                        new Claim("UserID", userInDatabase!.UserID!.ToString()),
                        new Claim(ClaimTypes.Name, userInDatabase!.Username!),
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
