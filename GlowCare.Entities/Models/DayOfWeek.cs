using System.ComponentModel.DataAnnotations;

namespace GlowCare.Entities.Models;

public class DayOfWeek
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public List<ScheduleDayOfWeek> ScheduleDaysOfWeek { get; set; }
        = new List<ScheduleDayOfWeek>();
}

