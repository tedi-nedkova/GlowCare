using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Controllers
{
    public class ReviewController(

        ) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
