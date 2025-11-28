using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlowCare.Areas.Admin.Controllers
{
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
        public async Task<IActionResult> AdminRoleAssign(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";

                    return RedirectToAction("Users");
                }

                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!roleResult.Succeeded)
                    {
                        logger.LogError($"Failed to create Admin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");

                        return RedirectToAction("Users");
                    }
                }

                var result = await _userManager.AddToRoleAsync(user, "Admin");

                if (!result.Succeeded)
                {
                    logger.LogError($"Failed to assign Admin role to user {id}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                    return RedirectToAction("Users");
                }

                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                logger.LogError($"Unexpected error in AdminRoleAssign for user {id}. {ex.Message}", ex);

                return RedirectToAction("Error", "Home");
            }
        }

    }
}
