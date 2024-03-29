#nullable disable

namespace FootballResults.WebApp.Models
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
        public User Owner { get; set; }
    }
}
