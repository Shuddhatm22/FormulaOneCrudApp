using System.ComponentModel.DataAnnotations;

namespace FormulaOneApp.Models.Dtos
{
    public class UserLoginRequestDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "invalid email address string")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
