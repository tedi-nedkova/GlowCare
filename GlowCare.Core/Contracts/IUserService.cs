using GlowCare.Entities.Models;
using GlowCare.ViewModels.Users;

namespace GlowCare.Core.Contracts;

public interface IUserService
{
    Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 5);

    Task<int> GetTotalPagesAsync(int pageSize = 5);

    Task<bool> UserExistsByIdAsync(string userId);

    Task<bool> AssignUserToRoleAsync(string userId, string roleName);

    Task<bool> RemoveUserFromRoleAsync(string userId, string roleName);

    Task<bool> RemoveUserRoleAsync(string userId, string roleName);

    Task<bool> DeleteUserAsync(string userId);

    Task<Client> CreateClientForUserAsync(GlowUser user);
    Task<GlowUser> RegisterUserAsync(RegisterViewModel model);
}

