namespace FootballResults.DataAccess.Entities
{
    public class SystemInformation : EntityWithID
    {
        public DateTime? MatchesLastUpdateForCurrentDay { get; set; }

        public DateTime? CountryFlagsLastDownload { get; set; }

        public DateTime? LeagueLogosLastDownload { get; set; }

        public DateTime? TeamLogosLastDownload { get; set; }

        public DateTime? PlayerPhotosLastDownload { get; set; }

        public DateTime? TopScorerPhotosLastDownload { get; set; }
    }
}
