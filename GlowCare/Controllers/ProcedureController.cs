using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Procedures;
using GlowCare.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Controllers;

public class ProcedureController(
    IProcedureService procedureService,
    ILogger<ProcedureController> logger,
    UserManager<GlowUser> userManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var procedure = new IndexViewModel();

        await LoadDropdownsAsync();

        return View(procedure);
    }

    [HttpPost]
    public async Task<IActionResult> Add(IndexViewModel model)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            TempData["BookingError"] = "Трябва да влезете в профила си, за да запазите час.";
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            TempData["BookingError"] = "Моля, попълнете всички полета правилно.";
            return RedirectToAction("Index", "Home");
        }

        try
        {
            string? userIdString = userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                TempData["BookingError"] = "Невалиден потребител.";
                return RedirectToAction("Login", "Account");
            }

            var availabilityResult = await procedureService.IsSlotAvailableAsync(
                model.EmployeeId,
                model.ServiceId,
                model.AppointmentDate
            );

            if (!availabilityResult.IsAvailable)
            {
                TempData["BookingError"] = availabilityResult.Message;
                return RedirectToAction("Index", "Home");
            }

            await procedureService.CreateProcedureAsync(model, userId);

            TempData["BookingSuccess"] = "Часът беше запазен успешно.";
            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Validation error while adding procedure.");
            TempData["BookingError"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while adding procedure.");
            TempData["BookingError"] = "Възникна грешка при запазването.";
            return RedirectToAction("Index", "Home");
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> MyProcedures()
    {
        try
        {
            Guid clientId = Guid.Parse(userManager.GetUserId(User)!);
            var models = await procedureService.GetAllProcedureDetailsByUserIdAsync(clientId);

            return View(models);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting procedures.");
            return RedirectToAction("Error", "Home");
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> CheckAvailability(Guid employeeId, int serviceId, DateTime dateTime)
    {
        try
        {
            var result = await procedureService.IsSlotAvailableAsync(employeeId, serviceId, dateTime);
            return PartialView("_CheckAvailabilityPopup", result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while checking availability.");
            return PartialView("_CheckAvailabilityPopup", false);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetServicesByEmployee(Guid employeeId)
    {
        var services = await procedureService.GetServicesByEmployeeIdAsync(employeeId);
        return Json(services);
    }

    [Authorize(Roles = "User,Specialist")]
    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            string? userIdString = userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Redirect("/Identity/Account/Login");
            }

            await procedureService.CancelProcedureAsync(id, userId);
            TempData["ProfileMessage"] = "Процедурата беше отказана успешно.";
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unauthorized cancel attempt for procedure {ProcedureId}.", id);
            TempData["ProfileError"] = "Нямате право да отказвате този час.";
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid cancel attempt for procedure {ProcedureId}.", id);
            TempData["ProfileError"] = "Само бъдещи активни часове могат да бъдат отказвани.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while cancelling a procedure.");
            TempData["ProfileError"] = "Възникна грешка при отказването на часа.";
        }

        return RedirectToAction("Index", "Profile");
    }

    private async Task LoadDropdownsAsync()
    {
        var employees = await procedureService.GetEmployeeSelectListAsync();
        var services = await procedureService.GetServiceSelectListAsync();

        ViewBag.Employees = employees;
        ViewBag.Services = services;
    }
}