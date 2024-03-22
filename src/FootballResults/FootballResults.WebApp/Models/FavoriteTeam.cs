namespace FootballResults.WebApp.Models
{
    public class FavoriteTeam
    {
        public int UserID { get; set; }
        public int TeamID { get; set; }
        public User? User { get; set; }
    }
}
