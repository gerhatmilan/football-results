using System.ComponentModel.DataAnnotations;

namespace FootballResults.Models.Users
{
    public class SettingsModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        public string Username { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}
