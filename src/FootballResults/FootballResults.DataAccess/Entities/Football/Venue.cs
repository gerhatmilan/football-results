using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Football
{
    public class Venue : EntityWithID
    {
        /// <summary>
        /// ID of the country where the venue is located
        /// </summary>
        public int CountryID { get; set; }

        /// <summary>
        /// City where the venue is located
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Name of the venue
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Country of the venue
        /// </summary>
        public Country Country { get; set; }

        /// <summary>
        /// Matches played at this venue
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Match> Matches { get; set; }

        /// <summary>
        /// Teams whose home venue is this venue
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Team> Teams { get; set; }

        public bool Equals(Venue venue)
        {
            return ID == venue.ID &&
                City == venue.City &&
                Name == venue.Name;
        }

        public void CopyFrom(Venue venue)
        {
            ID = venue.ID;
            City = venue.City;
            Name = venue.Name;
        }
    }
}