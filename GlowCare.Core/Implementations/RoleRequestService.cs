using GlowCare.Core.Contracts;
using GlowCare.Core.Helpers;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.Entities.Models.Enums;
using GlowCare.ViewModels.Roles;
using Microsoft.AspNetCore.Identity;

namespace GlowCare.Core.Implementations;

public class RoleRequestService(
    IRepository<IdentityRole, int> rolesRepository,
    UserManager<Client> userManager) : IRoleRequestService
{
    public async Task AcceptRequestAsync(int requestId, string requestType, string clientId)
    {
       
    }

    public async Task DeclineRequestAsync(int requestId, string requestType, string clinetId)
    {

    }

    public Task<PaginatedList<RoleRequestInfoViewModel>> GetSpecialistRoleRequestAsync(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task SendSpecialitRoleRequest(RoleRequestSentViewModel model, string clinetId)
    {



    }
}

