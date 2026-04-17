using GlowCare.Entities.Models.Enums;

namespace GlowCare.ViewModels.SpecialistRequest;

public class SpecialistApplicationViewModel
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string UserFullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Occupation { get; set; } = null!;

    public int ExperienceYears { get; set; }

    public string? Biography { get; set; }

    public RequestStatus Status { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? RejectionReason { get; set; }
}
