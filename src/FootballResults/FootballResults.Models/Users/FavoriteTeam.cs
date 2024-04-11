using FootballResults.Models.Football;

namespace FootballResults.Models.Users
{
    public class FavoriteTeam
    {
        public int UserID { get; set; }
        public int TeamID { get; set; }
        public User User { get; set; }
        public Team Team { get; set; }
    }
}
