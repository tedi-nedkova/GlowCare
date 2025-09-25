using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace GlowCare.Entities.Models;

public class Employee 
{
    [Key]
    [Required]
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;
    [ForeignKey(nameof(UserId))]
    public GlowUser User { get; set; } = null!;

    [Required]
    public string Occupation { get; set; } = null!;

    [Required]
    public int ExperienceYears { get; set; }

    public string? Biography { get; set; }

    public ICollection<Certificate> Certificates { get; set; }
        = new List<Certificate>();

    public ICollection<Procedure> Procedures { get; set; }
        = new List<Procedure>();

    public ICollection<Review> Reviews { get; set; }
        = new List<Review>();

}

