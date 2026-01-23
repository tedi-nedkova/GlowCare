using GlowCare.Core.Contracts;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Services;
using Microsoft.AspNetCore.Identity;

namespace GlowCare.Core.Implementations;

public class ServiceService(
    IRepository<Service, int> serviceRepository,
    UserManager<GlowUser> userManager
    ) : IServiceService
{
    public Task CreateServiceAsync(AddServiceViewModel model, Guid userId)
    {
        if ( model == null)
        {
            throw new NullReferenceException("Entity not found");
        }


    }

    public Task DeleteServiceAsync(DeleteServiceViewModel model)
    {
        throw new NotImplementedException();
    }

    public Task<Procedure> EditProcedureAsync(EditServiceViewModel model, int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<DetailsServiceViewModel>> GetAllProcedureDetailsByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<DeleteServiceViewModel> GetDeleteProcedureAsync(int id, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<EditServiceViewModel> GetEditProcedureAsync(int id)
    {
        throw new NotImplementedException();
    }
}
