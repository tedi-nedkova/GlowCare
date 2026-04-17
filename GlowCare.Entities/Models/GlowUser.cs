using GlowCare.Entities.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static GlowCare.Common.Constants.GlowUserConstants;

namespace GlowCare.Entities.Models;

public class GlowUser 
    : IdentityUser<Guid>
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

    public ICollection<Review> Reviews { get; set; }
    = new List<Review>();

    public int? MembershipId { get; set; }
    [ForeignKey(nameof(MembershipId))]
    public Membership? Membership { get; set; }

    public ICollection<Procedure> Procedures { get; set; }
        = new List<Procedure>();

    public bool IsSpecialist { get; set; }

    public bool IsDeleted { get; set; } = false;

    public ICollection<SpecialistApplication> SpecialistApplications { get; set; }
    = new List<SpecialistApplication>();
}

