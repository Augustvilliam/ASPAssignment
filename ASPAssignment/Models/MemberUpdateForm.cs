using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ASPAssignment.Models;

public class MemberUpdateForm
{
    public string Id { get; set; } = null!;
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    public string? JobTitle { get; set; }
    [Required]
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public IFormFile? ProfilePic { get; set; }
    public string? ExistingProfileImagePath { get; set; }

}
