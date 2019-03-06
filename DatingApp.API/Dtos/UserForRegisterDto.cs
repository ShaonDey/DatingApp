using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Password must be between 3 to 10 characters")]
        public string Password { get; set; }
    }
}