using FootballResults.DataAccess.Entities.Predictions;
using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Football
{
    public class LeagueSeason : EntityWithID
    {
        /// <summary>
        /// ID of the league for this league season
        /// </summary>
        public int LeagueID { get; set; }

        /// <summary>
        /// Year of the league season
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Value indicating whether the league season is in progress
        /// </summary>
        public bool InProgress { get; set; }

        /// <summary>
        /// Last time the standings were updated for this league season
        /// </summary>
        public DateTime? StandingsLastUpdate { get; set; }

        /// <summary>
        /// Last time the top scorers were updated for this league season
        /// </summary>
        public DateTime? TopScorersLastUpdate { get; set; }

        /// <summary>
        /// Last time the matches were updated for this league season
        /// </summary>
        public DateTime? MatchesLastUpdate { get; set; }

        /// <summary>
        /// League for the league season
        /// </summary>
        public League League { get; set; }

        /// <summary>
        /// Matches of the league season
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Match> Matches { get; set; }

        /// <summary>
        /// Standings of the league season
        /// </summary>
        [JsonIgnore]
        public IEnumerable<LeagueStanding> Standings { get; set; }

        /// <summary>
        /// Top scorers of the league season
        /// </summary>
        [JsonIgnore]
        public IEnumerable<TopScorer> TopScorers { get; set; }

        /// <summary>
        /// Prediction games where the league season was included
        /// </summary>
        [JsonIgnore]
        public IEnumerable<PredictionGame> PredictionGames { get; set; }

        // skip navigations

        [JsonIgnore]
        public IEnumerable<PredictionGameSeason> PredictionGameSeasons { get; set; }

        public bool Equals(LeagueSeason leagueSeason)
        {
            return LeagueID == leagueSeason.LeagueID &&
                InProgress == leagueSeason.InProgress &&
                Year == leagueSeason.Year;
        }

        public void CopyFrom(LeagueSeason leagueSeason)
        {
            LeagueID = leagueSeason.LeagueID;
            InProgress = leagueSeason.InProgress;
            Year = leagueSeason.Year;
        }
    }
}
