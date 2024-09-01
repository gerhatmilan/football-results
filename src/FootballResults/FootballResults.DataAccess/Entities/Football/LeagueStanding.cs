using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Football
{
    public class LeagueStanding : Entity
    {
        /// <summary>
        /// ID of the league season of the standing
        /// </summary>
        public int LeagueSeasonID { get; set; }

        /// <summary>
        /// ID of the team of the standing
        /// </summary>
        public int TeamID { get; set; }

        /// <summary>
        /// Rank of the team
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Group of the standing (for example, for cup formats there are Group A, Group B, etc.)
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Points earned by the team
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Number of matches played by the team
        /// </summary>
        public int Played { get; set; }

        /// <summary>
        /// Number of matches won by the team
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Number of matches drawn by the team
        /// </summary>
        public int Draws { get; set; }

        /// <summary>
        /// Number of matches lost by the team
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Number of goals scored by the team
        /// </summary>
        public int Scored { get; set; }

        /// <summary>
        /// Number of goals conceded by the team
        /// </summary>
        public int Conceded { get; set; }

        /// <summary>
        /// Last time when the standing info was updated
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        /// <summary>
        /// League season of the standing
        /// </summary>
        [JsonIgnore]
        public LeagueSeason LeagueSeason { get; set; }

        /// <summary>
        /// League of the standing
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public League League => LeagueSeason?.League;

        /// <summary>
        /// Team of the standing
        /// </summary>
        public Team Team { get; set; }

        public bool Equals(LeagueStanding standing)
        {
            return LeagueSeasonID == standing.LeagueSeasonID &&
                   TeamID == standing.TeamID &&
                   Rank == standing.Rank &&
                   Group == standing.Group &&
                   Points == standing.Points &&
                   Played == standing.Played &&
                   Wins == standing.Wins &&
                   Draws == standing.Draws &&
                   Losses == standing.Losses &&
                   Scored == standing.Scored &&
                   Conceded == standing.Conceded;
        }

        public void CopyFrom(LeagueStanding other)
        {
            LeagueSeasonID = other.LeagueSeasonID;
            TeamID = other.TeamID;
            Rank = other.Rank;
            Group = other.Group;
            Points = other.Points;
            Played = other.Played;
            Wins = other.Wins;
            Draws = other.Draws;
            Losses = other.Losses;
            Scored = other.Scored;
            Conceded = other.Conceded;
        }
    }
}
