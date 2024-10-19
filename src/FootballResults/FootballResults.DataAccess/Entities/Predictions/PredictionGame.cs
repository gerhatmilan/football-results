using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResults.DataAccess.Entities.Predictions
{
    public class PredictionGame : EntityWithID
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
        /// Last time the standings of this game were updated
        /// </summary>
        public DateTime? StandingsLastUpdate { get; set; }

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
        /// Standings of league seasons that are included in the prediction game
        /// </summary>
        [NotMapped]
        public IEnumerable<LeagueStanding> LeagueStandings => LeagueSeasons.SelectMany(ls => ls.Standings);

        /// <summary>
        /// Messages sent by users in the prediction game
        /// </summary>
        public ICollection<Message> Messages { get; set; }

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

        [NotMapped]
        public IEnumerable<PredictionGameStanding> LiveStandings
        {
            get
            {
                ICollection<PredictionGameStanding> existingStandingsCopy = new List<PredictionGameStanding>();

                Standings.ToList().ForEach(s =>
                {
                    var standingCopy = new PredictionGameStanding
                    {
                        ID = s.ID,
                        ParticipationID = s.ParticipationID,
                        Points = s.Points,
                        Participation = s.Participation,
                        PredictionGame = s.PredictionGame,
                        User = s.User
                    };

                    existingStandingsCopy.Add(standingCopy);
                });

                foreach (Prediction prediction in Predictions)
                {
                    if (!prediction.PointsGiven)
                    {
                        var points = prediction.CalculatePoints();
                        existingStandingsCopy.FirstOrDefault(s => s.PredictionGame.ID == ID && s.User.ID == prediction.User.ID).Points += points;
                    }
                }
                return existingStandingsCopy.OrderByDescending(s => s.Points);
            }
        }

        // skip navigations
        public IEnumerable<Participation> Participations { get; set; }
        
        public IEnumerable<PredictionGameSeason> PredictionGameSeasons { get; set; }

        public void RefreshData()
        {
            if (!Finished)
            {
                RefreshStandings();
                RefreshFinished();
            }
        }

        private void RefreshStandings()
        {
            Predictions.Where(p => p.Match.IsFinished && !p.PointsGiven).ToList()
                .ForEach(prediction =>
                {
                    var points = prediction.CalculatePoints();
                    var standing = prediction.Participation.Standing;

                    if (standing != null)
                    {
                        standing.Points += points;
                        prediction.PointsGiven = true;
                    }
                });

            StandingsLastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        }

        private void RefreshFinished()
        {
            if (LeagueSeasons.SelectMany(l => l.Matches).All(m => m.IsFinished))
            {
                Finished = true;
            }
        }
    }
}
