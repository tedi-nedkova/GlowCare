using GlowCare.Core.Contracts;
using GlowCare.Entities;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlowCare.Core.Implementations;
public class UserService(
    GlowCareDbContext _context,
    UserManager<GlowUser> _userManager,
    RoleManager<IdentityRole> _roleManager)
    : IUserService
{
    public async Task<bool> AssignUserToAdminRoleAsync(Guid userId, string roleName)
    {
        GlowUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null || !(await _roleManager.RoleExistsAsync(roleName)))
        {
            return false;
        }

        if (!await _userManager.IsInRoleAsync(user, roleName))
        {
            IdentityResult result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                return false;
            }
        }

        return true;
    }

    public Task<bool> DeleteUserAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 5)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalPagesAsync(int pageSize = 5)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveUserRoleAsync(Guid userId, string roleName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserExistsByIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}
