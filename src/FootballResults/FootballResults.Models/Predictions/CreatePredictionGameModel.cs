using FootballResults.DataAccess.Entities.Football;
using FootballResults.Models.General;
using System.ComponentModel.DataAnnotations;

namespace FootballResults.Models.Predictions
{
    public class CreatePredictionGameModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        [MinLength(5, ErrorMessage = "Name must be at least 5 characters long")]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string PicturePath { get; set; }

        public ICollection<Pair<League, bool>> IncludedLeagues { get; set; }

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
    }
}
