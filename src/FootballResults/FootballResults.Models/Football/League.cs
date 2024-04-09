using FootballResults.Models.Predictions;
using FootballResults.Models.Users;
using System.Text.Json.Serialization;

namespace FootballResults.Models.Football
{
    public class League : IBookmarkable
    {
        public int LeagueID { get; set; }

        public string CountryID { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int CurrentSeason { get; set; }

        public string LogoLink { get; set; }

        public Country Country { get; set; }

        public ICollection<AvailableSeason> AvailableSeasons { get; set; }

        public ICollection<Match> Matches { get; set; }

        public ICollection<LeagueStanding> Standings { get; set; }

        public ICollection<TopScorer> TopScorers { get; set; }

        [JsonIgnore]
        public int BookmarkID { get => LeagueID; }

        [JsonIgnore]
        public IEnumerable<User> UsersWhoBookmarked { get; set; }

        [JsonIgnore]
        public IEnumerable<PredictionGame> GamesWhereIncluded { get; set; }
        
        // skip navigations
        [JsonIgnore]
        public IEnumerable<FavoriteLeague> UserLeagues { get; set; }

        [JsonIgnore]
        public IEnumerable<GameLeague> GameLeagues { get; set; }
    }
}