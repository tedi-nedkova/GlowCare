using GlowCare.ViewModels.Users;
using GlowCare.Entities.Models;

namespace GlowCare.Core.Contracts;

public interface IUserService
{
    Task<bool> AssignUserToRoleAsync(Guid userId, string roleName);

    Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 5);

    Task<int> GetTotalPagesAsync(int pageSize = 5);

    Task<bool> UserExistsByIdAsync(Guid userId);

    Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName);

    Task<bool> DeleteUserAsync(Guid userId);

    Task<UserProfileViewModel> GetUserProfileAsync(Guid userId);

    Task UpdateUserMembershipAsync(GlowUser user);
}

