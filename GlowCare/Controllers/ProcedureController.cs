using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Procedures;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Controllers;

public class ProcedureController(
        IProcedureService procedureService,
        IConfiguration configuration,
        ILogger<ProcedureController> logger,
        UserManager<GlowUser> userManager) : Controller
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
            Guid userId = Guid.Parse(userManager.GetUserId(User));

            await procedureService.CreateProcedureAsync(model, userId);

            return RedirectToAction(nameof(MyProcedures));
        }
        catch (NullReferenceException nex)
        {
            logger.LogError($"An error occured while adding procedure. {nex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            logger.LogError($"An error occured while adding procedure. {iex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occured while adding procedure. {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> MyProcedures()
    {
        try
        {
            Guid clientId = Guid.Parse(userManager.GetUserId(User));
            var models = await procedureService.GetAllProcedureDetailsByUserIdAsync(clientId!);

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
    public async Task<ActionResult> Delete(
        int id)
    {
        try
        {
            Guid clientId = Guid.Parse(userManager.GetUserId(User));

            var course = await procedureService.GetDeleteProcedureAsync(id, clientId!);

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
}

