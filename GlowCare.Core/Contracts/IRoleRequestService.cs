using GlowCare.Core.Helpers;
using GlowCare.ViewModels.Roles;

namespace GlowCare.Core.Contracts;

public interface IRoleRequestService
{
    Task SendSpecialitRoleRequest(
        RoleRequestSentViewModel model, 
        string clinetId);

    Task<PaginatedList<RoleRequestInfoViewModel>> GetSpecialistRoleRequestAsync(
        int page, 
        int pageSize);

    Task AcceptRequestAsync(
        int requestId, 
        string requestType, 
        string clientId);

    Task DeclineRequestAsync(
        int requestId, 
        string requestType, 
        string clinetId);
}

