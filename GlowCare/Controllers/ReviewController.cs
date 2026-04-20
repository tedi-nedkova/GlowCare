using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Controllers
{
    [Authorize]
    public class ReviewController(
        IReviewService reviewService,
        ILogger<ReviewController> logger,
        UserManager<GlowUser> userManager) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(Guid employeeId)
        {
            ReviewIndexViewModel? model = await reviewService.GetReviewsByEmployeeIdAsync(employeeId);

            if (model == null)
            {
                return RedirectToAction("Error", "Home", new { code = 404 });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add(Guid employeeId, int? procedureId)
        {
            Guid userId = Guid.Parse(userManager.GetUserId(User)!);

            AddReviewViewModel? review =
                await reviewService.GetAddReviewModelAsync(employeeId, userId, procedureId);

            if (review == null)
            {
                return RedirectToAction("Error", "Home", new { code = 404 });
            }

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddReviewViewModel model)
        {
            Guid userId = Guid.Parse(userManager.GetUserId(User)!);

            if (!ModelState.IsValid)
            {
                AddReviewViewModel? refreshedModel =
                    await reviewService.GetAddReviewModelAsync(model.EmployeeId, userId, model.ProcedureId);

                if (refreshedModel != null)
                {
                    model.AvailableProcedures = refreshedModel.AvailableProcedures;
                    model.SpecialistName = refreshedModel.SpecialistName;
                    model.ServiceId = refreshedModel.ServiceId;
                }

                return View(model);
            }

            try
            {
                await reviewService.CreateReviewAsync(model, userId);

                TempData["ReviewSuccessMessage"] = "Ревюто беше добавено успешно.";
                return RedirectToAction("Details", "Employee", new { id = model.EmployeeId });
            }
            catch (NullReferenceException nex)
            {
                logger.LogError($"An error occured while adding the review. {nex.Message}");
                return RedirectToAction("Error", "Home", new { code = 404 });
            }
            catch (InvalidOperationException iex)
            {
                logger.LogError($"An error occured while adding the review. {iex.Message}");
                ModelState.AddModelError(string.Empty, iex.Message);

                AddReviewViewModel? refreshedModel =
                    await reviewService.GetAddReviewModelAsync(model.EmployeeId, userId, model.ProcedureId);

                if (refreshedModel != null)
                {
                    model.AvailableProcedures = refreshedModel.AvailableProcedures;
                    model.SpecialistName = refreshedModel.SpecialistName;
                    model.ServiceId = refreshedModel.ServiceId;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occured while adding the review. {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}