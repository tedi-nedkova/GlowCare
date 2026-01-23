using GlowCare.Entities.Models;
using GlowCare.ViewModels.Services;

namespace GlowCare.Core.Contracts;

public interface IServiceService
{
    Task CreateServiceAsync(
        AddServiceViewModel model,
        Guid userId);

    Task DeleteServiceAsync(
        DeleteServiceViewModel model);
    Task<DeleteServiceViewModel> GetDeleteProcedureAsync(
        int id,
        Guid userId);

    Task<Procedure> EditProcedureAsync(
        EditServiceViewModel model,
        int id);

    Task<EditServiceViewModel> GetEditProcedureAsync(
        int id);

    Task<IEnumerable<DetailsServiceViewModel>> GetAllProcedureDetailsByUserIdAsync(
        Guid userId);
}

