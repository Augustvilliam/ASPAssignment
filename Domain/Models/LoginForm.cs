
using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class LoginForm
{
    [Required]
    [Display(Name = "Email", Prompt = "Enter email Adress!")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;
    [Required]
    [Display(Name = "Password", Prompt = "Enter your Password!")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

}
