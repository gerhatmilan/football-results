using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResults.Models
{
    public class AvailableSeason
    {
        public int LeagueID { get; set; }

        public int Season { get; set; }

        public League League { get; set; }
    }
}