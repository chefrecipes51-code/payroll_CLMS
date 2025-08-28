using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.WebApp.Helpers;

namespace Payroll.WebApp.Controllers
{
    public class SetGetAPIUrlController : Controller
    {
        private readonly ApiSettings _apiSettings;

        // Inject IOptions<ApiSettings> to access the configuration
        public SetGetAPIUrlController(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        public IActionResult Index()
        {
            string authUrl = _apiSettings.UserAuthServiceEndpoints.AuthUrl;  
            string getAllUserUrl = _apiSettings.UserAuthServiceEndpoints.GetAllUserUrl;

            string payrollDataUrl = _apiSettings.PayrollMasterServiceEndpoints.GetPayrollDataUrl;  

            ViewBag.AuthUrl = authUrl;
            ViewBag.PayrollDataUrl = payrollDataUrl;


            // Do something with the URLs (like passing them to the view)
            return View();
        }
    }
}
