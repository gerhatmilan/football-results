using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FootballResults.Models.ViewModels.PredictionGames
{
    public class JoinPredictionGameFormModel
    {
        [Required(ErrorMessage = "Please provide a valid key")]
        public string JoinKey { get; set; }

        private bool _error;

        public bool Error
        {
            get => _error;
            set
            {
                if (!value)
                {
                    GameNotFoundError = false;
                }

                _error = value;
            }
        }

        public bool GameNotFoundError { get; set; }

        public void ResetMessages()
        {
            Error = false;
        }
    }
}
