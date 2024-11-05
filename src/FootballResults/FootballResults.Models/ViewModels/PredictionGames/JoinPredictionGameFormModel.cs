using System.ComponentModel.DataAnnotations;

namespace FootballResults.Models.ViewModels.PredictionGames
{
    public class JoinPredictionGameFormModel
    {
        [Required(ErrorMessage = "Please provide a valid key")]
        public string JoinKey { get; set; }
    }
}
