using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Reviews;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Controllers
{
    public class ReviewController(
        IReviewService reviewService,
        ILogger<ReviewController> logger,
        UserManager<GlowUser> userManager) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Add(int procedureId)
        {
            var review = new AddReviewViewModel();

            review.ProcedureId = procedureId;

            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = Guid.Parse(userManager.GetUserId(User));

                await reviewService.CreateReviewAsync(model, userId);

                return RedirectToAction("Details", "Service");
            }
            catch (NullReferenceException nex)
            {
                logger.LogError($"An error occured while adding the review. {nex.Message}");
                return RedirectToAction("Error", "Home", new { code = 404 });
            }
            catch (InvalidOperationException iex)
            {
                logger.LogError($"An error occured while adding the review. {iex.Message}");
                return RedirectToAction("Error", "Home", new { code = 500 });
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occured while adding the review. {ex.Message}");

                return RedirectToAction("Error", "Home");
            }
        }

    }
}
