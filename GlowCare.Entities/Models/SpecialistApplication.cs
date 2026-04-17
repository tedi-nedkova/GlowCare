using GlowCare.Entities.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlowCare.Entities.Models;

public class SpecialistApplication
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public GlowUser User { get; set; } = null!;

    [Required]
    public string Occupation { get; set; } = null!;

    [Required]
    [Range(0, 60)]
    public int ExperienceYears { get; set; }

    public string? Biography { get; set; }

    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    [Required]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public string? RejectionReason { get; set; }
}