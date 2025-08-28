/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-362                                                                  *
 *  Description:   NavMenuViewComponent                                                             *
                *      These classes help to bind User Menu                                         *                                   
 *                                                                                                  *
 *  Author: Harshida Parmar                                                                         *
 *  Date  : 10-Jan-'25                                                                              *
 *                                                                                                  *
 ****************************************************************************************************/
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.ApplicationModel;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollTransactionService.BAL.Models;
using System.Net.Http;
using UserService.BAL.Requests;

namespace Payroll.WebApp.Extensions
{
    [ViewComponent(Name = "MenuPartial")]
    public class NavMenuViewComponent : ViewComponent
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        public NavMenuViewComponent(RestApiUserServiceHelper userServiceHelper, IHttpContextAccessor httpContextAccessor, IOptions<ApiSettings> apiSettings, IMapper mapper)
        {
            _apiSettings = apiSettings.Value;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userServiceHelper = userServiceHelper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var isUserMenuSet = _httpContextAccessor.HttpContext.Session.GetString("isUserMenuSet");

            if (string.IsNullOrEmpty(isUserMenuSet) || isUserMenuSet == "false")
            {
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                var sessionUserData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
                
                // Extract companyId, roleId, and userId
                int companyId = sessionUserData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
               

                #region Manage LocationId
                int? selectedLocationId = HttpContext.Session.GetInt32("SelectedLocationId");
                int isLocationChanges = HttpContext.Session.GetInt32("IsLocationChanges") ?? 0;
                int isDefaultLocationChanges = HttpContext.Session.GetInt32("IsDefaultLocationChanges") ?? 0;

                //int userMapLocation_Id = sessionUserData.LocationDetails.FirstOrDefault(r => r.Default_Location)?.UserMapLocation_Id ?? 0;
                int userMapLocation_Id = sessionUserData.LocationDetails.FirstOrDefault(r => r.Default_Location)?.UserMapLocation_Id ?? 0;

                int finalLocationId;

                if (isLocationChanges == 1)
                {
                    finalLocationId = selectedLocationId ?? userMapLocation_Id;
                }
                else if (isDefaultLocationChanges == 1)
                {
                    finalLocationId = userMapLocation_Id;
                }
                else
                {
                    // First-time login case
                    finalLocationId = userMapLocation_Id;
                }
                #endregion
                #region Manage RoleId
                int? selectedRoleId = HttpContext.Session.GetInt32("SelectedRoleId");
                int isRoleChanges = HttpContext.Session.GetInt32("IsRoleChanges") ?? 0;
                int isDefaultRoleChanges = HttpContext.Session.GetInt32("IsDefaultRoleChanges") ?? 0;               
                int userRole_Id = sessionUserData.RoleDetails.FirstOrDefault(r => r.IsDefault_Role)?.Role_Id ?? 0;
                int finalRoleId;

                if (isRoleChanges == 1)
                {
                    finalRoleId = selectedRoleId ?? userRole_Id;
                }
                else if (isDefaultRoleChanges == 1)
                {
                    finalRoleId = userRole_Id;
                }
                else
                {
                    finalRoleId = userRole_Id;
                }
                #endregion
                int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;

                string serviceNameApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetUserRoleMenuUrl;
                string queryParams = $"?companyid={companyId}&roleid={finalRoleId}&userid={userId}&userMapLocationId={finalLocationId}";

                //var apiResponse = await RestApiUserServiceHelper.Instance.GetUserRoleMenuAsync($"{serviceNameApiUrl}{queryParams}");
                var apiResponse = await _userServiceHelper.GetUserRoleMenuAsync($"{serviceNameApiUrl}{queryParams}");
                if (apiResponse?.Result != null)
                {
                    IEnumerable<UserRoleBasedMenuDTO> menuItems = _mapper.Map<IEnumerable<UserRoleBasedMenuDTO>>(apiResponse.Result);
                    return await Task.FromResult((IViewComponentResult)View("MenuPartial", menuItems));
                }
            }
            return await Task.FromResult((IViewComponentResult)View("MenuPartial", isUserMenuSet));
        }
    }

}
