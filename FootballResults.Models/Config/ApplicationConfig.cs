using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace FootballResults.Models.Config
{
    public class ApplicationConfig
    {
        private string predictionGamePicturesDirectory;
        private string profilePicturesDirectory;
        private string countriesDirectory;
        private string leaguesDirectory;
        private string teamsDirectory;
        private string playersDirectory;
        private string predictionGameDefaultImage;

        public TimeSpan UpdaterWorkerFrequency { get; set; }
        public TimeSpan MatchesUpdateForCurrentDayFrequency { get; set; }
        public TimeSpan MatchesUpdateForCurrentSeasonFrequency { get; set; }
        public TimeSpan StandingsUpdateForCurrentSeasonFrequency { get; set; }
        public TimeSpan TopScorersUpdateForCurrentSeasonFrequency { get; set; }
        public TimeSpan ImageDownloaderWorkerFrequency { get; set; }
        public TimeSpan ImageDownloaderFrequency { get; set; }

        public string PredictionGamePicturesDirectory { get => GetPath(predictionGamePicturesDirectory); set => predictionGamePicturesDirectory = value; }
        public string ProfilePicturesDirectory { get => GetPath(profilePicturesDirectory); set => profilePicturesDirectory = value; }
        public string CountriesDirectory { get => GetPath(countriesDirectory); set => countriesDirectory = value; }
        public string LeaguesDirectory { get => GetPath(leaguesDirectory); set => leaguesDirectory = value; }
        public string TeamsDirectory { get => GetPath(teamsDirectory); set => teamsDirectory = value; }
        public string PlayersDirectory { get => GetPath(playersDirectory); set => playersDirectory = value; }
        public string PredictionGameDefaultImage { get => GetPath(predictionGameDefaultImage); set => predictionGameDefaultImage = value; }

        public IEnumerable<IncludedLeagueRecord> IncludedLeagues { get; set; }

        private static string GetPath(string configPath)
        {
            return Path.Combine(configPath.Split('/'));
        }
    }

    public class IncludedLeagueRecord
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
