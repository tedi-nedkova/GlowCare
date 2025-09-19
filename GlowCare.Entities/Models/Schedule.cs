using System.ComponentModel.DataAnnotations;

namespace GlowCare.Entities.Models;

public class Schedule
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string DayOfWeek { get; set; } = null!;

    [Required]
    public string StartTime { get; set; } = null!;

    [Required]
    public string EndTime { get; set; } = null!;

    public ICollection<EmployeeSchedule> EmployeesSchedules { get; set; }
            = new List<EmployeeSchedule>();
}

