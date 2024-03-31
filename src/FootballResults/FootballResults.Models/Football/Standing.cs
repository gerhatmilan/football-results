namespace FootballResults.Models.Football
{
    public class Standing
    {
        public int LeagueID { get; set; }

        public int Season { get; set; }

        public int TeamID { get; set; }

        public int Rank { get; set; }

        public string Group { get; set; }

        public int Points { get; set; }

        public int Played { get; set; }

        public int Wins { get; set; }

        public int Draws { get; set; }

        public int Losses { get; set; }

        public int Scored { get; set; }

        public int Conceded { get; set; }

        public DateTime? LastUpdate { get; set; }

        public League League { get; set; }

        public Team Team { get; set; }

    }
}
