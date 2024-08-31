using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResults.DataAccess.Entities.Predictions
{
    public class PredictionGame : Entity
    {
        /// <summary>
        /// ID of the user who created the prediction game
        /// </summary>
        public int OwnerID { get; set; }

        /// <summary>
        /// Name of the prediction game
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A key that can be used to join the prediction game
        /// </summary>
        public string JoinKey { get; set; }

        /// <summary>
        /// Description of the prediction game
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A file path to the image of the prediction game
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Points given for an exact scoreline prediction
        /// </summary>
        public int ExactScorelineReward { get; set; }

        /// <summary>
        /// Points given for a correct outcome prediction
        /// </summary>
        public int OutcomeReward { get; set; }

        /// <summary>
        /// Points given for a correct goal count prediction
        /// </summary>
        public int GoalCountReward { get; set; }

        /// <summary>
        /// Points given for a correct goal difference prediction
        /// </summary>
        public int GoalDifferenceReward { get; set; }

        /// <summary>
        /// Date and time when the prediction game was created
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Whether the prediction game has finished
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// Owner of the prediction game
        /// </summary>
        public User Owner { get; set; }

        /// <summary>
        /// League seasons containing matches that can be predicted
        /// </summary>
        public IEnumerable<LeagueSeason> LeagueSeasons { get; set; }

        /// <summary>
        /// Matches that can be predicted in the prediction game
        /// </summary>
        [NotMapped]
        public IEnumerable<Match> Matches => LeagueSeasons.SelectMany(ls => ls.Matches);      

        /// <summary>
        /// Messages sent by users in the prediction game
        /// </summary>
        public IEnumerable<Message> Messages { get; set; }

        /// <summary>
        /// Players participating in the prediction game
        /// </summary>
        public IEnumerable<User> Players { get; set; }

        /// <summary>
        /// Standings of the prediction game
        /// </summary>
        [NotMapped]
        public IEnumerable<PredictionGameStanding> Standings => Participations?.Select(p => p.Standing);

        /// <summary>
        /// Predictions made in the prediction game
        /// </summary>
        [NotMapped]
        public IEnumerable<Prediction> Predictions => Participations?.SelectMany(p => p.Predictions);

        // skip navigations
        public IEnumerable<Participation> Participations { get; set; }
        
        public IEnumerable<PredictionGameLeagueSeason> PredictionGameLeagueSeasons { get; set; }

        public void RefreshData()
        {
            if (!Finished)
            {
                RefreshStandings();
                RefreshFinished();
            }
        }

        public IEnumerable<PredictionGameStanding> GetLiveStandings()
        {
            ICollection<PredictionGameStanding> existingStandingsCopy = new List<PredictionGameStanding>();

            Standings.ToList().ForEach(s =>
            {
                var standingCopy = new PredictionGameStanding
                {
                    ID = s.ID,
                    ParticipationID = s.ParticipationID,
                    Points = s.Points,
                    LastUpdate = s.LastUpdate,
                    Participation = s.Participation,
                    PredictionGame = s.PredictionGame,
                    User = s.User
                };

                existingStandingsCopy.Add(standingCopy);
            });

            Predictions.ToList().ForEach(p =>
            {
                if (!p.PointsGiven)
                {
                    var points = p.CalculatePoints();
                    existingStandingsCopy.FirstOrDefault(s => s.PredictionGame.ID == ID && s.User.ID == p.User.ID).Points += points;
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
                    var standing = prediction.PredictionGame.Standings.FirstOrDefault(s => s.PredictionGame.ID == ID && s.User.ID == prediction.User.ID);

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
            if (LeagueSeasons.SelectMany(l => l.Matches).All(m => m.IsFinished()))
                Finished = true;
        }
    }
}
