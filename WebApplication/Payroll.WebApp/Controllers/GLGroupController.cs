using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.Helpers;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Requests;

namespace Payroll.WebApp.Controllers
{
    public class GLGroupController : SharedUtilityController
    {
        #region CTOR
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
        private readonly IConfiguration _configuration;
        private int SessionUserId
        {
            get
            {
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                return int.TryParse(sessionData?.UserId, out var parsedUserId) ? parsedUserId : 0;
            }
        }
        private int SessionCompanyId
        {
            get
            {
                var sessionCompanyData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
                return sessionCompanyData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
            }
        }
        public GLGroupController(IConfiguration config, RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _userServiceHelper = userServiceHelper;
            _masterServiceHelper = masterServiceHelper;
            _configuration = config;
        }
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }
        #endregion
        public async Task<IActionResult> Index()
        {
            var (isSuccess, glList, errorMessage) = await FetchAllGLGroupsAsync();

            if (isSuccess)
            {
                return View(glList);
            }
            else
            {
                return View(new List<GlGroupRequest>());
            }
        }
        public IActionResult AddUpdateGLGroup()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetGLGroupByEncryptedId(string glgroupId)
        {
            var (isSuccess, result, error) = await FetchGLGroupByEncryptedIdAsync(glgroupId);

            if (isSuccess && result != null)
            {
                return Json(result);
            }
            else
            {
                return Json(new { success = false, message = error ?? "Something went wrong." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddGLGroup([FromBody] GlGroupRequest accountHead)
        {
            if (accountHead == null)
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
                accountHead.CreatedBy = SessionUserId;
                accountHead.CreatedDate = DateTime.Now;
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }

                if (accountHead.GL_Group_Id > 0)
                {
                    return await UpdateGLGroupAsync(accountHead, apiKey);
                }
                else
                {
                    accountHead.IsActive = true;
                    return await CreatGLGroupAsync(accountHead, apiKey);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        public async Task<IActionResult> DeleteGLGroup([FromBody] GlGroupRequest model)
        {
            model.UpdatedBy = SessionUserId;
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            if (apiKey != null)
            {
                var deleteApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.DeleteGLGroupUrl}/{model.GL_Group_Id}";
                var deleteResponse = await _masterServiceHelper.DeleteCommonWithKeyAsync<GlGroupRequest, GlGroupRequest>(deleteApiUrl, model, apiKey);
                if (deleteResponse != null && deleteResponse.IsSuccess)
                {
                    return Json(new { success = true, message = deleteResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = deleteResponse.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "API Key generation failed." });
            }
        }
        #region Private Method 
        private async Task<IActionResult> CreatGLGroupAsync(GlGroupRequest glgroup, string apiKey)
        {
            var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostGLGroupUrl;
            var apiResponse = await _masterServiceHelper.PostSingleCommonWithKeyAsync(apiUrl, glgroup, apiKey);

            if (apiResponse.IsSuccess)
            {

                return Json(new { success = true, message = apiResponse.Message, resultGeneralAccount = apiResponse.Result });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task<IActionResult> UpdateGLGroupAsync(GlGroupRequest glgroup, string apiKey)
        {
            glgroup.UpdatedBy = SessionUserId;
            var apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.PutGLGroupUrl}/{glgroup.GL_Group_Id}";

            var apiResponse = await _masterServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, glgroup, apiKey);

            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task<(bool isSuccess, GlGroupRequest? result, string? errorMessage)> FetchGLGroupByEncryptedIdAsync(string? glgroupID)
        {
            if (!string.IsNullOrEmpty(glgroupID))
            {
                try
                {
                    string decryptedIdStr = SingleEncryptionHelper.Decrypt(glgroupID);
                    if (int.TryParse(decryptedIdStr, out int parsedglgroupid))
                    {
                        int decryptedaccHeadId = parsedglgroupid;

                        var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                        if (apiKey != null)
                        {
                            string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetGLGroupByIdUrl}/{decryptedaccHeadId}";
                            var apiResponse = await _masterServiceHelper.GetByIdCommonWithKeyAsync<GlGroupRequest>(apiUrl, decryptedaccHeadId, apiKey);

                            if (apiResponse.IsSuccess && apiResponse.Result != null)
                            {
                                return (true, apiResponse.Result, null);
                            }
                            else
                            {
                                return (false, null, apiResponse.Message ?? "Failed to load GL Group .");
                            }
                        }
                        else
                        {
                            return (false, null, "API Key generation failed.");
                        }
                    }
                    else
                    {
                        return (false, null, "Invalid GL Group Request ID.");
                    }
                }
                catch
                {
                    return (false, null, "Invalid GL Group Request ID.");
                }
            }
            return (false, null, "No GL Group ID provided.");
        }
        private async Task<(bool isSuccess, List<GlGroupRequest> result, string? errorMessage)> FetchAllGLGroupsAsync()
        {
            try
            {
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                if (string.IsNullOrEmpty(apiKey))
                    return (false, new List<GlGroupRequest>(), "API Key generation failed.");

                string apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetGLGroupUrl;

                var apiResponse = await _masterServiceHelper.GetListWithKeyAsync<GlGroupRequest>(apiUrl, apiKey);

                if (apiResponse != null && apiResponse.Any())
                {
                    return (true, apiResponse, null);
                }
                else
                {
                    return (false, new List<GlGroupRequest>(), "No GL Group records found.");
                }
            }
            catch (Exception ex)
            {
                return (false, new List<GlGroupRequest>(), $"Error fetching GL Groups: {ex.Message}");
            }
        }
        #endregion
    }
}
