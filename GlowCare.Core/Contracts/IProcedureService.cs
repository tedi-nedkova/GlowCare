using GlowCare.Entities.Models;
using GlowCare.ViewModels.Procedures;

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
}

