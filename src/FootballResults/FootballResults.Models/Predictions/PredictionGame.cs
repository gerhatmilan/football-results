using FootballResults.Models.Users;

namespace FootballResults.Models.Predictions
{
    public class PredictionGame
    {
        public int GameID { get; set; }
        public string Name { get; set; }
        public int OwnerID { get; set; }
        public string JoinKey { get; set; }
        public string Description { get; set;}
        public string ImagePath { get; set; }
        public int ExactScorelineReward { get; set; }
        public int OutcomeReward { get; set; }
        public int GoalCountReward { get; set; }
        public int GoalDifferenceReward { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsFinished { get; set; }
        public IEnumerable<IncludedLeague> IncludedLeagues { get; set; }
        public IEnumerable<Prediction> Predictions { get; set; }
        public IEnumerable<User> Players { get; set; }

        public IEnumerable<GameStanding> Standings { get; set; } 
        public IEnumerable<Participation> Participations { get; set; }
    }
}
