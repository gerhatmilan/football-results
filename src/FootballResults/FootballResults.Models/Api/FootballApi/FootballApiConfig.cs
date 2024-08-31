namespace FootballResults.Models.Api.FootballApi
{
    public class FootballApiConfig
    {
        public string BaseAddressHeaderKey { get; set; }
        public string BaseAddress { get; set; }
        public string ApiKeyHeaderKey { get; set; }
        public string ApiKey { get; set; }
        public int RateLimit { get; set; }
        public DataFetch DataFetch { get; set; }

        public IEnumerable<IncludedLeagueRecord> IncludedLeagues { get; set; }

        public Dictionary<string, string> RequestHeaders => new Dictionary<string, string>
        {
            { BaseAddressHeaderKey, BaseAddress },
            { ApiKeyHeaderKey, ApiKey }
        };
    }

    public class DataFetch
    {
        public bool ShouldBackupData { get; set; }
        public UpdaterSpecificSettings Countries { get; set; }
        public UpdaterSpecificSettings Leagues { get; set; }
        public UpdaterSpecificSettings Venues { get; set; }
        public UpdaterSpecificSettings TeamsForLeagueAndSeason { get; set; }
        public UpdaterSpecificSettings SquadForTeam { get; set; }
        public UpdaterSpecificSettings MatchesForLeagueAndSeason { get; set; }
        public UpdaterSpecificSettings MatchesForDate { get; set; }
        public UpdaterSpecificSettings StandingsForLeagueAndSeason { get; set; }
        public UpdaterSpecificSettings TopScorersForLeagueAndSeason { get; set; }
    }

    public class UpdaterSpecificSettings
    {
        public string Endpoint { get; set; }
        public string BackupPath { get; set; }
        public bool LoadDataFromBackup { get; set; }
    }

    public class IncludedLeagueRecord
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
