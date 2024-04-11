using FootballResults.Models.Football;
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
        public ICollection<League> Leagues { get; set; }
        public ICollection<Prediction> Predictions { get; set; }
        public ICollection<User> Players { get; set; }
        public IEnumerable<GameStanding> Standings { get; set; } 
        public User Owner { get; set; }

        // skip navigations
        public IEnumerable<Participation> Participations { get; set; }
        public IEnumerable<GameLeague> GameLeagues { get; set; }

        public void RefreshData()
        {
            RefreshStandings();
            RefreshFinished();
        }

        private void RefreshStandings()
        {
            foreach (var prediction in Predictions)
            {
                if (prediction.Match.IsFinished() && !prediction.PointsGiven)
                {
                    var points = prediction.CalculatePoints();
                    var standing = prediction.Game.Standings.FirstOrDefault(s => s.GameID == GameID && s.UserID == prediction.UserID);

                    if (standing != null)
                    {
                        standing.Points += points;
                        prediction.PointsGiven = true;
                    }
                }
            }
        }

        private void RefreshFinished()
        {
            if (Leagues.SelectMany(l => l.Matches).All(m => m.IsFinished()))
                IsFinished = true;
        }
    }
}
