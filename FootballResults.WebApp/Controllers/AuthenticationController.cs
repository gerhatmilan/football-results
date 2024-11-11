using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.Authentication;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FootballResults.WebApp.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public AuthenticationController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            User? userInDatabase = await _loginService.AuthenticateUserAsync(request.Username, request.Password);

            if (userInDatabase == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim("UserID", userInDatabase.ID.ToString()),
                new Claim(ClaimTypes.Name, userInDatabase.Username),
                new Claim(ClaimTypes.Role, "User"),
            };

            if (userInDatabase.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);


            await HttpContext!.SignInAsync(claimsPrincipal);

            return Ok();
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }
    }
}
