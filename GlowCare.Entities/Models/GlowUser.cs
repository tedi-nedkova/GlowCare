using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GlowCare.Entities.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace GlowCare.Entities.Models;

public class GlowUser 
    : IdentityUser
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    public int Age { get; set; }
    public Gender Gender { get; set; }
}

