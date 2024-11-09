using FootballResults.DataAccess.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Football
{
    public class Team : EntityWithID, IBookmarkable
    {
        /// <summary>
        /// ID of the country the team is from
        /// </summary>
        public int CountryID { get; set; }

        /// <summary>
        /// ID of the venue (stadium) the team plays at
        /// </summary>
        public int? VenueID { get; set; }

        /// <summary>
        /// Name of the team
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Short name of the team
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// A link pointing to an image of the team's logo
        /// </summary>
        public string LogoLink { get; set; }

        /// <summary>
        /// Path pointing to an image of the team's logo
        /// </summary>
        public string LogoPath { get; set; }

        /// <summary>
        /// Whether the team is a national team
        /// </summary>
        public bool National { get; set; }

        /// <summary>
        /// Last time the squad was updated
        /// </summary>
        public DateTime? SquadLastUpdate { get; set; }

        /// <summary>
        /// Country of the team
        /// </summary>
        public Country Country { get; set; }

        /// <summary>
        /// Venue of the team
        /// </summary>
        public Venue Venue { get; set; }

        /// <summary>
        /// Matches played by the team at their venue
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Match> HomeMatches { get; set; }

        /// <summary>
        /// Matches played by the team away
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Match> AwayMatches { get; set; }

        /// <summary>
        /// All matches played by the team
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public IEnumerable<Match> Matches => HomeMatches.Concat(AwayMatches);

        /// <summary>
        /// Standing of the team in any league
        /// </summary>
        [JsonIgnore]
        public IEnumerable<LeagueStanding> Standings { get; set; }

        /// <summary>
        /// Topscorers of the team in any league
        /// </summary>
        [JsonIgnore]
        public IEnumerable<TopScorer> TopScorers { get; set; }

        /// <summary>
        /// Players of the team
        /// </summary>
        public IEnumerable<Player> Squad { get; set; }

        /// <summary>
        /// Bookmark ID for this team (equals to TeamID)
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public int BookmarkID => ID;

        /// <summary>
        /// Users who marked this team as their favorite
        /// </summary>
        [JsonIgnore]
        public IEnumerable<User> UsersWhoBookmarked { get; set; }

        // skip navigations
        [JsonIgnore]
        public IEnumerable<FavoriteTeam> FavoriteTeams { get; set; }

        public bool Equals(Team team)
        {
            return ID == team.ID && CountryID == team.CountryID && VenueID == team.VenueID
                && Name == team.Name && ShortName == team.ShortName && LogoLink == team.LogoLink && National == team.National;
        }

        public void CopyFrom(Team other)
        {
            ID = other.ID;
            CountryID = other.CountryID;
            VenueID = other.VenueID;
            Name = other.Name;
            ShortName = other.ShortName;
            LogoLink = other.LogoLink;
            National = other.National;
        }
    }
}
