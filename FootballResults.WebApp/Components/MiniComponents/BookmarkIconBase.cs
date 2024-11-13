using FootballResults.DataAccess.Entities;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FootballResults.WebApp.Components.MiniComponents
{
    public partial class BookmarkIconBase : ComponentBase
    {
        [Inject]
        public IUserService? UserService { get; set; }

        [Inject]
        public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        [CascadingParameter(Name = "User")]
        public User? User { get; set; }

        [Parameter]
        public EventCallback ButtonClicked { get; set; }

        [Parameter]
        public IBookmarkable? Bookmark { get; set; }

        [Parameter]
        public String? DefaultColor { get; set; }

        protected async Task FavoriteLeagueButtonClickedAsync()
        {
            if (User!.FavoriteLeagues.Select(fl => fl.BookmarkID).Contains(Bookmark!.BookmarkID))
            {
                await UserService!.RemoveFromFavoriteLeaguesAsync(User, Bookmark.BookmarkID);
            }
            else
            {
                await UserService!.AddToFavoriteLeaguesAsync(User, Bookmark.BookmarkID);
            }

            await ButtonClicked.InvokeAsync();
        }

        protected async Task FavoriteTeamButtonClickedAsync()
        {
            if (User!.FavoriteTeams.Select(ft => ft.BookmarkID).Contains(Bookmark!.BookmarkID))
            {
                await UserService!.RemoveFromFavoriteTeamsAsync(User, Bookmark.BookmarkID);
            }
            else
            {
                await UserService!.AddToFavoriteTeamsAsync(User, Bookmark.BookmarkID);
            }

            await ButtonClicked.InvokeAsync();
        }
    }
}
