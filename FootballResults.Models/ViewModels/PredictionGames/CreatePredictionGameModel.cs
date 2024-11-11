using FootballResults.DataAccess.Entities.Football;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FootballResults.Models.ViewModels.PredictionGames
{
    public class CreatePredictionGameModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        [MinLength(5, ErrorMessage = "Name must be at least 5 characters long")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string PicturePath { get; set; }

        public ICollection<IncludedLeague> IncludedLeagues { get; set; }

        [Required(ErrorMessage = "Exact scoreline reward must be provided")]
        [Range(1, int.MaxValue, ErrorMessage = "Exact scoreline reward must be a positive number")]
        public int ExactScorelineReward { get; set; } = 10;

        [Required(ErrorMessage = "Outcome reward must be provided")]
        [Range(1, int.MaxValue, ErrorMessage = "Outcome reward must be a positive number")]
        public int OutcomeReward { get; set; } = 8;

        [Required(ErrorMessage = "Goal count reward must be provided")]
        [Range(1, int.MaxValue, ErrorMessage = "Goal count reward must be a positive number")]
        public int GoalCountReward { get; set; } = 5;

        [Required(ErrorMessage = "Goal difference reward must be provided")]
        [Range(1, int.MaxValue, ErrorMessage = "Goal difference reward must be a positive number")]
        public int GoalDifferenceReward { get; set; } = 3;


        private bool _error;
        private bool _imageError;
        private string _imageErrorMessage;
        private bool _includedLeaguesError;
        private string _includedLeaguesErrorMessage;

        public bool Error
        {
            get => _error;
            set
            {
                if (!value)
                {
                    IncludedLeaguesError = false;
                    ImageError = false;
                }

                _error = value;
            }
        }

        public bool IncludedLeaguesError
        {
            get => _includedLeaguesError;
            set
            {
                if (!value)
                {
                    IncludedLeaguesErrorMessage = string.Empty;
                }

                _includedLeaguesError = value;
            }
        }

        public string IncludedLeaguesErrorMessage
        {
            get => _includedLeaguesErrorMessage;

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    IncludedLeaguesError = true;
                }

                _includedLeaguesErrorMessage = value;
            }
        }

        public bool ImageError
        {
            get => _imageError;
            set
            {
                if (!value)
                {
                    ImageErrorMessage = string.Empty;
                }

                _imageError = value;
            }
        }

        public string ImageErrorMessage
        {
            get => _imageErrorMessage;

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ImageError = true;
                }

                _imageErrorMessage = value;
            }
        }

        public void ResetMessages()
        {
            Error = false;
        }

        public bool ValidateIncludedLeagues()
        {
            // at least one league needs to be selected
            if (!IncludedLeagues.Any(includedLeague => includedLeague.Included))
            {
                IncludedLeaguesErrorMessage = "At least one league needs to be selected";
                return false;
            }
            else if (IncludedLeagues.Any(i => i.Included && i.League.CurrentSeason == null))
            {
                IncludedLeaguesErrorMessage = "One of the selected leagues does not have a season in progress";
                return false;
            }
            else
            {
                IncludedLeaguesError = false;
                return true;
            }
        }
    }
}
