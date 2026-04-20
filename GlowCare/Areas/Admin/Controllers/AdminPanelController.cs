using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Reviews;
using GlowCare.ViewModels.Services;
using GlowCare.ViewModels.SpecialistRequest;
using GlowCare.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminPanelController(
        IUserService userService,
        ISpecialistApplicationService specialistApplicationService,
        IServiceService serviceService,
        IReviewService reviewService,
        UserManager<GlowUser> userManager,
        ILogger<AdminPanelController> logger) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ServiceManagement(string? searchTerm, int pageNumber = 1)
        {
            const int PageSize = 6;
            ViewData["Title"] = "Управление на услуги";

            try
            {
                return View(await serviceService.GetAdminServiceManagementViewModelAsync(searchTerm, pageNumber, PageSize));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while loading service management.");
                TempData["ErrorMessage"] = "Неуспешно зареждане на услугите.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddService()
        {
            ViewData["Title"] = "Добавяне на услуга";

            try
            {
                return View(await serviceService.GetAdminServiceManagementViewModelAsync());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while loading the add service page.");
                TempData["ErrorMessage"] = "Неуспешно зареждане на формата за добавяне на услуга.";
                return RedirectToAction(nameof(ServiceManagement));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceManagement(AdminServiceManagementViewModel model)
        {
            ViewData["Title"] = "Добавяне на услуга";

            if (!ModelState.IsValid)
            {
                AdminServiceManagementViewModel viewModel = await serviceService.GetAdminServiceManagementViewModelAsync(formModel: model.NewService);
                return View("AddService", viewModel);
            }

            try
            {
                Guid userId = Guid.Parse(userManager.GetUserId(User)!);
                await serviceService.CreateServiceAsync(model.NewService, userId);
                TempData["SuccessMessage"] = "Услугата беше добавена успешно.";
                return RedirectToAction(nameof(ServiceManagement));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a service.");
                TempData["ErrorMessage"] = "Възникна грешка при създаването на услугата.";

                AdminServiceManagementViewModel viewModel = await serviceService.GetAdminServiceManagementViewModelAsync(formModel: model.NewService);
                return View("AddService", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id, string? searchTerm, int pageNumber = 1)
        {
            try
            {
                await serviceService.DeleteServiceAsync(new DeleteServiceViewModel { Id = id });
                TempData["SuccessMessage"] = "Услугата беше изтрита успешно.";
            }
            catch (NullReferenceException)
            {
                TempData["ErrorMessage"] = "Услугата не беше намерена.";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting service {ServiceId}.", id);
                TempData["ErrorMessage"] = "Възникна грешка при изтриването на услугата.";
            }

            return RedirectToAction(nameof(ServiceManagement), new { searchTerm, pageNumber });
        }

        [HttpGet]
        public async Task<IActionResult> EditService(int id)
        {
            ViewData["Title"] = "Редактиране на услуга";

            try
            {
                EditServiceViewModel model = await serviceService.GetEditServiceAsync(id);
                return View(model);
            }
            catch (NullReferenceException)
            {
                TempData["ErrorMessage"] = "Услугата не беше намерена.";
                return RedirectToAction(nameof(ServiceManagement));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while loading service {ServiceId} for edit.", id);
                TempData["ErrorMessage"] = "Възникна грешка при зареждането на услугата за редакция.";
                return RedirectToAction(nameof(ServiceManagement));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(EditServiceViewModel model)
        {
            ViewData["Title"] = "Редактиране на услуга";

            if (!ModelState.IsValid)
            {
                await serviceService.PopulateEditServiceLookupDataAsync(model);
                return View(model);
            }

            try
            {
                await serviceService.EditServiceAsync(model);
                TempData["SuccessMessage"] = "Услугата беше редактирана успешно.";
                return RedirectToAction(nameof(ServiceManagement));
            }
            catch (NullReferenceException)
            {
                TempData["ErrorMessage"] = "Услугата не беше намерена.";
                return RedirectToAction(nameof(ServiceManagement));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while editing service {ServiceId}.", model.Id);
                TempData["ErrorMessage"] = "Възникна грешка при редакцията на услугата.";
                await serviceService.PopulateEditServiceLookupDataAsync(model);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReviewsManagement()
        {
            try
            {
                AdminReviewManagementViewModel model = new()
                {
                    Reviews = await reviewService.GetAllReviewsForAdminAsync()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while loading the reviews management page.");
                TempData["ErrorMessage"] = "Неуспешно зареждане на ревютата.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await reviewService.SoftDeleteReviewAsync(id);
                TempData["SuccessMessage"] = "Ревюто беше изтрито успешно.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting review {ReviewId}.", id);
                TempData["ErrorMessage"] = "Възникна грешка при изтриването на ревюто.";
            }

            return RedirectToAction(nameof(ReviewsManagement));
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
                    TempData["ErrorMessage"] = "Ролята „Специалист“ може да бъде зададена само чрез одобрена заявка.";
                    return RedirectToAction(nameof(UserManagement));
                }

                bool userExists = await userService.UserExistsByIdAsync(userId);
                if (!userExists)
                {
                    TempData["ErrorMessage"] = "Потребителят не беше намерен.";
                    return RedirectToAction(nameof(UserManagement));
                }

                bool assignResult = await userService.AssignUserToRoleAsync(userId, roleName);
                if (!assignResult)
                {
                    TempData["ErrorMessage"] = "Ролята не можа да бъде зададена на потребителя.";
                    return RedirectToAction(nameof(UserManagement));
                }

                TempData["SuccessMessage"] = "Ролята беше зададена успешно.";
                return RedirectToAction(nameof(UserManagement));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while assigning role {RoleName} to user {UserId}.", roleName, userId);
                TempData["ErrorMessage"] = "Възникна неочаквана грешка при задаване на ролята.";
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
                    TempData["ErrorMessage"] = "Потребителят не беше намерен.";
                    return RedirectToAction(nameof(UserManagement));
                }

                bool removeResult = await userService.RemoveUserFromRoleAsync(userId, roleName);
                if (!removeResult)
                {
                    TempData["ErrorMessage"] = "Ролята не можа да бъде премахната от потребителя.";
                    return RedirectToAction(nameof(UserManagement));
                }

                TempData["SuccessMessage"] = "Ролята беше премахната успешно.";
                return RedirectToAction(nameof(UserManagement));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while removing role {RoleName} from user {UserId}.", roleName, userId);
                TempData["ErrorMessage"] = "Възникна неочаквана грешка при премахване на ролята.";
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
                    TempData["ErrorMessage"] = "Потребителят не беше намерен.";
                    return RedirectToAction(nameof(UserManagement));
                }

                bool removeResult = await userService.DeleteUserAsync(userId);
                if (!removeResult)
                {
                    TempData["ErrorMessage"] = "Потребителят не можа да бъде изтрит.";
                    return RedirectToAction(nameof(UserManagement));
                }

                TempData["SuccessMessage"] = "Потребителят беше изтрит успешно.";
                return RedirectToAction(nameof(UserManagement));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting user {UserId}.", userId);
                TempData["ErrorMessage"] = "Възникна неочаквана грешка при изтриването на потребителя.";
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
                TempData["ErrorMessage"] = "Чакащите заявки не можаха да бъдат заредени.";
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
                TempData["ErrorMessage"] = "Детайлите за заявката не можаха да бъдат заредени.";
                return RedirectToAction(nameof(PendingApplications));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await specialistApplicationService.ApproveAsync(id);
                TempData["SuccessMessage"] = "Заявката беше одобрена успешно.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while approving application {ApplicationId}.", id);
                TempData["ErrorMessage"] = "Възникна неочаквана грешка при одобряване на заявката.";
            }

            return RedirectToAction(nameof(PendingApplications));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(ReviewApplicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault() ?? "Невалидна заявка за отхвърляне.";
                return RedirectToAction(nameof(ApplicationDetails), new { id = model.Id });
            }

            try
            {
                await specialistApplicationService.DeclineAsync(model.Id, model.RejectionReason);
                TempData["SuccessMessage"] = "Заявката беше отхвърлена успешно.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while declining application {ApplicationId}.", model.Id);
                TempData["ErrorMessage"] = "Възникна неочаквана грешка при отхвърляне на заявката.";
            }

            return RedirectToAction(nameof(PendingApplications));
        }
    }
}
