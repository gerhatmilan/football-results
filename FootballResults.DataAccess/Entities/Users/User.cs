using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Predictions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Users
{
    public class User : EntityWithID
    {
        /// <summary>
        /// Email of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Username of the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password of the user (encrypted)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// A file path to the profile picture of the user
        /// </summary>
        public string ProfilePicturePath { get; set; }

        /// <summary>
        /// Date and time when the user registered
        /// </summary>
        public DateTime? RegistrataionDate { get; set; }

        /// <summary>
        /// A flag whether the user is an admin user
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Leagues the user marked as favorite
        /// </summary>
        public IEnumerable<League> FavoriteLeagues { get; set; }

        /// <summary>
        /// Teams the user marked as favorite
        /// </summary>
        public IEnumerable<Team> FavoriteTeams { get; set; }

        /// <summary>
        /// Messages sent by the user
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Message> Messages { get; set; }

        /// <summary>
        /// Prediction games the user is participating in
        /// </summary>
        [JsonIgnore]
        public IEnumerable<PredictionGame> PredictionGames { get; set; }

        /// <summary>
        /// Prediction games created by the user
        /// </summary>
        [JsonIgnore]
        public IEnumerable<PredictionGame> CreatedPredictionGames { get; set; }

        /// <summary>
        /// Predictions made by the user
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public IEnumerable<Prediction> Predictions { get => Participations?.SelectMany(p => p.Predictions); set { } }

        /// <summary>
        /// Standings of the user in the prediction games
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public IEnumerable<PredictionGameStanding> Standings { get => Participations?.Select(p => p.Standing); set { } }

        // skip navigations
        [JsonIgnore]
        public IEnumerable<Participation> Participations { get; set; }

        [JsonIgnore]
        public IEnumerable<FavoriteLeague> UserLeagues { get; set; }

        [JsonIgnore]
        public IEnumerable<FavoriteTeam> UserTeams { get; set; }
    }
}
