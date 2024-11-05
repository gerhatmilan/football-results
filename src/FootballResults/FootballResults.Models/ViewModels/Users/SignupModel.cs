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
    }
}
