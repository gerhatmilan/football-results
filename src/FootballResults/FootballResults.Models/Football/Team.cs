using FootballResults.Models.Users;
using System.Text.Json.Serialization;

namespace FootballResults.Models.Football
{
    public class Team : IBookmarkable
    {
        public int TeamID { get; set; }

        public string CountryID { get; set; } 
        
        public int? VenueID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string LogoLink { get; set; }

        public bool National { get; set; }

        public Country Country { get; set; }

        public Venue Venue { get; set; }

        [JsonIgnore]
        public ICollection<Match> HomeMatches { get; set; }

        [JsonIgnore]
        public ICollection<Match> AwayMatches { get; set; }

        public ICollection<Player> Squad { get; set; }

        [JsonIgnore]
        public int BookmarkID { get => TeamID; }

        [JsonIgnore]
        public IEnumerable<User> UsersWhoBookmarked { get; set; }

        // skip navigations
        [JsonIgnore]
        public IEnumerable<FavoriteTeam> UserTeams { get; set; }
    }
}
