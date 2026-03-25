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
    public async Task CreateServiceAsync(AddServiceViewModel model, Guid userId)
    {
        if ( model == null)
        {
            throw new NullReferenceException("Entity not found");
        }

        var service = new Service
        {
            Name = model.Name,
            CategoryId = model.CategoryId,
            Description = model.Description,
            DurationInMinutes = model.DurationInMinutes,
            Price = model.Price,
            Points = model.Points,
        };

        await serviceRepository.AddAsync(service);
    }

    public async Task DeleteServiceAsync(DeleteServiceViewModel model)
    {
        Service service = await serviceRepository.GetByIdAsync(model.Id);

        if (service == null)
        {
            throw new NullReferenceException("Entity not found.");
        }

        if (service.IsDeleted)
        {
            throw new ArgumentException("Entity is already deleted.");
        }

        if (!service.IsDeleted || service != null)
        {
            service.IsDeleted = true;

            await serviceRepository.UpdateAsync(service);
        }
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
