using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Football
{
    public class Match : EntityWithID
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

        public bool DateKnown
        {
            get => Date != null;
        }

        public bool IsInProgress
        {
            get => HasStarted && ( Status == MatchStatus.NotStarted || Status == MatchStatus.FirstHalf || Status == MatchStatus.HalfTime || Status == MatchStatus.SecondHalf
                || Status == MatchStatus.ExtraTime || Status == MatchStatus.BreakTime || Status == MatchStatus.PenaltiesInProgress || Status == MatchStatus.Suspended || Status == MatchStatus.Interrupted || Status == MatchStatus.Live);
        }

        public bool IsFinished
        {
            get => (Date < DateTime.UtcNow) && (Status == MatchStatus.FullTime || Status == MatchStatus.AwardedExtraTime || Status == MatchStatus.Penalties || Status == MatchStatus.WalkOver);
        }

        public bool IsToBeDecided
        {
            get => Status == MatchStatus.ToBeDecided;
        }

        public bool HasStarted
        {
            get => Date < DateTime.UtcNow;
        }

        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case MatchStatus.ToBeDecided:
                        return "To be decided";
                    case MatchStatus.NotStarted:
                        return "Not started";
                    case MatchStatus.FirstHalf:
                        return "First half";
                    case MatchStatus.HalfTime:
                        return "Half time";
                    case MatchStatus.SecondHalf:
                        return "Second half";
                    case MatchStatus.ExtraTime:
                        return "Extra time";
                    case MatchStatus.BreakTime:
                        return "Break time";
                    case MatchStatus.PenaltiesInProgress:
                        return "Penalties in progress";
                    case MatchStatus.Suspended:
                        return "Suspended";
                    case MatchStatus.Interrupted:
                        return "Interrupted";
                    case MatchStatus.FullTime:
                        return "Full time";
                    case MatchStatus.AwardedExtraTime:
                        return "Awarded extra time";
                    case MatchStatus.Penalties:
                        return "Penalties";
                    case MatchStatus.Postponed:
                        return "Postponed";
                    case MatchStatus.Cancelled:
                        return "Cancelled";
                    case MatchStatus.Abandoned:
                        return "Abandoned";
                    case MatchStatus.TechnicalLoss:
                        return "Technical loss";
                    case MatchStatus.WalkOver:
                        return "Walk over";
                    case MatchStatus.Live:
                        return "Live";
                    default:
                        return "Unknown";
                }
            }
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
