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
        public IEnumerable<Message> Messages { get; set; }

        // skip navigations
        public IEnumerable<Participation> Participations { get; set; }
        public IEnumerable<GameLeague> GameLeagues { get; set; }

        public void RefreshData()
        {
            if (!IsFinished)
            {
                RefreshStandings();
                RefreshFinished();
            }
        }

        public IEnumerable<GameStanding> GetLiveStandings()
        {
            ICollection<GameStanding> existingStandingsCopy = new List<GameStanding>();

            Standings.ToList().ForEach(s =>
            {
                var standingCopy = new GameStanding
                {
                    GameID = s.GameID,
                    UserID = s.UserID,
                    Points = s.Points,
                    LastUpdate = s.LastUpdate,
                    Game = s.Game,
                    User = s.User
                };

                existingStandingsCopy.Add(standingCopy);
            });

            Predictions.ToList().ForEach(p =>
            {
                if (!p.PointsGiven)
                {
                    var points = p.CalculatePoints();
                    existingStandingsCopy.FirstOrDefault(s => s.GameID == GameID && s.UserID == p.UserID).Points += points;
                }
            });

            return existingStandingsCopy.OrderByDescending(s => s.Points);
        }

        private void RefreshStandings()
        {
            Predictions.ToList().ForEach(prediction =>
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
            });
        }

        private void RefreshFinished()
        {
            if (Leagues.SelectMany(l => l.Matches).All(m => m.IsFinished()))
                IsFinished = true;
        }
    }
}
