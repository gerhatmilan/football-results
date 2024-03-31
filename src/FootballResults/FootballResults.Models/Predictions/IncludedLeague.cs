namespace FootballResults.Models.Predictions
{
    public class IncludedLeague
    {
        public int PredictionGameID { get; set; }
        public int LeagueID { get; set; }

        public PredictionGame PredictionGame { get; set; }
    }
}