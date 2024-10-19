using FootballResults.DataAccess.Entities.Users;
using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Predictions
{
    public class Participation : EntityWithID
    {
        /// <summary>
        /// ID of the prediction game the user is participating in
        /// </summary>
        public int PredictionGameID { get; set; }

        /// <summary>
        /// ID of the user participating in the prediction game
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// ID of the standing of the user in the prediction game
        /// </summary>
        public int StandingID { get; set; }

        /// <summary>
        /// Time when the user joined the prediction game
        /// </summary>
        public DateTime? JoinDate { get; set; }

        /// <summary>
        /// Prediction game the user is participating in
        /// </summary>
        [JsonIgnore]
        public PredictionGame PredictionGame { get; set; }

        /// <summary>
        /// User participating in the prediction game
        /// </summary>
        [JsonIgnore]
        public User User { get; set; }

        /// <summary>
        /// Predictions made by the user in the prediction game
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Prediction> Predictions { get; set; }

        /// <summary>
        /// Standing of the user in the prediction game
        /// </summary>
        [JsonIgnore]
        public PredictionGameStanding Standing { get; set; }
    }
}
