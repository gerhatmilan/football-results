using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Football
{
    public class Player : EntityWithID
    {
        /// <summary>
        /// ID of the player
        /// </summary>
        public int PlayerID { get; set; }

        /// <summary>
        /// ID of the team of the player
        /// </summary>
        public int? TeamID { get; set; }

        /// <summary>
        /// Name of the player
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Age of the player
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Shirt number of the player
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// Position the player plays in
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// A link pointing to an image of the player
        /// </summary>
        public string PhotoLink { get; set; }

        /// <summary>
        /// Team of the player
        /// </summary>
        [JsonIgnore]
        public Team Team { get; set; }

        public bool Equals(Player player)
        {
            return PlayerID == player.PlayerID
                && TeamID == player.TeamID
                && Name == player.Name
                && Age == player.Age
                && Number == player.Number
                && Position == player.Position
                && PhotoLink == player.PhotoLink;
        }

        public void CopyFrom(Player other)
        {
            PlayerID = other.PlayerID;
            TeamID = other.TeamID;
            Name = other.Name;
            Age = other.Age;
            Number = other.Number;
            Position = other.Position;
            PhotoLink = other.PhotoLink;
        }
    }
}
