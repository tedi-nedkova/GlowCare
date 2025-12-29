using GlowCare.Entities.Models;
using GlowCare.ViewModels.Procedures;

namespace GlowCare.Core.Contracts;

public interface IProcedureService
{
    Task CreateProcedureAsync(
        ProcedureAddViewModel model,
        Guid userId);

    Task DeleteProcedureAsync(
        ProcedureDeleteViewModel model);
    Task<ProcedureDeleteViewModel> GetDeleteProcedureAsync(
        int id,
        Guid userId);

    Task<Procedure> EditProcedureAsync(
        ProcedureEditViewModel model, 
        int id);

    Task<ProcedureEditViewModel> GetEditProcedureAsync(
        int id);

    Task <IEnumerable<ProcedureDetailsViewModel>> GetAllProcedureDetailsByUserIdAsync(
        Guid userId);
}

