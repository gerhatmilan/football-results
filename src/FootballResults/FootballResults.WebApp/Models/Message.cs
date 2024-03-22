namespace FootballResults.WebApp.Models
{
    public class Message
    {
        public int MessageID { get; set; }
        
        public int UserID { get; set; }
        
        public DateTime? SentAt { get; set; }

        public string? Text { get; set; }

        public int? MatchID { get; set; }
        
        public int? PredictionGameID { get; set; }

        public User? User { get; set; }
    }
}
