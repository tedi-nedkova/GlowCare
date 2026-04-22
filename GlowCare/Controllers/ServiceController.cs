using GlowCare.Core.Contracts;
using GlowCare.ViewModels.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Controllers
{
    public class ServiceController(
        IServiceService serviceService
    ) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(
            int? selectedCategoryId,
            string? selectedPriceRange,
            string? selectedAvailabilityRange,
            int page = 1)
        {
            const int pageSize = 6;

            var filteredServices = (await serviceService.GetFilteredServicesAsync(
                selectedCategoryId,
                selectedPriceRange,
                selectedAvailabilityRange)).ToList();

            var paginatedServices = filteredServices
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new ServiceIndexViewModel
            {
                SelectedCategoryId = selectedCategoryId,
                SelectedPriceRange = selectedPriceRange,
                SelectedAvailabilityRange = selectedAvailabilityRange,
                Categories = await serviceService.GetCategoryOptionsAsync(),
                Services = paginatedServices
            };

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)filteredServices.Count / pageSize);

            return View(model);
        }
    }
}