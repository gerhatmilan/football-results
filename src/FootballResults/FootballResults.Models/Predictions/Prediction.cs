
using FootballResults.Models.Users;

namespace FootballResults.Models.Predictions
{
    public class Prediction
    {
        public int UserID { get; set; }
        public int PredictionGameID { get; set; }
        public int MatchID { get; set; }
        public int HomeTeamGoals { get; set; }
        public int AwayTeamGoals { get; set; }
        public DateTime? PredictionDate { get; set; }

        public User User { get; set; }
        public PredictionGame PredictionGame { get; set; }
    }
}
