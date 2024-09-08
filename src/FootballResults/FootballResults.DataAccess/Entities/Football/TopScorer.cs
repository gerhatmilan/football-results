using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Football
{
    public class TopScorer : Entity
    {
        /// <summary>
        /// ID of the league season for the topscorer
        /// </summary>
        public int LeagueSeasonID { get; set; }
        
        /// <summary>
        /// ID of the team of the topscorer
        /// </summary>
        public int TeamID { get; set; }

        /// <summary>
        /// Position where the player is ranked
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Name of the player
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// A link pointing to an image of the player
        /// </summary>
        public string PhotoLink { get; set; }

        /// <summary>
        /// Number of games played by the player
        /// </summary>
        public int? Played { get; set; }

        /// <summary>
        /// Number of goals scored by the player
        /// </summary>
        public int Goals { get; set; }

        /// <summary>
        /// Number of assists made by the player
        /// </summary>
        public int? Assists { get; set; }

        /// <summary>
        /// League season of the topscorer record
        /// </summary>
        [JsonIgnore]
        public LeagueSeason LeagueSeason { get; set; }

        /// <summary>
        /// League of the topscorer record
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public League League => LeagueSeason?.League;

        /// <summary>
        /// Team of the topscorer
        /// </summary>
        public Team Team { get; set; }

        public bool Equals(TopScorer topScorer)
        {
            return LeagueSeasonID == topScorer.LeagueSeasonID &&
                TeamID == topScorer.TeamID &&
                Rank == topScorer.Rank &&
                PlayerName == topScorer.PlayerName &&
                PhotoLink == topScorer.PhotoLink &&
                Played == topScorer.Played &&
                Goals == topScorer.Goals &&
                Assists == topScorer.Assists;
        }

        public void CopyFrom(TopScorer other)
        {
            LeagueSeasonID = other.LeagueSeasonID;
            TeamID = other.TeamID;
            Rank = other.Rank;
            PlayerName = other.PlayerName;
            PhotoLink = other.PhotoLink;
            Played = other.Played;
            Goals = other.Goals;
            Assists = other.Assists;
        }
    }
}