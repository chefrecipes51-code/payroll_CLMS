using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;

namespace Payroll.WebApp.Controllers
{
    public class BreadcrumbController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly RestApiUserServiceHelper _userServiceHelper;

        public BreadcrumbController(IOptions<ApiSettings> apiSettings, RestApiUserServiceHelper userServiceHelper)
        {
            _apiSettings = apiSettings.Value;
            _userServiceHelper = userServiceHelper;
        }

        [HttpPost]
        public IActionResult SetMenuId(int menuId)
        {
            HttpContext.Session.SetInt32("ActiveMenuId", menuId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetBreadcrumb()
        {
            int? menuId = HttpContext.Session.GetInt32("ActiveMenuId");
            if (menuId == null)
                return PartialView("BreadcrumbPartial", new List<BreadCrumbDTO>());

            string url = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetBreadcrumbByMenuIdUrl}?menu_Id={menuId}";

            var apiResponse = await _userServiceHelper.GetUserRoleMenuAsync(url); // You may need a different method here if `GetUserRoleMenuAsync` is specific.

            var breadcrumbList = JsonConvert.DeserializeObject<IEnumerable<BreadCrumbDTO>>(JsonConvert.SerializeObject(apiResponse.Result));
            return PartialView("BreadcrumbPartial", breadcrumbList);
        }
    }
}
