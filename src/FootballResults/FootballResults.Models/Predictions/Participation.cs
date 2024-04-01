using FootballResults.Models.Users;

namespace FootballResults.Models.Predictions
{
    public class Participation
    {
        public int PredictionGameID { get; set; }
        public int UserID { get; set; }
        public DateTime? JoinDate { get; set; }
        public PredictionGame PredictionGame { get; set; }
        public User User { get; set; }
    }
}
