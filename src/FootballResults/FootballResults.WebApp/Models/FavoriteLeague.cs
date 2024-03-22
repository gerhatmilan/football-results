namespace FootballResults.WebApp.Models
{
    public class FavoriteLeague
    {
        public int UserID { get; set; }
        public int LeagueID { get; set; }
        public User? User { get; set; }
    }
}
