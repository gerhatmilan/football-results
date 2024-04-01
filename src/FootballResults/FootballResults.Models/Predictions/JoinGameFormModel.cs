using System.ComponentModel.DataAnnotations;

namespace FootballResults.Models.Predictions
{
    public class JoinGameFormModel
    {
        [Required(ErrorMessage = "Please provide a valid key")]
        public string? JoinKey { get; set; }
    }
}
