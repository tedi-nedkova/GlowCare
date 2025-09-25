using System.ComponentModel.DataAnnotations;

namespace GlowCare.Entities.Models;

public class Schedule
{
    [Key]
    [Required]
    public int Id { get; set; }

    public List<ScheduleDayOfWeek> ScheduleDaysOfWeek { get; set; } 
        = new List<ScheduleDayOfWeek>();

    [Required]
    public string StartTime { get; set; } = null!;

    [Required]
    public string EndTime { get; set; } = null!;

    public ICollection<EmployeeSchedule> EmployeesSchedules { get; set; }
            = new List<EmployeeSchedule>();
}

