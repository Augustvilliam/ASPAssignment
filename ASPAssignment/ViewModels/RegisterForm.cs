﻿
using System.ComponentModel.DataAnnotations;


namespace ASPAssignment.ViewModels;

public class RegisterForm //lite striktare med validering här eftersom det är vid registrering. 
{
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "First Name", Prompt = "Enter First Name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Last Name", Prompt = "Enter Last Name")]
    public string LastName { get; set; } = null!;

    [Required]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Invalid email address. Use format name@example.com")]
    [RegularExpression(@"^[\w\.-]+@[\w\.-]+\.\w{2,4}$", ErrorMessage = "Invalid email address. Use format name@example.com")]
    [Display(Name = "Email", Prompt = "Enter Email Address")]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter Password")]
    public string Password { get; set; } = null!;

    [Compare(nameof(Password), ErrorMessage = "Passwords Do Not Match!")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password", Prompt = "Confirm Password")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "You must accept the terms and conditions.")]
    [Display(Name = "I accept terms and conditions")]
    public bool Terms { get; set; }

}
