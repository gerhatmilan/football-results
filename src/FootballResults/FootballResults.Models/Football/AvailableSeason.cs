namespace FootballResults.Models.Football
{
    public class AvailableSeason
    {
        public int LeagueID { get; set; }

        public int Season { get; set; }

        public League League { get; set; }
    }
}