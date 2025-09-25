using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlowCare.Entities.Models;

[PrimaryKey(nameof(DayOfWeekId), nameof(ScheduleId))]
public class ScheduleDayOfWeek
{
    public int DayOfWeekId { get; set; }
    [ForeignKey(nameof(DayOfWeekId))]
    public DayOfWeek DayOfWeek { get; set; } = null!;

    public int ScheduleId { get; set; }
    [ForeignKey(nameof(ScheduleId))]
    public Schedule Schedule { get; set; } = null!;
}

