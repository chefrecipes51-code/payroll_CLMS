/****************************************************************************************************
 *  Jira Task Ticket :  Payroll-594                                                                 *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for Approval setup.                                     *
 *  It includes APIs to retrieve, create, update, and delete Approval Setup                         *
 *  Author: Chirag Gurjar                                                                           *
 *  Date  : 18-Mar-2025                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payroll.Common.Helpers;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Extensions;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using System.Net.Http;
using UserService.BAL.Models;

namespace Payroll.WebApp.Controllers
{

    public class ApprovalSetUpController : SharedUtilityController
    {

        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        //private readonly BindDropdownDataHelper _dropdownHelper;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
        private readonly RestApiTransactionServiceHelper _transactionServiceHelper;
        private readonly HttpClient _httpClient;
          

        public ApprovalSetUpController(RestApiMasterServiceHelper masterServiceHelper
            , RestApiUserServiceHelper userServiceHelper
            , RestApiTransactionServiceHelper transactionServiceHelper
            , IMapper mapper, IOptions<ApiSettings> apiSettings
            , HttpClient httpClient)
        {
            this._mapper = mapper;
            this._apiSettings = apiSettings.Value;
            //_dropdownHelper = dropdownHelper;
            _userServiceHelper = userServiceHelper;
            _masterServiceHelper = masterServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _httpClient = httpClient;
        }

        [HttpGet("ApprovalSetUp/ApprovalConfigGrid")]
        public async Task<IActionResult> ApprovalConfigGrid(int? configId)
        {
            try
            {
                string queryParams = $"?configId={configId}"; // Used these to allow these parameter for CLMS parameter

                string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetApprovalConfigGridUrl}";
                var apiResponse = await _masterServiceHelper.GetAllRecordsAsync<ApprovalConfigGrid>($"{apiUrl}{queryParams}");

                ViewBag.TotalCount = 0;
                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {

                    var approvalList = apiResponse.Result;
                    ViewBag.TotalCount = approvalList.Count();
                    return View("ApprovalConfigGrid", approvalList);  // 👈 returns View with model
                }

                // Return empty list with error message in TempData if needed
                TempData["Error"] = apiResponse?.Message ?? "Failed to load data.";
                return View("ApprovalConfigGrid", new List<ApprovalConfigGrid>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Internal Server Error: " + ex.Message;
                return View("ApprovalConfigGrid", new List<ApprovalConfigGrid>());
            }
        }


        [HttpGet("ApprovalSetUp/GetApprovalConfig")]
        public async Task<IActionResult> GetApprovalConfig(string configId)
        {
            int? decryptedConfigId = null;
            if (!string.IsNullOrEmpty(configId))
            {
                try
                {
                    string decryptedIdStr = SingleEncryptionHelper.Decrypt(configId);
                    if (int.TryParse(decryptedIdStr, out int parsedConfigId))
                    {
                        decryptedConfigId = parsedConfigId;
                    }
                }
                catch (Exception ex)
                {
                    // return BadRequest("Invalid encrypted company ID");
                }
            }

            try
            {

                string queryParams = $"?configId={decryptedConfigId ?? 0}"; // Used these to allow these parameter for CLMS parameter

                string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetApprovalConfigUrl}";///{companyId}/{locationId}/{serviceId}";
               // var apiResponse = await _masterServiceHelper.GetAllRecordsAsync<ApprovalConfig>($"{apiUrl}{ queryParams}");
                var apiResponse = await _masterServiceHelper.GetAllApiResponseByIdAsync<ApprovalConfigCommon>($"{apiUrl}{queryParams}");

                //var acJson = JsonConvert.SerializeObject(apiResponse.Result);
                //Console.WriteLine(acJson);


                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    return View("GetApprovalConfig", apiResponse.Result); //  returns full view
                }

                TempData["Error"] = apiResponse?.Message ?? "Failed to load data.";
                return View("GetApprovalConfig", new ApprovalConfigCommon
                {
                    Config = new ApprovalConfig(),// <-- initialize the nested object
                    Levels = new List<ApprovalLevel>(),
                    Details = new List<ApprovalDetail>()
                });

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Internal Server Error: " + ex.Message;
                return View("GetApprovalConfig", new ApprovalConfigCommon
                {
                    Config = new ApprovalConfig(),// <-- initialize the nested object
                    Levels = new List<ApprovalLevel>(),
                    Details = new List<ApprovalDetail>()
                });
            }
        }

        [HttpPost("ApprovalSetUp/AddApprovalConfig")]
        public async Task<IActionResult> AddApprovalConfig([FromBody] ApprovalConfigCommon approvalConfig)
        {

            // return Json(new { success = false, message = "" });
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync(); // Get API Key
            try
            {
                // Set CreatedBy field
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
                approvalConfig.Config.CreatedBy = userId;
                approvalConfig.Config.UpdatedBy = userId;


                if (approvalConfig == null ||
             approvalConfig.Config == null ||
              !approvalConfig.Levels.Any() ||
                 !approvalConfig.Details.Any())
                {
                    return Json(new { success = false, message = "Missing required fields." });
                }


                // Define API URL
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostApprovalSetUpUrl;

                // apiUrl = "http://localhost:5265/api/ApprovalSetUpApi/postapprovalsetup";
                // Call the generic PostAsync method to post approval data
                var apiResponse = await _masterServiceHelper
                                    .PostCommonWithKeyAsync<ApprovalConfigCommon, ApprovalConfigCommon>(apiUrl, approvalConfig, apiKey);

                // Handle response
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = "Approval details added successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public async Task<IActionResult> DeleteApprovalMatrix([FromBody] ApprovalConfig model)
        {

            // Set UpdatedBy from session
            model.UpdatedBy = SessionUserId;

            // Construct API URL
            var deleteApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.DeleteApprovalMatrix}/{model.ConfigID}";

            // Call the common delete method (now sending the request body)
            var deleteResponse = await _masterServiceHelper.DeleteCommonAsync<ApprovalConfig, ApprovalConfig>(deleteApiUrl, model);

            if (deleteResponse != null && deleteResponse.IsSuccess)
            {
                return Json(new { success = true, message = deleteResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = deleteResponse.Message ?? "Failed to delete the approval." });
            }


        }







































        [HttpPost]
        public async Task<IActionResult> add([FromBody] ApprovalSetUp approvalSetup)
        {

            try
            {
                // Set CreatedBy field
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
                approvalSetup.CreatedBy = userId;
                approvalSetup.UpdatedBy = userId;

                // Define API URL
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostApprovalSetUpUrl;

                // apiUrl = "http://localhost:5265/api/ApprovalSetUpApi/postapprovalsetup";
                // Call the generic PostAsync method to post approval data
                var apiResponse = await _userServiceHelper
                                    .PostCommonAsync<ApprovalSetUp, ApprovalSetUp>(apiUrl, approvalSetup);

                // Handle response
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = "Approval details added successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public IActionResult Test()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Index2()
        {
            return View();
        }

        [HttpGet]
        public IActionResult add()
        {
            return View();
        }

        [HttpGet]
        [Route("ApprovalSetUp/[action]/{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            try
            {

                string apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetServiceByIdUrl}/{id}";
                var apiResponse = await _userServiceHelper.GetByIdCommonAsync<ServiceMaster>(apiUrl, id);


                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    //var service = _mapper.Map<ServiceMaster>(apiResponse.Result);
                    var service = apiResponse.Result;
                    return Json(new { IsSuccess = true, Result = service });
                }

                return Json(new { IsSuccess = false, Message = apiResponse.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("ApprovalSetUp/[action]/{id}")]
        public async Task<IActionResult> GetApprovalSetUpByServiceId(int id)
        {
            try
            {

                string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetApprovalSetUpUrl}/{id}";
                var apiResponse = await _masterServiceHelper.GetAllRecordsAsync<ApprovalSetUpFilter>(apiUrl);


                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    //var service = _mapper.Map<ServiceMaster>(apiResponse.Result);
                    var approvalSetUp = apiResponse.Result;
                    return Json(new { IsSuccess = true, Result = approvalSetUp });
                }

                return Json(new { IsSuccess = false, Message = apiResponse.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }



    }
}
