
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FootballResults.Models.ViewModels.Application
{
    public class UpdaterRunnerFormModel
    {
        public bool ModeSelected { get; set; }
        public int? ModeParameterInteger { get; set; }
        public DateTime? ModeParameterDateTime { get; set; } = DateTime.Now;
        public TimeSpan ModeParameterTimeSpan { get; set; }

        private bool _error;
        private bool _success;

        public bool Success
        {
            get => _success;
            set
            {
                if (_success)
                {
                    Error = false;
                }

                _success = value;
            }
        }
        public bool Error
        {
            get => _error;
            set
            {
                if (value)
                {
                    _success = false;
                }
                else
                {
                    ErrorMessage = string.Empty;
                }

                _error = value;
            }
        }

        public string ErrorMessage { get; set; }

        public void ResetMessages()
        {
            Error = false;
            Success = false;
        }
    }
}
