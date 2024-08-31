using FootballResults.DataAccess.Entities.Users;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FootballResults.WebApp.Components.Pages.PredictionGames
{
    public partial class PredictionGamesBase : ComponentBase
    {
        [Inject]
        protected IUserService? UserService { get; set; }

        [Inject]
        protected AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        [Inject]
        protected NavigationManager? NavigationManager { get; set; }

        [CascadingParameter(Name = "User")]
        protected User? User { get; set; }

        public string? ActiveSubMenu { get; set; } = "games";
    }
}
