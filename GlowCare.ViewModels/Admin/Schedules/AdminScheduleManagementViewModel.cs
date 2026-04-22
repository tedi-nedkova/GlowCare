namespace GlowCare.ViewModels.Admin.Schedules;

public class AdminScheduleManagementViewModel
{
    public List<AdminScheduleListItemViewModel> Schedules { get; set; } = new();
    public CreateAdminScheduleViewModel NewSchedule { get; set; } = new();
}
