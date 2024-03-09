using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResults.Models
{
    public class Venue
    {
        public int VenueID { get; set; }

        public string CountryID { get; set; }

        public string City { get; set; }

        public string Name { get; set; }

        public Country Country { get; set; }
    }
}