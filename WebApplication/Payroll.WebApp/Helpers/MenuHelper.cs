/****************************************************************************************************
 *  Created By : Chirag Gurjar                                                                      *
 *  Date  : 04-mar-2025                                                                             *
 *  Task  : Payroll-494 Mechanism to grant page level rights.                                       *
 *              Use to get menu data, and store in session so next time get that from session       *
 ****************************************************************************************************/
using AutoMapper;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Models.DTOs;

namespace Payroll.WebApp.Helpers
{
    public static class MenuHelper
    {      
        public static async Task<IEnumerable<UserRoleBasedMenuDTO>> GetUserMenus(
            HttpContext httpContext,
            RestApiUserServiceHelper userServiceHelper, 
            IMapper mapper, 
            ApiSettings apiSettings)
        {
            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(httpContext, "UserSessionData");
            var sessionUserData = SessionHelper.GetSessionObject<UserSessionViewModel>(httpContext, "UserDetailData");

            if (sessionData == null)
            {
                return new List<UserRoleBasedMenuDTO>(); // Return empty list if session is null
            }

            // Extract companyId, roleId, and userId
            int companyId = sessionUserData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
            int roleId = sessionUserData.RoleDetails.FirstOrDefault(r => r.IsDefault_Role)?.Role_Id ?? 0;
            int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;

            //int companyId = 32; // Get from sessionData dynamically
            //int roleId = 1;
            //int userId = 79;

            // Check session first
            var menuItems = SessionHelper.GetSessionObject<IEnumerable<UserRoleBasedMenuDTO>>(httpContext, "UserMenus");
            string storedRoleIdStr = httpContext.Session.GetString("UserRoleId");
            int storedRoleId = 0;

            if (!string.IsNullOrEmpty(storedRoleIdStr))
            {
                int.TryParse(storedRoleIdStr, out storedRoleId);
            }

            if (menuItems != null && menuItems.Any() && storedRoleId == roleId) //check for same roleid
            {
                return menuItems;
            }
            #region Manage LocationId
            int? selectedLocationId = httpContext.Session.GetInt32("SelectedLocationId");
            int isLocationChanges = httpContext.Session.GetInt32("IsLocationChanges") ?? 0;
            int isDefaultLocationChanges = httpContext.Session.GetInt32("IsDefaultLocationChanges") ?? 0;

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
            int? selectedRoleId = httpContext.Session.GetInt32("SelectedRoleId");
            int isRoleChanges = httpContext.Session.GetInt32("IsRoleChanges") ?? 0;
            int isDefaultRoleChanges = httpContext.Session.GetInt32("IsDefaultRoleChanges") ?? 0;
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
            // Fetch from API if session is empty
            string serviceNameApiUrl = apiSettings.BaseUrlPayrollUserServiceEndpoints.GetUserRoleMenuUrl;
            //string queryParams = $"?companyid={companyId}&roleid={roleId}&userid={userId}";
            string queryParams = $"?companyid={companyId}&roleid={finalRoleId}&userid={userId}&userMapLocationId={finalLocationId}";

            var apiResponse = await userServiceHelper.GetUserRoleMenuAsync($"{serviceNameApiUrl}{queryParams}");

            if (apiResponse?.Result != null)
            {
                menuItems = mapper.Map<IEnumerable<UserRoleBasedMenuDTO>>(apiResponse.Result);

                // Clear previous session data before storing new role-specific data
                httpContext.Session.Remove("UserMenus");
                httpContext.Session.Remove("UserRoleId");


                // Store the new menu and associated roleId in session
                SessionHelper.SetSessionObject(httpContext, "UserMenus", menuItems);
                SessionHelper.SetSessionObject(httpContext, "UserRoleId", roleId);
            }

            return menuItems ?? new List<UserRoleBasedMenuDTO>();
        }

        //public static bool HasAccess(HttpContext httpContext, string currentUrl, IEnumerable<UserRoleBasedMenuDTO> menuItems)
        //{
        //    if (menuItems == null || !menuItems.Any())
        //    {
        //        return false;
        //    }

        //    currentUrl = currentUrl.TrimStart('/').ToLower();
        //    return menuItems.Any(m => m.ActionUrl?.ToLower() == currentUrl);
        //}
    }
}
