using GlowCare.Core.Contracts;
using GlowCare.Core.Helpers;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.Entities.Models.Enums;
using GlowCare.ViewModels.Roles;
using Microsoft.AspNetCore.Identity;

namespace GlowCare.Core.Implementations;

public class RoleRequestService(
    IRepository<SpecialistRoleRequest, int> specialistRoleRequestRepository,
    UserManager<GlowUser> userManager) : IRoleRequestService
{
    public async Task AcceptRequestAsync(
        int requestId, 
        string clientId)
    {
        var request = await specialistRoleRequestRepository.GetByIdAsync(requestId);

        if (request != null)
        {
            request.Status = RequestStatus.Accepted;

            await specialistRoleRequestRepository.UpdateAsync(request);

            var user = await userManager.FindByIdAsync(request.SenderId)
                ?? throw new NullReferenceException("User not found!");

            await userManager.AddToRoleAsync(user, "Specialist");
        }
    }

    public async Task DeclineRequestAsync(
        int requestId)
    {
        var request = await specialistRoleRequestRepository.GetByIdAsync(requestId);

        if (request != null)
        {
            request.Status = RequestStatus.Declined;

            await specialistRoleRequestRepository.UpdateAsync(request);
        }
    }

    public async Task<PaginatedList<RoleRequestInfoViewModel>> GetSpecialistRoleRequestsAsync(
        int page, 
        int pageSize)
    {
        var query = specialistRoleRequestRepository
            .GetAllAttached()
            .Where(r => r.Status == RequestStatus.Pending);

        var paginatedList = await PaginatedList<RoleRequestInfoViewModel>.CreateAsync(
                query.Select(r => new RoleRequestInfoViewModel
                {
                    Id = r.Id,
                    Sender = r.Sender,
                    Description = r.Description,
                    Status = r.Status.ToString()
                }),
                page,
                pageSize
            );

        return paginatedList;
    }

    public async Task SendSpecialitRoleRequest(
        RoleRequestSentViewModel model, 
        string clientId)
    {
        if (model == null)
        {
            throw new NullReferenceException("Entity was null!");
        }

        var request = new SpecialistRoleRequest()
        {
            Id = model.Id,
            Sender = model.Sender,
            SenderId = clientId,
            Description = model.Description,
            Status = RequestStatus.Pending
        };

        await specialistRoleRequestRepository.AddAsync(request);
    }
}

