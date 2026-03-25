using System.ComponentModel.DataAnnotations;
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
    public string DaysOfWeek { get; set; } = null!;

    public ICollection<EmployeeSchedule> EmployeesSchedules { get; set; }
            = new List<EmployeeSchedule>();
}

