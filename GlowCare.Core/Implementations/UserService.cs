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
    UserManager<GlowUser> _userManager)
    : IUserService
{
    public Task<bool> AssignUserToRoleAsync(string userId, string roleName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserAsync(string userId)
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

    public Task<bool> RemoveUserRoleAsync(string userId, string roleName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserExistsByIdAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
