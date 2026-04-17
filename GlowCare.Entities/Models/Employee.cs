using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static GlowCare.Common.Constants.EmployeeConstants;

namespace GlowCare.Entities.Models;

public class Employee 
{
    [Key]
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public GlowUser User { get; set; } = null!;

    [Required]
    [MinLength(OccupationMinLength)]
    [MaxLength(OccupationMaxLength)]
    public string Occupation { get; set; } = null!;

    [Required]
    [Range(MinExperienceYears, MaxExperienceYears)]
    public int ExperienceYears { get; set; }

    public string? Biography { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<Certificate> Certificates { get; set; }
        = new List<Certificate>();

    public ICollection<Procedure> Procedures { get; set; }
        = new List<Procedure>();

    public ICollection<Review> Reviews { get; set; }
        = new List<Review>();

    public ICollection<Schedule> Schedules { get; set; }
        = new List<Schedule>();

    public ICollection<EmployeeService> EmployeeServices { get; set; }
        = new List<EmployeeService>();
}

