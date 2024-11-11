using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FootballResults.Models.ViewModels.Users
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [PasswordPropertyText]
        public string Password { get; set; }

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
                    InvalidCredentialsError = false;
                }

                _error = value;
            }
        }

        public void ResetMessages()
        {
            Success = false;
            Error = false;
        }

        public bool InvalidCredentialsError { get; set; }

        public bool UserNotFoundError { get; set; }
        public bool InvalidPasswordError { get; set; }
    }
}
