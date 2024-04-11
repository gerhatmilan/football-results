using FootballResults.Models.Users;

namespace FootballResults.Models.Predictions
{
    public class GameStanding
    {
        public int GameID { get; set; }
        public int UserID { get; set; }
        public int Points { get; set; }
        public DateTime? LastUpdate { get; set; }

        public PredictionGame Game { get; set; }
        public User User { get; set; }
    }
}
