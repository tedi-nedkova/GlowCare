using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace GlowCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminPanelController(
         IUserService userService,
        UserManager<GlowUser> _userManager,
       RoleManager<IdentityRole> _roleManager,
       ILogger<AdminPanelController> logger) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserManagement(int pageNumber = 1)
        {
            try
            {
                int pageSize = 4;
                var users = await userService.GetAllUsersAsync(pageNumber, pageSize);
                var totalPages = await userService.GetTotalPagesAsync(pageSize);

                var model = new UserManagementViewModel()
                {
                    Users = users,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occured while fetching the users. {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AssignAdminRole(Guid userId, string role)
        {
            bool userExists = await userService
                .UserExistsByIdAsync(userId);

            if (!userExists)
            {
                return RedirectToAction(nameof(UserManagement));
            }

            bool assignResult = await userService
                .AssignUserToAdminRoleAsync(userId, role);

            if (!assignResult)
            {
                return RedirectToAction(nameof(UserManagement));
            }

            return RedirectToAction(nameof(UserManagement));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAdminRole(Guid userId, string role)
        {
            bool userExists = await userService
                .UserExistsByIdAsync(userId);

            if (!userExists)
            {
                return RedirectToAction(nameof(UserManagement));
            }

            bool removeResult = await userService
                .RemoveUserRoleAsync(userId, role);

            if (!removeResult)
            {
                return RedirectToAction(nameof(UserManagement));
            }

            return RedirectToAction(nameof(UserManagement));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            bool userExists = await userService
                .UserExistsByIdAsync(userId);

            if (!userExists)
            {
                return RedirectToAction(nameof(UserManagement));
            }

            bool removeResult = await userService
                .DeleteUserAsync(userId);

            if (!removeResult)
            {
                return RedirectToAction(nameof(UserManagement));
            }

            return RedirectToAction(nameof(UserManagement));
        }

        private string GetCurrentClientId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}
