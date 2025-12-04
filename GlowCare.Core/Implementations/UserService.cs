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

    public async Task<Client> CreateClientForUserAsync(GlowUser user)
    {
        var client = new Client { UserId = user.Id };
        _context.Clients.Add(client);

        await _context.SaveChangesAsync();

        return client;
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

    public Task<bool> RemoveUserFromRoleAsync(string userId, string roleName)
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

    public async Task<GlowUser> RegisterUserAsync(RegisterViewModel model)
    {
        var user = new GlowUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Age = model.Age,
            Gender = model.Gender,
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "User");

        var client = new Client
        {
            UserId = user.Id,
        };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return user;
    }
}
