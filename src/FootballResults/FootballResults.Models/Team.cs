using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballResults.Models
{
    public class Team
    {
        public int TeamID { get; set; }
        public string CountryID { get; set; }  
        public int VenueID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string LogoLink { get; set; }
        public bool National { get; set; }
        public Country Country { get; set; }
        public Venue Venue { get; set; }
        public ICollection<Match> Matches { get; set; }
        public ICollection<Player> Squad { get; set; }
    }
}
