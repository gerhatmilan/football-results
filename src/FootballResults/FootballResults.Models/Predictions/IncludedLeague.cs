namespace FootballResults.Models.Predictions
{
    public class IncludedLeague
    {
        public int GameID { get; set; }
        public int LeagueID { get; set; }

        public PredictionGame Game { get; set; }
    }
}