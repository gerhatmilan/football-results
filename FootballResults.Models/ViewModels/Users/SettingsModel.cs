using System.ComponentModel.DataAnnotations;

namespace FootballResults.Models.ViewModels.Users
{
    public class SettingsModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        public string Username { get; set; }

        public string ImagePath { get; set; }

        private bool _success;
        private bool _error;
        private bool _imageError;
        private string _imageErrorMessage;

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
                    UsernameAlreadyInUseError = false;
                    ImageError = false;
                }

                _error = value;
            }
        }
        public bool UsernameAlreadyInUseError { get; set; }

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
            Success = false;
            Error = false;
        }
    }
}
