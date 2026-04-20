using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Controllers;

[Authorize]
public class ProfileController(
    IUserService userService,
    UserManager<GlowUser> userManager,
    ILogger<ProfileController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            string? userIdString = userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Redirect("/Identity/Account/Login");
            }

            var model = await userService.GetUserProfileAsync(userId);
            return View(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while loading the user profile.");
            return RedirectToAction("Error", "Home");
        }
    }
}
