using System.ComponentModel.DataAnnotations;

namespace ASPAssignment.ViewModels
{
    public class AdminLoginForm //för admins inloggningsformulär
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
