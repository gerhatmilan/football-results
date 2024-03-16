using System.Text.Json.Serialization;

namespace FootballResults.Models
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

        public int BookmarkID { get => TeamID; }
    }
}
