using Microsoft.AspNetCore.Mvc;

namespace Payroll.WebApp.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
