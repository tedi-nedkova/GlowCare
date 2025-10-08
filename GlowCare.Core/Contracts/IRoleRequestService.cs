using GlowCare.Core.Helpers;
using GlowCare.ViewModels.Roles;

namespace GlowCare.Core.Contracts;

public interface IRoleRequestService
{
    Task SendSpecialitRoleRequest(
        RoleRequestSentViewModel model,
        string clientId);

    Task<PaginatedList<RoleRequestInfoViewModel>> GetSpecialistRoleRequestsAsync(
        int page,
        int pageSize);

    Task AcceptRequestAsync(
        int requestId,
        string clientId);

    Task DeclineRequestAsync(
        int requestId);
}

