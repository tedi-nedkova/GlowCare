using GlowCare.Core.Contracts;
using GlowCare.Core.Implementations;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Models;
using GlowCare.ViewModels.Procedures;
using GlowCare.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GlowCare.Controllers
{
    public class HomeController(
       IProcedureService procedureService,
       IServiceService serviceService,
       ILogger<HomeController> _logger
        ) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Employees = await procedureService.GetEmployeeSelectListAsync();


            var model = new IndexViewModel
            {
                ServicesInfo = await serviceService.GetAllServicesAsync()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
