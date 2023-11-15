using System.ComponentModel.DataAnnotations;

namespace FormulaOneApp.Models.Dtos
{
    public class UserRegistrationRequestDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "invalid email address string")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
