namespace FootballResults.Models.Config
{
    public class ApplicationConfig
    {
        public TimeSpan UpdaterWorkerFrequency { get; set; }
        public TimeSpan MatchesUpdateForCurrentDayFrequency { get; set; }
        public TimeSpan MatchesUpdateForCurrentSeasonFrequency { get; set; }
        public TimeSpan StandingsUpdateForCurrentSeasonFrequency { get; set; }
        public TimeSpan TopScorersUpdateForCurrentSeasonFrequency { get; set; }

        public IEnumerable<IncludedLeagueRecord> IncludedLeagues { get; set; }
    }

    public class IncludedLeagueRecord
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
