using FootballResults.DataAccess.Entities.Football;

namespace FootballResults.DataAccess.Entities.Users
{
    public class FavoriteLeague : EntityWithID
    {
        /// <summary>
        /// ID of the user who marked the league as favorite
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// ID of the league the user marked as favorite
        /// </summary>
        public int LeagueID { get; set; }
        
        /// <summary>
        /// User who marked the league as favorite
        /// </summary>
        public User User { get; set; }
        
        /// <summary>
        /// League the user marked as favorite
        /// </summary>
        public League League { get; set; }
    }
}
