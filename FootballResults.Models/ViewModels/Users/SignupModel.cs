using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FootballResults.Models.ViewModels.Users
{
    public class SignupModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
        [MaxLength(20, ErrorMessage = "Username must be maximum 20 characters long")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters long")]
        [MaxLength(50, ErrorMessage = "Password must be maximum 50 characters long")]
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
                    EmailAlreadyInUseError = false;
                    UsernameAlreadyInUseError = false;
                }

                _error = value;
            }
        }

        public bool EmailAlreadyInUseError { get; set; }
        public bool UsernameAlreadyInUseError { get; set; }

        public void ResetMessages()
        {
            Success = false;
            Error = false;
        }
    }
}
