using GlowCare.ViewModels.Admin.Schedules;

namespace GlowCare.Core.Contracts;

public interface IAdminScheduleService
{
    Task<AdminScheduleManagementViewModel> GetScheduleManagementViewModelAsync();

    Task<CreateAdminScheduleViewModel> GetCreateScheduleViewModelAsync(
        CreateAdminScheduleViewModel? formModel = null);

    Task<EditAdminScheduleViewModel> GetEditScheduleViewModelAsync(
        int id,
        EditAdminScheduleViewModel? formModel = null);

    Task CreateScheduleAsync(CreateAdminScheduleViewModel model);

    Task EditScheduleAsync(EditAdminScheduleViewModel model);

    Task DeleteScheduleAsync(int id);
}