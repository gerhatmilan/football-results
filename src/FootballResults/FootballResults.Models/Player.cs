using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballResults.Models
{
    public class Player
    {
        public int PlayerID { get; set; }
        public int TeamID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Number { get; set; }
        public string Position { get; set; }
        public string PhotoLink { get; set; }
        public Team Team { get; set; }
    }
}
