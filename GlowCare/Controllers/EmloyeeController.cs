using GlowCare.Core.Contracts;
using GlowCare.ViewModels.SpecialistRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GlowCare.Controllers;

public class EmployeeController(
    IEmployeeService employeeService,
    ISpecialistApplicationService specialistApplicationService,
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

        if (hasPending)
        {
            TempData["ErrorMessage"] = "You already have a pending application.";
            return RedirectToAction("Index", "Home");
        }

        if (isSpecialist)
        {
            TempData["ErrorMessage"] = "You are already a specialist.";
            return RedirectToAction("Index", "Home");
        }

        ApplySpecialistViewModel model = await specialistApplicationService.GetApplicationDraftAsync(userId)
              ?? new ApplySpecialistViewModel();

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
                TempData["ErrorMessage"] = "User could not be identified.";
                return RedirectToAction("Index", "Home");
            }

            Guid userId = Guid.Parse(userIdValue);

            await specialistApplicationService.ApplyAsync(userId, model);

            TempData["SuccessMessage"] = "Your application was submitted successfully.";
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
            TempData["ErrorMessage"] = "Unexpected error occurred while submitting your application.";
            return RedirectToAction("Index", "Home");
        }
    }
}
