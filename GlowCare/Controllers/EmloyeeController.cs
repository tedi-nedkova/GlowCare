using GlowCare.Core.Contracts;
using GlowCare.Entities.Models.Enums;
using GlowCare.ViewModels.SpecialistRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GlowCare.Controllers;

public class EmployeeController(
    IEmployeeService employeeService,
    ISpecialistApplicationService specialistApplicationService,
    IUserService userService,
    ILogger<EmployeeController> logger) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, string? selectedService)
    {
        var model = await employeeService.GetEmployeesForIndexAsync(searchTerm, selectedService);
        return View(model);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        GlowCare.ViewModels.Employees.EmployeeInfoViewModel? employee =
            await employeeService.GetEmployeeByIdAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(userIdValue)
            && Guid.TryParse(userIdValue, out Guid userId)
            && employee.UserId == userId)
        {
            var profile = await userService.GetUserProfileAsync(userId);
            employee.IsCurrentSpecialistOwner = profile.IsSpecialist;
            employee.Procedures = profile.Procedures;
        }

        return View(employee);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Apply()
    {
        string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdValue))
        {
            return RedirectToAction("Index", "Home");
        }

        Guid userId = Guid.Parse(userIdValue);

        bool hasPending = await specialistApplicationService.UserHasPendingApplicationAsync(userId);
        bool isSpecialist = await specialistApplicationService.UserIsAlreadySpecialistAsync(userId);
        SpecialistApplicationViewModel? latestApplication =
            await specialistApplicationService.GetLatestByUserIdAsync(userId);

        ApplySpecialistViewModel model = await specialistApplicationService.GetApplicationDraftAsync(userId)
              ?? new ApplySpecialistViewModel();

        if (hasPending)
        {
            ViewBag.HideApplicationForm = true;

            if (TempData.Peek("ApplicationSuccessMessage") == null)
            {
                ViewBag.ApplicationInfoMessage = "Заявката ви в момента се разглежда.";
            }

            return View(model);
        }

        if (isSpecialist)
        {
            ViewBag.HideApplicationForm = true;
            ViewBag.ApplicationInfoMessage = "Вече сте специалист.";
            TempData.Remove("ApplicationSuccessMessage");
            TempData.Remove("ApplicationErrorMessage");

            return View(model);
        }

        if (latestApplication?.Status == RequestStatus.Declined)
        {
            ViewBag.ApplicationWarningMessage = "Последната ви заявка беше отхвърлена.";
            ViewBag.ApplicationRejectionReason = latestApplication.RejectionReason;
        }

        TempData.Remove("ApplicationErrorMessage");

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(ApplySpecialistViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdValue))
            {
                TempData["ApplicationErrorMessage"] = "Потребителят не можа да бъде разпознат.";
                return RedirectToAction("Index", "Home");
            }

            Guid userId = Guid.Parse(userIdValue);

            TempData.Remove("ApplicationErrorMessage");
            await specialistApplicationService.ApplyAsync(userId, model);

            TempData["ApplicationSuccessMessage"] = "Заявката беше изпратена успешно.";
            return RedirectToAction(nameof(Apply));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while submitting specialist application.");
            TempData["ApplicationErrorMessage"] = "Възникна неочаквана грешка при изпращането на заявката.";
            return RedirectToAction("Index", "Home");
        }
    }
}
