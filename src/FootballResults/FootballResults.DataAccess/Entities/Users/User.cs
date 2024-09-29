using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Predictions;
using System.ComponentModel.DataAnnotations.Schema;

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
        public IEnumerable<Message> Messages { get; set; }

        /// <summary>
        /// Prediction games the user is participating in
        /// </summary>
        public IEnumerable<PredictionGame> PredictionGames { get; set; }

        /// <summary>
        /// Prediction games created by the user
        /// </summary>
        public IEnumerable<PredictionGame> CreatedPredictionGames { get; set; }

        /// <summary>
        /// Predictions made by the user
        /// </summary>
        [NotMapped]
        public IEnumerable<Prediction> Predictions { get => Participations?.SelectMany(p => p.Predictions); set { } }

        /// <summary>
        /// Standings of the user in the prediction games
        /// </summary>
        [NotMapped]
        public IEnumerable<Predictions.PredictionGameStanding> Standings { get => Participations?.Select(p => p.Standing); set { } }

        // skip navigations
        public IEnumerable<Participation> Participations { get; set; }
        
        public IEnumerable<FavoriteLeague> UserLeagues { get; set; }
        
        public IEnumerable<FavoriteTeam> UserTeams { get; set; }
    }
}
