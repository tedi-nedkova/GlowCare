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
    RoleManager<IdentityRole> _roleManager,
    UserManager<GlowUser> _userManager)
    : IUserService
{
    public async Task<bool> AssignUserToRoleAsync(Guid userId, string roleName)
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

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        GlowUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return false;
        }

        IdentityResult? result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return false;
        }

        return true;
    }

    public async Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 5)
    {
        var users = await _userManager.Users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

        var userViewModels = new List<AllUsersViewModel>();

        foreach (var user in users)
        {
            userViewModels.Add(new AllUsersViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!
            });
        }

        return userViewModels;
    }

    public async Task<int> GetTotalPagesAsync(int pageSize = 5)
    {
        var totalUsers = await _userManager.Users.CountAsync();
        return (int)Math.Ceiling(totalUsers / (double)pageSize);
    }

    public async Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName)
    {
        GlowUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null || !(await _roleManager.RoleExistsAsync(roleName)))
        {
            return false;
        }

        bool alreadyInRole = await _userManager.IsInRoleAsync(user, roleName);

        if (alreadyInRole)
        {
            IdentityResult? result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<bool> UserExistsByIdAsync(Guid userId)
    {
        GlowUser? user = await _userManager.FindByIdAsync(userId.ToString());

        return user != null;
    }
}
