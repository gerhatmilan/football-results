namespace FootballResults.Models.ViewModels.Application
{
    public class ApplicationSettingsFormModel
    {
        private string _apiKeyBoundValue;
        public string ApiKeyBoundValue
        {
            get => _apiKeyBoundValue;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _apiKeyBoundValue = null;
                }
                else
                {
                    _apiKeyBoundValue = value;
                }
            }
        }

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

        private bool _error;
        public bool Error
        {
            get => _error;
            set
            {
                if (value)
                {
                    _success = false;
                }

                _error = value;
            }
        }

        public void ResetMessages()
        {
            Error = Success = false;
        }
    }
}
