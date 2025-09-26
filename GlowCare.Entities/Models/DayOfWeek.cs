using System.ComponentModel.DataAnnotations;
using static GlowCare.Common.Constants.DayOfWeekConstants;

namespace GlowCare.Entities.Models;

public class DayOfWeek
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    [MinLength(NameMinLength)]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    public List<ScheduleDayOfWeek> ScheduleDaysOfWeek { get; set; }
        = new List<ScheduleDayOfWeek>();
}

