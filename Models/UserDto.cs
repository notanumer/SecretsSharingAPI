using System.ComponentModel.DataAnnotations;

namespace SecretsSharingAPI.Models
{
    public class UserDto
    {
        [EmailAddress(ErrorMessage="Please, input a correct email")]
        [Required(ErrorMessage="Please, input your email")]
        public string Email { get; set; }

        [Required(ErrorMessage="Please, input your password")]
        public string Password { get; set; }
    }
}
