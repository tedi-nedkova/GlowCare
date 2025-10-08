using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Controllers;

public class RoleRequestController(
    IRoleRequestService roleRequestService,
    IConfiguration configuration,
    ILogger<RoleRequestController> logger,
    UserManager<GlowUser> userManager
    ) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult SendSpecialistRoleRequest()
    {
        var model = new RoleRequestInfoViewModel();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SendSpecialistRoleRequest(
        RoleRequestSentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            string clientId = userManager.GetUserId(User)
                ?? throw new NullReferenceException("User not found.");

            await roleRequestService.SendSpecialitRoleRequest(model, clientId);
        }
        catch (NullReferenceException nex)
        {
            logger.LogError($"An error occured while sending role request. {nex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (InvalidOperationException iex)
        {
            logger.LogError($"An error occured while sending role request. {iex.Message}");
            return RedirectToAction("Error", "Home");
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occured while sending role request. {ex.Message}");
            return RedirectToAction("Error", "Home");
        }

        return RedirectToAction(nameof(Index));
    }
}

