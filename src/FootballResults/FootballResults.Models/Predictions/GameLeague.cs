using FootballResults.Models.Football;

namespace FootballResults.Models.Predictions
{
    public class GameLeague
    {
        public int GameID { get; set; }
        public int LeagueID { get; set; }
        public int Season { get; set; }

        public PredictionGame Game { get; set; }
        public League League { get; set; }
    }
}