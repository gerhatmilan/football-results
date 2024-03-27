using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FootballResults.WebApp.Models
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [PasswordPropertyText]
        public string? Password { get; set; }
    }
}
