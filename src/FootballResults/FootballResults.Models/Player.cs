using System.Text.Json.Serialization;

namespace FootballResults.Models
{
    public class Player
    {
        public int PlayerID { get; set; }

        public int TeamID { get; set; }

        public string Name { get; set; }

        public int? Age { get; set; }

        public int? Number { get; set; }

        public string Position { get; set; }

        public string PhotoLink { get; set; }

        [JsonIgnore]
        public Team Team { get; set; }
    }
}
