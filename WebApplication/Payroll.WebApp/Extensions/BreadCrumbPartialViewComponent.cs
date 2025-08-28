using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.ApplicationModel;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;

namespace Payroll.WebApp.Extensions
{
    [ViewComponent(Name = "BreadCrumbPartial")]
    public class BreadCrumbPartialViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;

        public BreadCrumbPartialViewComponent(
            IHttpContextAccessor httpContextAccessor,
            RestApiUserServiceHelper userServiceHelper,
            IMapper mapper,
            IOptions<ApiSettings> apiSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _userServiceHelper = userServiceHelper;
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int? menuId = _httpContextAccessor.HttpContext.Session.GetInt32("SelectedMenuId");
            if (menuId == null)
            {
                return View("~/Views/Shared/Components/BreadCrumbs/BreadCrumbPartial.cshtml", new List<BreadCrumbDTO>()); // return empty breadcrumb
            }

            string apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetBreadcrumbByMenuIdUrl}?menu_Id={menuId}";
            var apiResponse = await _userServiceHelper.GetAsync<ApiResponseModel<IEnumerable<BreadCrumbDTO>>>(apiUrl);
            if (apiResponse?.IsSuccess == true)
            {
                // Map to DTO for the view (if needed)
                var breadcrumbDtoList = apiResponse.Result.Select(b => new BreadCrumbDTO
                {
                    Breadcrumb_Id = b.Breadcrumb_Id,
                    MenuGroup_Id =b.MenuGroup_Id,
                    Title = b.Title,
                    Action_URL = b.Action_URL,
                    DisplayOrder = b.DisplayOrder,
                    IsActive = b.IsActive,
                }).OrderBy(b => b.DisplayOrder).ToList();

                return View("~/Views/Shared/Components/BreadCrumbs/BreadCrumbPartial.cshtml", breadcrumbDtoList);
            }
            return View("~/Views/Shared/Components/BreadCrumbs/BreadCrumbPartial.cshtml", new List<BreadCrumbDTO>()); // fallback empty breadcrumb
        }
    }
}
