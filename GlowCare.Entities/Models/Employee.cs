using System.ComponentModel.DataAnnotations;

namespace GlowCare.Entities.Models;

public class Employee : GlowUser
{
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

