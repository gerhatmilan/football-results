using FootballResults.Models.Predictions;

namespace FootballResults.Models.Users
{
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProfilePicturePath { get; set; }
        public DateTime? RegistrataionDate { get; set; }

        public IEnumerable<FavoriteLeague> FavoriteLeagues { get; set; }
        public IEnumerable<FavoriteTeam> FavoriteTeams { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public IEnumerable<Prediction> Predictions { get; set; }
        public IEnumerable<Participation> Participations { get; set; }
        public IEnumerable<PredictionGame> Games { get; set; }
    }
}
