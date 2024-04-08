using FootballResults.Models.Football;
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

        public ICollection<League> FavoriteLeagues { get; set; }
        public ICollection<Team> FavoriteTeams { get; set; }
        public ICollection<Message> Messages { get; set; }
        public IEnumerable<PredictionGame> Games { get; set; }
        public ICollection<Prediction> Predictions { get; set; }
        public IEnumerable<GameStanding> Standings { get; set; }

        // skip navigations
        public IEnumerable<Participation> Participations { get; set; }
        public ICollection<FavoriteLeague> UserLeagues { get; set; }
        public ICollection<FavoriteTeam> UserTeams { get; set; }
    }
}
