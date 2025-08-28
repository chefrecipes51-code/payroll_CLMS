using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.Helpers;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;

namespace Payroll.WebApp.Controllers
{
    public class GeneralAccountController : SharedUtilityController
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
        public GeneralAccountController(IConfiguration config,  RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings)
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
            var (isSuccess, accoungHeadList, errorMessage) = await FetchAllAccountingHeadAsync(); 

            if (isSuccess)
            {
                return View(accoungHeadList);
            }
            else
            {
                return View(new List<AccountingHeadRequest>());
            }
        }
        public IActionResult AddUpdateAccountMaster()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetGeneralAccountByEncryptedId(string generalAccountId)
        {
            var (isSuccess, result, error) = await FetchGeneralAccountByEncryptedIdAsync(generalAccountId);

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
        public async Task<IActionResult> AddGeneralAccount([FromBody] AccountingHeadRequest accountHead)
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

                if (accountHead.Accounting_Head_Id > 0)
                {
                    return await UpdateGeneralAccountAsync(accountHead, apiKey);
                }
                else
                {
                    accountHead.IsActive = true;
                    return await CreateGeneralAccountAsync(accountHead, apiKey);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> DeleteGeneralAccount([FromBody] AccountingHeadRequest model)
        {
            model.UpdatedBy = SessionUserId;
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            if (apiKey != null)
            {
                var deleteApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.DeleteAccountingHeadDetailUrl}/{model.Accounting_Head_Id}";

                var deleteResponse = await _masterServiceHelper.DeleteCommonWithKeyAsync<AccountingHeadRequest, AccountingHeadRequest>(deleteApiUrl, model, apiKey);

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
        private async Task<(bool isSuccess, AccountingHeadRequest? result, string? errorMessage)> FetchGeneralAccountByEncryptedIdAsync(string? accHeadId)
        {
            if (!string.IsNullOrEmpty(accHeadId))
            {
                try
                {
                    string decryptedIdStr = SingleEncryptionHelper.Decrypt(accHeadId);
                    if (int.TryParse(decryptedIdStr, out int parsedaccHeadId))
                    {
                        int decryptedaccHeadId = parsedaccHeadId;

                        var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                        if (apiKey != null)
                        {
                            string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetAccountingHeadDetailByIdUrl}/{decryptedaccHeadId}";
                            var apiResponse = await _masterServiceHelper.GetByIdCommonWithKeyAsync<AccountingHeadRequest>(apiUrl, decryptedaccHeadId, apiKey);

                            if (apiResponse.IsSuccess && apiResponse.Result != null)
                            {
                                return (true, apiResponse.Result, null);
                            }
                            else
                            {
                                return (false, null, apiResponse.Message ?? "Failed to load Account Head.");
                            }
                        }
                        else
                        {
                            return (false, null, "API Key generation failed.");
                        }
                    }
                    else
                    {
                        return (false, null, "Invalid Accounting Head Request ID.");
                    }
                }
                catch
                {
                    return (false, null, "Invalid Accounting Head Request ID.");
                }
            }
            return (false, null, "No Accounting Head ID provided.");
        }
        private async Task<IActionResult> CreateGeneralAccountAsync(AccountingHeadRequest accountHead, string apiKey)
        {
            var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostAccountingHeadDetailUrl;
            var apiResponse = await _masterServiceHelper.PostSingleCommonWithKeyAsync(apiUrl, accountHead, apiKey);

            if (apiResponse.IsSuccess)
            {

                return Json(new { success = true, message = apiResponse.Message, resultGeneralAccount = apiResponse.Result });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task<IActionResult> UpdateGeneralAccountAsync(AccountingHeadRequest accountHead, string apiKey)
        {
            accountHead.UpdatedBy = SessionUserId;
            var apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.PutAccountingHeadDetailUrl}/{accountHead.Accounting_Head_Id}";

            var apiResponse = await _masterServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, accountHead, apiKey);

            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task<(bool isSuccess, List<AccountingHeadRequest> result, string? errorMessage)> FetchAllAccountingHeadAsync()
        {
            try
            {
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync(); // Get API Key

                if (apiKey != null)
                {
                    string apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAccountingHeadDetailUrl;

                    // Fetching the Accounting Head list from the API
                    var apiResponse = await _masterServiceHelper.GetListWithKeyAsync<AccountingHeadRequest>(apiUrl, apiKey);

                    if (apiResponse != null && apiResponse.Any())
                    {
                        return (true, apiResponse, null); // Return the AccountingHead list
                    }
                    else
                    {
                        return (false, new List<AccountingHeadRequest>(), "No Accounting Head found.");
                    }
                }
                else
                {
                    return (false, new List<AccountingHeadRequest>(), "API Key generation failed.");
                }
            }
            catch (Exception ex)
            {
                return (false, new List<AccountingHeadRequest>(), $"Error fetching Accounting Head: {ex.Message}");
            }
        }
        #endregion
    }
}
