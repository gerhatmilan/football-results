using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FootballResults.DataAccess.Entities.Football
{
    public class League : EntityWithID, IBookmarkable
    {
        private LeagueSeason _currentSeason;

        /// <summary>
        /// ID of the country the league is in
        /// </summary>
        public int? CountryID { get; set; }

        /// <summary>
        /// Name of the league
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the league (e. g. League, Cup)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A link pointing to an image of the league's logo
        /// </summary>
        public string LogoLink { get; set; }

        /// <summary>
        /// Path pointing to an image of the league's logo
        /// </summary>
        public string LogoPath { get; set; }

        /// <summary>
        /// Bookmark ID for this league (equals to ID)
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public int BookmarkID => ID;

        /// <summary>
        /// Country the league is in
        /// </summary>
        [JsonIgnore]
        public Country Country { get; set; }

        /// <summary>
        /// Season that is currently in progress
        /// </summary>
        [NotMapped]
        public LeagueSeason CurrentSeason { get => _currentSeason ?? LeagueSeasons?.FirstOrDefault(ls => ls.InProgress); set => _currentSeason = value; }

        /// <summary>
        /// Users who marked this league as their favorite
        /// </summary>
        [JsonIgnore]
        public IEnumerable<User> UsersWhoBookmarked { get; set; }

        // skip navigations

        [JsonIgnore]
        public IEnumerable<LeagueSeason> LeagueSeasons { get; set; }

        [JsonIgnore]
        public IEnumerable<FavoriteLeague> FavoriteLeagues { get; set; }

        public bool Equals(League league)
        {
            return ID == league.ID && CountryID == league.CountryID && Name == league.Name
                && Type == league.Type && LogoLink == league.LogoLink;
        }

        public void CopyFrom(League league)
        {
            ID = league.ID;
            CountryID = league.CountryID;
            Name = league.Name;
            Type = league.Type;
            LogoLink = league.LogoLink;
        }
    }
}