using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static GlowCare.Common.Constants.ScheduleConstants;

namespace GlowCare.Entities.Models;

public class Schedule
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    [MinLength(MinTime)]
    [MaxLength(MaxTime)]
    public string StartTime { get; set; } = null!;

    [Required]
    [MinLength(MinTime)]
    [MaxLength(MaxTime)]
    public string EndTime { get; set; } = null!;

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    public Guid EmployeeId { get; set; }
    [ForeignKey(nameof(EmployeeId))]
    public Employee? Employee { get; set; }
}

