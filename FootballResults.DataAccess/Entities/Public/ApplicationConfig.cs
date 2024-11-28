using Extensions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResults.DataAccess.Entities
{
    public class ApplicationConfig : EntityWithID
    {
        public long UpdateWorkerFrequencyTicks { get; set; }

        [NotMapped]
        public TimeSpan UpdateWorkerFrequency
        {
            get
            {
                return TimeSpan.FromTicks(UpdateWorkerFrequencyTicks);
            }
            set
            {
                UpdateWorkerFrequencyTicks = value.Ticks;
            }
        }

        public long MatchUpdateForCurrentDayFrequencyTicks { get; set; }

        [NotMapped]
        public TimeSpan MatchUpdateForCurrentDayFrequency
        {
            get
            {
                return TimeSpan.FromTicks(MatchUpdateForCurrentDayFrequencyTicks);
            }
            set
            {
                MatchUpdateForCurrentDayFrequencyTicks = value.Ticks;
            }
        }

        public long MatchUpdateForCurrentSeasonFrequencyTicks { get; set; }

        [NotMapped]
        public TimeSpan MatchUpdateForCurrentSeasonFrequency
        {
            get
            {
                return TimeSpan.FromTicks(MatchUpdateForCurrentSeasonFrequencyTicks);
            }
            set
            {
                MatchUpdateForCurrentSeasonFrequencyTicks = value.Ticks;
            }
        }

        public long StandingsUpdateForCurrentSeasonFrequencyTicks { get; set; }

        [NotMapped]
        public TimeSpan StandingsUpdateForCurrentSeasonFrequency
        {
            get
            {
                return TimeSpan.FromTicks(StandingsUpdateForCurrentSeasonFrequencyTicks);
            }
            set
            {
                StandingsUpdateForCurrentSeasonFrequencyTicks = value.Ticks;
            }
        }

        public long TopScorersUpdateForCurrentSeasonFrequencyTicks { get; set; }

        [NotMapped]
        public TimeSpan TopScorersUpdateForCurrentSeasonFrequency
        {
            get
            {
                return TimeSpan.FromTicks(TopScorersUpdateForCurrentSeasonFrequencyTicks);
            }
            set
            {
                TopScorersUpdateForCurrentSeasonFrequencyTicks = value.Ticks;
            }
        }

        public long ImageDownloadWorkerFrequencyTicks { get; set; }

        [NotMapped]
        public TimeSpan ImageDownloadWorkerFrequency
        {
            get
            {
                return TimeSpan.FromTicks(ImageDownloadWorkerFrequencyTicks);
            }
            set
            {
                ImageDownloadWorkerFrequencyTicks = value.Ticks;
            }
        }

        public long ImageDownloadFrequencyTicks { get; set; }

        [NotMapped]
        public TimeSpan ImageDownloadFrequency
        {
            get
            {
                return TimeSpan.FromTicks(ImageDownloadFrequencyTicks);
            }
            set
            {
                ImageDownloadFrequencyTicks = value.Ticks;
            }
        }

        private string _predictionGamePicturesDirectory;
        public string PredictionGamePicturesDirectory
        {
            get => FileExtensions.GetNormalizedPath(_predictionGamePicturesDirectory);
            set => _predictionGamePicturesDirectory = FileExtensions.GetNormalizedPath(value);
        }

        private string _profilePicturesDirectory;
        public string ProfilePicturesDirectory
        {
            get => FileExtensions.GetNormalizedPath(_profilePicturesDirectory);
            set => _profilePicturesDirectory = FileExtensions.GetNormalizedPath(value);
        }

        private string _countriesDirectory;
        public string CountriesDirectory
        {
            get => FileExtensions.GetNormalizedPath(_countriesDirectory);
            set => _countriesDirectory = FileExtensions.GetNormalizedPath(value);
        }

        private string _leaguesDirectory;
        public string LeaguesDirectory
        {
            get => FileExtensions.GetNormalizedPath(_leaguesDirectory);
            set => _leaguesDirectory = FileExtensions.GetNormalizedPath(value);
        }

        private string _teamsDirectory;
        public string TeamsDirectory
        {
            get => FileExtensions.GetNormalizedPath(_teamsDirectory);
            set => _teamsDirectory = FileExtensions.GetNormalizedPath(value);
        }

        private string _playersDirectory;
        public string PlayersDirectory
        {
            get => FileExtensions.GetNormalizedPath(_playersDirectory);
            set => _playersDirectory = FileExtensions.GetNormalizedPath(value);
        }

        private string _predictionGameDefaultImage;
        public string PredictionGameDefaultImage
        {
            get => FileExtensions.GetNormalizedPath(_predictionGameDefaultImage);
            set => _predictionGameDefaultImage = FileExtensions.GetNormalizedPath(value);
        }

    }
}
