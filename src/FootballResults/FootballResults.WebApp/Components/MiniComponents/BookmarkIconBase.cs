using FootballResults.Models.Football;
using FootballResults.Models.Users;
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

        protected IEnumerable<int> FavoriteLeagueIDs { get => User?.FavoriteLeagues?.Select(fl => fl.LeagueID) ?? new List<int>(); }

        protected IEnumerable<int> FavoriteTeamIDs { get => User?.FavoriteTeams?.Select(ft => ft.TeamID) ?? new List<int>(); }

        protected async Task FavoriteLeagueButtonClickedAsync()
        {
            if (FavoriteLeagueIDs.Contains(Bookmark!.BookmarkID))
            {
                await UserService!.RemoveFromFavoriteLeaguesAsync(User!.UserID, Bookmark!.BookmarkID);
            }
            else
            {
                await UserService!.AddToFavoriteLeaguesAsync(User!.UserID, Bookmark!.BookmarkID);
            }

            await ButtonClicked.InvokeAsync();
        }

        protected async Task FavoriteTeamButtonClickedAsync()
        {
            if (FavoriteTeamIDs.Contains(Bookmark!.BookmarkID))
            {
                await UserService!.RemoveFromFavoriteTeamsAsync(User!.UserID, Bookmark!.BookmarkID);
            }
            else
            {
                await UserService!.AddToFavoriteTeamsAsync(User!.UserID, Bookmark!.BookmarkID);
            }

            await ButtonClicked.InvokeAsync();
        }
    }
}
