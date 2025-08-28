using Microsoft.AspNetCore.Mvc;

namespace Payroll.WebApp.Controllers.WageComponent
{
    public class WageGradeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
