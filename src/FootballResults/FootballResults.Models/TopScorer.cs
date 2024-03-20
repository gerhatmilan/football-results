namespace FootballResults.Models
{
    public class TopScorer
    {
        public int LeagueID { get; set; }

        public int Season { get; set; }

        public int Rank { get; set; }

        public string PlayerName { get; set; }

        public string PhotoLink { get; set; }

        public int TeamID { get; set; }

        public int? Played { get; set; }

        public int Goals { get; set; }

        public int? Assists { get; set; }

        public DateTime? LastUpdate { get; set; }

        public League League { get; set; }

        public Team Team { get; set; }

    }
}