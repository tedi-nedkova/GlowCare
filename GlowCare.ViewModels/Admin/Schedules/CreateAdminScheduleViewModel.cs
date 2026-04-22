using System.ComponentModel.DataAnnotations;
using GlowCare.ViewModels.Common;

namespace GlowCare.ViewModels.Admin.Schedules;

public class CreateAdminScheduleViewModel
{
    [Display(Name = "Специалист")]
    [Required(ErrorMessage = "Моля, изберете специалист.")]
    public Guid? EmployeeId { get; set; }

    [Display(Name = "Ден от седмицата")]
    [Required(ErrorMessage = "Моля, изберете ден.")]
    public DayOfWeek? DayOfWeek { get; set; }

    [Display(Name = "Начален час")]
    [Required(ErrorMessage = "Моля, въведете начален час.")]
    [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "Часът трябва да е във формат HH:mm.")]
    public string StartTime { get; set; } = string.Empty;

    [Display(Name = "Краен час")]
    [Required(ErrorMessage = "Моля, въведете краен час.")]
    [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "Часът трябва да е във формат HH:mm.")]
    public string EndTime { get; set; } = string.Empty;

    public IEnumerable<DropdownItemViewModel> Specialists { get; set; }
        = Enumerable.Empty<DropdownItemViewModel>();

    public IEnumerable<DropdownItemViewModel> Days { get; set; }
        = Enumerable.Empty<DropdownItemViewModel>();
}