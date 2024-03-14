using FootballResults.Components.Pages;
using FootballResults.Models;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.MiniComponents
{
    public partial class LeagueCardBase : ComponentBase
    {
        [Parameter]
        public League? League { get; set; }

        [Parameter]
        public IEnumerable<Match>? Matches { get; set; }

        protected const string DEFAULT_BUTTON_COLOR = "#ffffff";
        protected const string FAVORITE_BUTTON_COLOR = "var(--bs-link-color)";

        protected bool isFavorite = false; // TODO: implement favorite functionality

        protected void FavoriteButtonClicked(int leagueID)
        {
            if (isFavorite)
            {
                isFavorite = false;
                // TODO: remove from favorites
            }
            else
            {
                isFavorite = true;
                // TODO: add to favorites
            }
        }
    }
}