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

        public string PredictionGamePicturesDirectory { get; set; }

        public string ProfilePicturesDirectory { get; set; }

        public string CountriesDirectory { get; set; }

        public string LeaguesDirectory { get; set; }

        public string TeamsDirectory { get; set; }

        public string PlayersDirectory { get; set; }

        public string PredictionGameDefaultImage { get; set; }
    }
}
