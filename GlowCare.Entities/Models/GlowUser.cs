using System.ComponentModel.DataAnnotations;
using GlowCare.Entities.Models.Enums;
using Microsoft.AspNetCore.Identity;
using static GlowCare.Common.Constants.GlowUserConstants;

namespace GlowCare.Entities.Models;

public class GlowUser 
    : IdentityUser
{
    [Required]
    [MinLength(UserFirstNameMinLength)]
    [MaxLength(UserLastNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MinLength(UserLastNameMinLength)]
    [MaxLength(UserLastNameMaxLength)]
    public string LastName { get; set; } = null!;

    public int Age { get; set; }
    public Gender Gender { get; set; }
}

