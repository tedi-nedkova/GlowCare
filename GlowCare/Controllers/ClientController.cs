using Microsoft.AspNetCore.Mvc;

namespace GlowCare.Controllers
{
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
