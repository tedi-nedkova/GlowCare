using GlowCare.Entities.Models;
using GlowCare.ViewModels.Procedures;
using GlowCare.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GlowCare.Core.Contracts;

public interface IProcedureService
{
    Task CreateProcedureAsync(
        IndexViewModel model,
        Guid userId);

    Task DeleteProcedureAsync(
        DeleteProcedureViewModel model);
    Task<DeleteProcedureViewModel> GetDeleteProcedureAsync(
        int id,
        Guid userId);

    Task<Procedure> EditProcedureAsync(
        EditProcedureViewModel model,
        int id);

    Task<EditProcedureViewModel> GetEditProcedureAsync(
        int id);

    Task<IEnumerable<DetailsProcedureViewModel>> GetAllProcedureDetailsByUserIdAsync(
        Guid userId);

    Task<AvailabilityCheckResultViewModel> IsSlotAvailableAsync(
        Guid employeeId,
        int serviceId,
        DateTime requestedTime);

    Task<IEnumerable<SelectListItem>> GetEmployeeSelectListAsync();

    Task<IEnumerable<SelectListItem>> GetServiceSelectListAsync();

    Task<IEnumerable<SelectListItem>> GetServicesByEmployeeIdAsync(
        Guid employeeId);

    Task RejectProcedureAsync(
        int procedureId,
        Guid specialistUserId);
}

