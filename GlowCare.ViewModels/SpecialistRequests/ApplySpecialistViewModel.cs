using System.ComponentModel.DataAnnotations;

namespace GlowCare.ViewModels.SpecialistRequest;

public class ApplySpecialistViewModel
{
    [Required(ErrorMessage = "Полето „Професия“ е задължително.")]
    public string Occupation { get; set; } = null!;

    [Required(ErrorMessage = "Полето „Години опит“ е задължително.")]
    [Range(0, 100, ErrorMessage = "Годините опит трябва да са между 0 и 100.")]
    public int ExperienceYears { get; set; }

    [StringLength(1000, ErrorMessage = "Биографията не може да бъде по-дълга от 1000 символа.")]
    public string? Biography { get; set; }
}
