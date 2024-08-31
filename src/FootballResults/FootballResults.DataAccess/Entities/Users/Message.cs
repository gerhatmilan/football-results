using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Predictions;
using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Users
{
    public class Message : Entity
    {
        /// <summary>
        /// ID of the match the message is related to (if any)
        /// </summary>
        public int? MatchID { get; set; }

        /// <summary>
        /// ID of the prediction game the message is related to (if any)
        /// </summary>
        public int? PredictionGameID { get; set; }
        
        /// <summary>
        /// ID of the user who sent the message
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Date and time when the message was sent
        /// </summary>
        public DateTime SentAt { get; set; }

        /// <summary>
        /// Text of the message
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// User who sent the message
        /// </summary>
        [JsonIgnore]
        public User User { get; set; }

        /// <summary>
        /// Match the message is related to (if any)
        /// </summary>
        [JsonIgnore]
        public Match Match { get; set; }

        /// <summary>
        /// Prediction game the message is related to (if any)
        /// </summary>
        [JsonIgnore]
        public PredictionGame PredictionGame { get; set; }
    }
}
