namespace FootballResults.Models
{
    public class League : IBookmarkable
    {
        public int LeagueID { get; set; }

        public string CountryID { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int? CurrentSeason { get; set; }

        public string LogoLink { get; set; }

        public Country Country { get; set; }

        public ICollection<AvailableSeason> AvailableSeasons { get; set; }

        public ICollection<Match> Matches { get; set; }

        public ICollection<Standing> Standings { get; set; }

        public ICollection<TopScorer> TopScorers { get; set; }

        public int BookmarkID { get => LeagueID; }
    }
}