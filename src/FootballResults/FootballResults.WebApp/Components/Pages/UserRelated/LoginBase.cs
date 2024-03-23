using FootballResults.WebApp.Models;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages.UserRelated
{
    public class LoginBase : ComponentBase
    {
        [SupplyParameterFromForm]
        public User User { get; set; } = new User();

        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        protected void AuthenticateUserAsync()
        {
            throw new NotImplementedException();
        }
    }
}
