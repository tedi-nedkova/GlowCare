using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Procedures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GlowCare.Controllers;

public class ProcedureController(
        IProcedureService procedureService,
        IServiceService serviceService,
        IEmployeeService employeeService,
        IConfiguration configuration,
        ILogger<ProcedureController> logger,
        UserManager<GlowUser> userManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var procedure = new AddProcedureViewModel();

        await LoadDropdownsAsync();

        return View(procedure);
    }

    
    [HttpPost]
    public async Task<IActionResult> Add(AddProcedureViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["BookingError"] = "Моля, попълнете всички полета правилно.";
            return RedirectToAction("Index", "Home");
        }

        try
        {
            Guid userId = Guid.Parse(userManager.GetUserId(User));

            bool isAvailable = await procedureService.IsSlotAvailableAsync(
                model.EmployeeId,
                model.ServiceId,
                model.AppointmentDate
            );

            if (!isAvailable)
            {
                TempData["BookingError"] = "Избраният час не е свободен.";
                return RedirectToAction("Index", "Home");
            }

            await procedureService.CreateProcedureAsync(model, userId);

            TempData["BookingSuccess"] = "Часът беше запазен успешно.";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occured while adding procedure. {ex.Message}");
            return RedirectToAction("Login", "");
        }
    }

    [HttpGet]
    public async Task<IActionResult> MyProcedures()
    {
        try
        {
            Guid clientId = Guid.Parse(userManager.GetUserId(User));
            var models = await procedureService.GetAllProcedureDetailsByUserIdAsync(clientId);

            return View(models);
        }
        catch (NullReferenceException nex)
        {
            logger.LogError($"An error occured while getting the procedures. {nex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            logger.LogError($"An error occured while getting the procedures. {iex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occured while getting the procedures. {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var procedure = await procedureService.GetEditProcedureAsync(id);

            return View(procedure);
        }
        catch (NullReferenceException nex)
        {
            logger.LogError($"An error occured while fetching procedure info. {nex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            logger.LogError($"An error occured while fetching procedure info. {iex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occured while fetching procedure info. {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditProcedureViewModel model, int id)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await procedureService.EditProcedureAsync(model, id);

            return RedirectToAction(nameof(MyProcedures));
        }
        catch (NullReferenceException nex)
        {
            logger.LogError($"An error occured while editing a procedure. {nex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            logger.LogError($"An error occured while editing a procedure. {iex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occured while editing a procedure. {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            Guid clientId = Guid.Parse(userManager.GetUserId(User));

            var course = await procedureService.GetDeleteProcedureAsync(id, clientId);

            return View(course);
        }
        catch (NullReferenceException nex)
        {
            logger.LogError($"An error occured while fetching procedure delete info. {nex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            logger.LogError($"An error occured while fetching procedure delete info. {iex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occured while fetching procedure delete info. {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost]
    public async Task<ActionResult> Delete(DeleteProcedureViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await procedureService.DeleteProcedureAsync(model);

            return RedirectToAction(nameof(MyProcedures));
        }
        catch (NullReferenceException nex)
        {
            logger.LogError($"An error occured while deleting a procedure. {nex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            logger.LogError($"An error occured while deleting a procedure. {iex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occured while deleting a procedure. {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> CheckAvailability(Guid employeeId, int serviceId, DateTime dateTime)
    {
        try
        {
            bool isAvailable = await procedureService.IsSlotAvailableAsync(employeeId, serviceId, dateTime);
            return PartialView("_CheckAvailabilityPopup", isAvailable);
        }
        catch
        {
            return PartialView("_CheckAvailabilityPopup", false);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetServicesByEmployee(Guid employeeId)
    {
        var services = await procedureService.GetServicesByEmployeeIdAsync(employeeId);

        return Json(services);
    }

    private async Task LoadDropdownsAsync()
    {
        var employees = await procedureService.GetEmployeeSelectListAsync();
        var services = await procedureService.GetServiceSelectListAsync();

        ViewBag.Employees = employees;
        ViewBag.Services = services;
    }
}