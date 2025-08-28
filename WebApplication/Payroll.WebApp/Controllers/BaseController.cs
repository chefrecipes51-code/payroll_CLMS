using Payroll.WebApp.Common;
using System.Drawing.Imaging;
using Payroll.Common.Helpers;
using Payroll.WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Payroll.WebApp.Models;

namespace Payroll.WebApp.Controllers
{
    public class BaseController : Controller
    {
        private readonly ICompositeViewEngine _viewEngine;
        public BaseController(ICompositeViewEngine viewEngine)
        {
            this._viewEngine = viewEngine;
        }

        #region  Base Redirection Page Code

        public IActionResult PageNotFound()
        {
            return View();
        }

        public IActionResult InternalServerError()
        {
            return View();
        }

        public IActionResult ForbiddenError()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View("_AccessDenied");
        }

        #endregion
        

        #region Common

        public string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;

            using (var stringWriter = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                if (!viewResult.Success)
                {
                    throw new InvalidOperationException($"View '{viewName}' not found.");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    stringWriter,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        [HttpPost]
        public IActionResult GetTabIdAndRestrictedUrls()
        {
            var tabId = Guid.NewGuid().ToString();

            Response.Cookies.Append(
                "UniquePayrollTabId",
                tabId,
                new CookieOptions
                {
                    Path = "/",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = true
                }
            );
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/config/RestrictedUrls.json");
            var jsonData = System.IO.File.ReadAllText(jsonPath);
            var restrictedUrls = JsonConvert.DeserializeObject<RestrictedUrlModel>(jsonData);

            return Json(new
            {
                tabId,
                restrictedUrls = restrictedUrls?.RestrictedUrls ?? new List<string>()
            });
        }
        #endregion
    }
}
