namespace GlowCare.ViewModels.Admin.Schedules;

public class AdminScheduleListItemViewModel
{
    public int Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string SpecialistName { get; set; } = string.Empty;

    public string DayOfWeek { get; set; } = string.Empty;

    public string StartTime { get; set; } = string.Empty;

    public string EndTime { get; set; } = string.Empty;
}