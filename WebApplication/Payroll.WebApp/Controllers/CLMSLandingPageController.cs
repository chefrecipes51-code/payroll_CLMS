using Microsoft.AspNetCore.Mvc;

namespace Payroll.WebApp.Controllers
{
    public class CLMSLandingPageController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
