using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlowCare.Entities.Models;

[PrimaryKey(nameof(EmployeeId), nameof(ScheduleId))]
public class EmployeeSchedule
{
    public string EmployeeId { get; set; } = null!;
    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public int ScheduleId { get; set; }
    [ForeignKey(nameof(ScheduleId))]
    public Schedule Schedule { get; set; } = null!;
}
