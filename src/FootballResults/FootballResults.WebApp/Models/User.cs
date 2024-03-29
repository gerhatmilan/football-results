using FootballResults.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FootballResults.WebApp.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string? Email { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public DateTime? RegistrataionDate { get; set; }

        public IEnumerable<FavoriteLeague>? FavoriteLeagues { get; set; }

        public IEnumerable<FavoriteTeam>? FavoriteTeams { get; set; }

        public IEnumerable<Message>? Messages { get; set; }

        public IEnumerable<PredictionGame>? PredictionGames { get; set; }
    }
}
