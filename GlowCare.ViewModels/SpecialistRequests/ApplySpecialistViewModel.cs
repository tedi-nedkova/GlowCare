using System.ComponentModel.DataAnnotations;

namespace GlowCare.ViewModels.SpecialistRequest;

public class ApplySpecialistViewModel
{
    [Required]
    public string Occupation { get; set; } = null!;

    [Required]
    public int ExperienceYears { get; set; }

    public string? Biography { get; set; }
}

