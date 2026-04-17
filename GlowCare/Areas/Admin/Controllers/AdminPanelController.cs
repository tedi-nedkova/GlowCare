using GlowCare.Core.Contracts;
using GlowCare.ViewModels.SpecialistRequest;
using GlowCare.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminPanelController(
        IUserService userService,
        ISpecialistApplicationService specialistApplicationService,
        ILogger<AdminPanelController> logger) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserManagement(int pageNumber = 1)
        {
            try
            {
                const int PageSize = 4;

                UserManagementViewModel model = new()
                {
                    Users = await userService.GetAllUsersAsync(pageNumber, PageSize),
                    CurrentPage = pageNumber,
                    TotalPages = await userService.GetTotalPagesAsync(PageSize)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching the users.");
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AdminRoleAssign(Guid userId, string roleName)
        {
            try
            {
                if (roleName == "Specialist")
                {
                    TempData["ErrorMessage"] = "Specialist role can only be granted through approved applications.";
                    return RedirectToAction(nameof(UserManagement));
                }

                bool userExists = await userService.UserExistsByIdAsync(userId);
                if (!userExists)
                {
                    TempData["ErrorMessage"] = "User was not found.";
                    return RedirectToAction(nameof(UserManagement));
                }

                bool assignResult = await userService.AssignUserToRoleAsync(userId, roleName);
                if (!assignResult)
                {
                    TempData["ErrorMessage"] = "Could not assign role to user.";
                    return RedirectToAction(nameof(UserManagement));
                }

                TempData["SuccessMessage"] = "Role assigned successfully.";
                return RedirectToAction(nameof(UserManagement));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while assigning role {RoleName} to user {UserId}.", roleName, userId);
                TempData["ErrorMessage"] = "Unexpected error occurred while assigning role.";
                return RedirectToAction(nameof(UserManagement));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserRole(Guid userId, string roleName)
        {
            try
            {
                bool userExists = await userService.UserExistsByIdAsync(userId);
                if (!userExists)
                {
                    TempData["ErrorMessage"] = "User was not found.";
                    return RedirectToAction(nameof(UserManagement));
                }

                bool removeResult = await userService.RemoveUserFromRoleAsync(userId, roleName);
                if (!removeResult)
                {
                    TempData["ErrorMessage"] = "Could not remove role from user.";
                    return RedirectToAction(nameof(UserManagement));
                }

                TempData["SuccessMessage"] = "Role removed successfully.";
                return RedirectToAction(nameof(UserManagement));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while removing role {RoleName} from user {UserId}.", roleName, userId);
                TempData["ErrorMessage"] = "Unexpected error occurred while removing role.";
                return RedirectToAction(nameof(UserManagement));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                bool userExists = await userService.UserExistsByIdAsync(userId);
                if (!userExists)
                {
                    TempData["ErrorMessage"] = "User was not found.";
                    return RedirectToAction(nameof(UserManagement));
                }

                bool removeResult = await userService.DeleteUserAsync(userId);
                if (!removeResult)
                {
                    TempData["ErrorMessage"] = "Could not delete user.";
                    return RedirectToAction(nameof(UserManagement));
                }

                TempData["SuccessMessage"] = "User deleted successfully.";
                return RedirectToAction(nameof(UserManagement));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting user {UserId}.", userId);
                TempData["ErrorMessage"] = "Unexpected error occurred while deleting user.";
                return RedirectToAction(nameof(UserManagement));
            }
        }

        [HttpGet]
        public async Task<IActionResult> PendingApplications()
        {
            try
            {
                var applications = await specialistApplicationService.GetPendingApplicationsAsync();
                return View(applications);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching pending specialist applications.");
                TempData["ErrorMessage"] = "Could not load pending applications.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ApplicationDetails(int id)
        {
            try
            {
                var application = await specialistApplicationService.GetByIdAsync(id);

                if (application == null)
                {
                    return NotFound();
                }

                return View(application);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching application details for application {ApplicationId}.", id);
                TempData["ErrorMessage"] = "Could not load application details.";
                return RedirectToAction(nameof(PendingApplications));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await specialistApplicationService.ApproveAsync(id);
                TempData["SuccessMessage"] = "Application approved successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while approving application {ApplicationId}.", id);
                TempData["ErrorMessage"] = "Unexpected error occurred while approving application.";
            }

            return RedirectToAction(nameof(PendingApplications));
        }

        [HttpPost]
        public async Task<IActionResult> Decline(ReviewApplicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid decline request.";
                return RedirectToAction(nameof(ApplicationDetails), new { id = model.Id });
            }

            try
            {
                await specialistApplicationService.DeclineAsync(model.Id, model.RejectionReason);
                TempData["SuccessMessage"] = "Application declined successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while declining application {ApplicationId}.", model.Id);
                TempData["ErrorMessage"] = "Unexpected error occurred while declining application.";
            }

            return RedirectToAction(nameof(PendingApplications));
        }
    }
}