using System.ComponentModel.DataAnnotations;

namespace GlowCare.ViewModels.SpecialistRequest;

public class ReviewApplicationViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Моля, въведете причина за отхвърляне.")]
    [StringLength(1000, ErrorMessage = "Причината за отхвърляне не може да е по-дълга от 1000 символа.")]
    public string? RejectionReason { get; set; }
}
