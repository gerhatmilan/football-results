using FootballResults.Models.Predictions;
using System.Text.Json.Serialization;

namespace FootballResults.Models.Users
{
    public class Message
    {
        public int MessageID { get; set; }
        
        public int UserID { get; set; }
        
        public DateTime SentAt { get; set; }

        public string Text { get; set; }

        public int? MatchID { get; set; }
        
        public int? GameID { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public PredictionGame Game { get; set; }
    }
}
