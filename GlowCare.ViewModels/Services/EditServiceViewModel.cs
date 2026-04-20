using System.ComponentModel.DataAnnotations;
using static GlowCare.Common.Constants.ServiceConstants;

namespace GlowCare.ViewModels.Services;

public class EditServiceViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Моля, избери категория.")]
    [Range(1, int.MaxValue, ErrorMessage = "Моля, избери валидна категория.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Името на услугата е задължително.")]
    [MinLength(NameMinLength, ErrorMessage = "Името на услугата е твърде кратко.")]
    [MaxLength(NameMaxLength, ErrorMessage = "Името на услугата е твърде дълго.")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Range(MinDurationInMinutes, MaxDurationInMinutes, ErrorMessage = "Продължителността трябва да е в допустимите граници.")]
    public int DurationInMinutes { get; set; }

    [Range(MinPrice, MaxPrice, ErrorMessage = "Цената трябва да е в допустимите граници.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Точките не могат да бъдат отрицателни.")]
    public int Points { get; set; }

    public ICollection<Guid> SelectedEmployeeIds { get; set; }
        = new List<Guid>();

    public IEnumerable<ServiceCategoryOptionViewModel> Categories { get; set; }
        = new List<ServiceCategoryOptionViewModel>();

    public IEnumerable<SpecialistOptionViewModel> Specialists { get; set; }
        = new List<SpecialistOptionViewModel>();
}
