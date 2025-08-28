using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models;
using System.Diagnostics;
using Payroll.WebApp.Extensions;
using System.Security.Principal;

namespace Payroll.WebApp.Controllers
{
   // [UserAuthorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //[UserAuthorize]
        public IActionResult Index()
        {
            //Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            //Response.Headers["Pragma"] = "no-cache";
            //Response.Headers["Expires"] = "-1";
            #region Added By Harshida 23-12-'24
            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");

            if (sessionData.UserId != null || sessionData.UserId != null)
            {
                return View(sessionData); // Pass session data to the view directly
            }

            // If session data is not available, return a default view or error message
            return RedirectToAction("LoginPage", "Account");

            #endregion           
        }

        //public IActionResult Index()
        //{          
        //    return View();
        //}
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult DashboardV2()
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
