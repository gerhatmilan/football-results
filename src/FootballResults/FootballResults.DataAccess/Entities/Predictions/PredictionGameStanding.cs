using FootballResults.DataAccess.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResults.DataAccess.Entities.Predictions
{
    public class PredictionGameStanding : Entity
    {
        /// <summary>
        /// ID of the participation
        /// </summary>
        public int ParticipationID { get; set; }

        /// <summary>
        /// Points scored by the user in the prediction game
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Last time the standing was updated
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        /// <summary>
        /// Participation the standing is for
        /// </summary>
        public Participation Participation { get; set; }

        /// <summary>
        /// Prediction game the standing is for
        /// </summary>
        [NotMapped]
        public PredictionGame PredictionGame { get => Participation?.PredictionGame; set { } }

        /// <summary>
        /// User the standing is for
        /// </summary>
        [NotMapped]
        public User User { get => Participation?.User; set { } }
    }
}
