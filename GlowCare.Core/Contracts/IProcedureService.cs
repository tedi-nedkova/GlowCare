using GlowCare.Entities.Models;
using GlowCare.ViewModels.Procedures;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GlowCare.Core.Contracts;

public interface IProcedureService
{
    Task CreateProcedureAsync(
        AddProcedureViewModel model,
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

    Task <IEnumerable<DetailsProcedureViewModel>> GetAllProcedureDetailsByUserIdAsync(
        Guid userId);

    Task<bool> IsSlotAvailableAsync(
        Guid employeeId,
        int serviceId,
        DateTime requestedTime);

    Task<IEnumerable<SelectListItem>> GetEmployeeSelectListAsync();

    Task<IEnumerable<SelectListItem>> GetServiceSelectListAsync();

    Task<IEnumerable<SelectListItem>> GetServicesByEmployeeIdAsync(
        Guid employeeId);
}

