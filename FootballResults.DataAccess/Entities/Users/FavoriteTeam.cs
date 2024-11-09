using FootballResults.DataAccess.Entities.Football;

namespace FootballResults.DataAccess.Entities.Users
{
    public class FavoriteTeam : EntityWithID
    {
        /// <summary>
        /// ID of the user who marked the team as favorite
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// ID of the team the user marked as favorite
        /// </summary>
        public int TeamID { get; set; }

        /// <summary>
        /// User who marked the team as favorite
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Team the user marked as favorite
        /// </summary>
        public Team Team { get; set; }
    }
}
