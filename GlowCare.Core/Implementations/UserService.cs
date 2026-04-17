using GlowCare.Core.Contracts;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.Entities.Models.Enums;
using GlowCare.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GlowCare.Core.Implementations;

public class UserService(
    RoleManager<IdentityRole<Guid>> _roleManager,
    UserManager<GlowUser> _userManager,
    IRepository<Employee, Guid> employeeRepository,
    IRepository<SpecialistApplication, int> specialistApplicationRepository)
    : IUserService
{
    public async Task<bool> AssignUserToRoleAsync(Guid userId, string roleName)
    {
        GlowUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null || user.IsDeleted || !(await _roleManager.RoleExistsAsync(roleName)))
        {
            return false;
        }

        if (roleName == "Specialist")
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

        if (user == null || user.IsDeleted)
        {
            return false;
        }

        user.IsDeleted = true;
        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;

        Employee? employee = await employeeRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee != null)
        {
            employee.IsDeleted = true;

            bool employeeUpdated = await employeeRepository.UpdateAsync(employee);
            if (!employeeUpdated)
            {
                return false;
            }
        }

        IdentityResult result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                Console.WriteLine($"{error.Code}: {error.Description}");
            }

            return false;
        }

        await _userManager.UpdateSecurityStampAsync(user);

        return true;
    }

    public async Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 5)
    {
        var users = await _userManager.Users
            .Where(u => !u.IsDeleted)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userViewModels = new List<AllUsersViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            userViewModels.Add(new AllUsersViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Roles = roles.ToList()
            });
        }

        return userViewModels;
    }

    public async Task<int> GetTotalPagesAsync(int pageSize = 5)
    {
        var totalUsers = await _userManager.Users
            .Where(u => !u.IsDeleted)
            .CountAsync();

        return (int)Math.Ceiling(totalUsers / (double)pageSize);
    }

    public async Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName)
    {
        GlowUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null || user.IsDeleted || !(await _roleManager.RoleExistsAsync(roleName)))
        {
            return false;
        }

        bool alreadyInRole = await _userManager.IsInRoleAsync(user, roleName);

        if (!alreadyInRole)
        {
            return true;
        }

        IdentityResult result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if (!result.Succeeded)
        {
            return false;
        }

        if (roleName == "Specialist")
        {
            user.IsSpecialist = false;

            IdentityResult userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                return false;
            }

            SpecialistApplication? latestAcceptedApplication = await specialistApplicationRepository
                .GetAllAttached()
                .Where(a => a.UserId == userId && a.Status == RequestStatus.Accepted)
                .OrderByDescending(a => a.CreatedOn)
                .FirstOrDefaultAsync();

            if (latestAcceptedApplication != null)
            {
                latestAcceptedApplication.Status = RequestStatus.Revoked;
                latestAcceptedApplication.RejectionReason = null;

                bool applicationUpdated = await specialistApplicationRepository.UpdateAsync(latestAcceptedApplication);
                if (!applicationUpdated)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public async Task<bool> UserExistsByIdAsync(Guid userId)
    {
        GlowUser? user = await _userManager.FindByIdAsync(userId.ToString());

        return user != null && !user.IsDeleted;
    }
}