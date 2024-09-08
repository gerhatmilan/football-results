using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Football
{
    public class Match : Entity
    {
        /// <summary>
        /// ID of the league season the match belongs to
        /// </summary>
        public int LeagueSeasonID { get; set; }
        
        /// <summary>
        /// ID of the venue of this match
        /// </summary>
        public int? VenueID { get; set; }

        /// <summary>
        /// ID of the home team of this match
        /// </summary>
        public int HomeTeamID { get; set; }

        /// <summary>
        /// ID of the away team of this match
        /// </summary>
        public int AwayTeamID { get; set; }

        /// <summary>
        /// Round of the match
        /// </summary>
        public string Round { get; set; }

        /// <summary>
        /// Date of the match
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Status of the match (e.g NS, 1H, HT, 2H, FT, ...)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Minute of the match
        /// </summary>
        public int? Minute { get; set; }

        /// <summary>
        /// Goals scored by the home team
        /// </summary>
        public int? HomeTeamGoals { get; set; }

        /// <summary>
        /// Goals scored by the away team
        /// </summary>
        public int? AwayTeamGoals { get; set; }

        /// <summary>
        /// Last time when the match info was updated
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        /// <summary>
        /// League season of the match
        /// </summary>
        public LeagueSeason LeagueSeason { get; set; }

        /// <summary>
        /// League of the match
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public League League { get => LeagueSeason?.League; set { } }
        
        /// <summary>
        /// Venue of the match
        /// </summary>
        public Venue Venue { get; set; }

        /// <summary>
        /// Home team of the match
        /// </summary>
        public Team HomeTeam { get; set; }

        /// <summary>
        /// Away team of the match
        /// </summary>
        public Team AwayTeam { get; set; }

        /// <summary>
        /// Predictions made for the match
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Prediction> Predictions { get; set; }

        /// <summary>
        /// Messages sent for the match
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Message> Messages { get; set; }

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

        public bool Equals(Match match)
        {
            return ID == match.ID &&
                LeagueSeasonID == match.LeagueSeasonID &&
                VenueID == match.VenueID &&
                HomeTeamID == match.HomeTeamID &&
                AwayTeamID == match.AwayTeamID &&
                Round == match.Round &&
                Date == match.Date &&
                Status == match.Status &&
                Minute == match.Minute &&
                HomeTeamGoals == match.HomeTeamGoals &&
                AwayTeamGoals == match.AwayTeamGoals;
        }

        public void CopyFrom(Match other)
        {
            ID = other.ID;
            LeagueSeasonID = other.LeagueSeasonID;
            VenueID = other.VenueID;
            HomeTeamID = other.HomeTeamID;
            AwayTeamID = other.AwayTeamID;
            Round = other.Round;
            Date = other.Date;
            Status = other.Status;
            Minute = other.Minute;
            HomeTeamGoals = other.HomeTeamGoals;
            AwayTeamGoals = other.AwayTeamGoals;
            LastUpdate = other.LastUpdate;
        }
    }
}
