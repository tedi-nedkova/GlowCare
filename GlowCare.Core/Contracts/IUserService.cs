using GlowCare.ViewModels.Users;

namespace GlowCare.Core.Contracts;

public interface IUserService
{
    Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 5);

    Task<int> GetTotalPagesAsync(int pageSize = 5);

    Task<bool> UserExistsByIdAsync(Guid userId);

    Task<bool> AssignUserToAdminRoleAsync(Guid userId, string roleName);

    Task<bool> RemoveUserRoleAsync(Guid userId, string roleName);

    Task<bool> DeleteUserAsync(Guid userId);
}

