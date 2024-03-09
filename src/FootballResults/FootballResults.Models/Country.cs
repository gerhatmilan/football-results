using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FootballResults.Models
{
    public class Country
    {
        public string CountryID { get; set; }

        public string FlagLink { get; set; }

        public ICollection<League> Leagues { get; set; }

        public ICollection<Team> Teams { get; set; }

        public ICollection<Venue> Venues { get; set; }

    }
}