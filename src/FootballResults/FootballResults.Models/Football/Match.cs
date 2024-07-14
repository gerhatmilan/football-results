using FootballResults.Models.Predictions;

namespace FootballResults.Models.Football
{
    public class Match
    {
        public int MatchID { get; set; }

        public DateTime? Date { get; set; }

        public int? VenueID { get; set; }

        public int LeagueID { get; set; }

        public int Season { get; set; }

        public string Round { get; set; }

        public int HomeTeamID { get; set; }

        public int AwayTeamID { get; set; }

        public string Status { get; set; }

        public int? Minute { get; set; }

        public int? HomeTeamGoals { get; set; }

        public int? AwayTeamGoals { get; set; }

        public DateTime? LastUpdate { get; set; }

        public Venue Venue { get; set; }

        public League League { get; set; }

        public Team HomeTeam { get; set; }

        public Team AwayTeam { get; set; }

        public IEnumerable<Prediction> Predictions { get; set; }

        public bool DateKnown()
        {
            return Date != null;
        }

        public bool IsInProgress()
        {
            return (Date < DateTime.UtcNow) && (Status == "1H" || Status == "HT" || Status == "2H"
                || Status == "ET" || Status == "BT" || Status == "P" || Status == "SUSP" || Status == "INT" || Status == "LIVE");
        }

        public bool IsFinished()
        {
            return (Date < DateTime.UtcNow) && (Status == "FT" || Status == "AET" || Status == "PEN" || Status == "WO");
        }

        public bool HasStarted()
        {
            return Date < DateTime.UtcNow;
        }
    }
}
