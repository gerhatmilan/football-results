using FootballResults.DataAccess.Entities.Football;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResults.DataAccess.Entities.Predictions
{
    public class PredictionGameSeason : Entity
    {
        /// <summary>
        /// Prediction game ID
        /// </summary>
        public int PredictionGameID { get; set; }

        /// <summary>
        /// League season ID
        /// </summary>
        public int LeagueSeasonID { get; set; }

        /// <summary>
        /// Prediction game
        /// </summary>
        public PredictionGame PredictionGame { get; set; }

        /// <summary>
        /// League season
        /// </summary>
        public LeagueSeason LeagueSeason { get; set; }
        
        /// <summary>
        /// League
        /// </summary>
        [NotMapped]
        public League League => LeagueSeason?.League;
    }
}