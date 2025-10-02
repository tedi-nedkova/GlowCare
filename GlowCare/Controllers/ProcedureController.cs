using GlowCare.Common.Helpers;
using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Procedures;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Controllers;

public class ProcedureController(
        IProcedureService procedureService,
        IConfiguration configuration,
        ILogger<ProcedureController> logger,
        UserManager<Client> userManager) : Controller
{
    [HttpGet]
    public IActionResult Add()
    {
        var procedure = new ProcedureAddViewModel(); 

        return View(procedure);
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        ProcedureAddViewModel model)
    {
        if (ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            string? userId = userManager.GetUserId(User);

            await procedureService.CreateProcedureAsync(model, userId!);

            return RedirectToAction(nameof(MyProcedures));
        }
        catch (NullReferenceException nex)
        {
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> MyProcedures()
    {
        try
        {
            string? clientId = userManager.GetUserId(User);
            var models = await procedureService.GetAllProcedureDetailsByClientIdAsync(clientId!);

            return View(models);
        }
        catch (NullReferenceException nex)
        {
            return RedirectToAction("Error", "Home", new { code = 404 });
        }
        catch (InvalidOperationException iex)
        {
            return RedirectToAction("Error", "Home", new { code = 500 });
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id)
    {
        try
        {
            var procedure = await procedureService.GetEditProcedureAsync(id);

            return View(procedure);
        }
        catch (NullReferenceException nex)
        {
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        ProcedureEditViewModel model, 
        int id)
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
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {

            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet]
    public async Task<ActionResult> Delete(
        int id)
    {
        try
        {
            string? clientId = userManager.GetUserId(User);

            var course = await procedureService.GetDeleteProcedureAsync(id, clientId!);

            return View(course);
        }
        catch (NullReferenceException nex)
        {
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost]
    public async Task<ActionResult> Delete(
        ProcedureDeleteViewModel model)
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
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home");
        }
    }
}

