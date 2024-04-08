using FootballResults.Models.Football;

namespace FootballResults.Models.Users
{
    public class FavoriteLeague
    {
        public int UserID { get; set; }
        public int LeagueID { get; set; }
        public User User { get; set; }
        public League League { get; set; }
    }
}
