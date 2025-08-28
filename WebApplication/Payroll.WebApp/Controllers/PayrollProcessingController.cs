using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using Payroll.WebApp.SignalRHubs;
using PayrollTransactionService.BAL.Models;

namespace Payroll.WebApp.Controllers
{
    public class PayrollProcessingController : SharedUtilityController
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
        private readonly RestApiTransactionServiceHelper _transactionServiceHelper;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        //private readonly IBackgroundTaskQueue _repository;
        private readonly IHubContext<NotificationHub> _hubContext;
        // Property to get UserId from Session
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
                //var sessionCompanyData = SessionHelper.GetSessionObject<UserCompanyDetails>(HttpContext, "UserSessionData");
                var sessionCompanyData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
                // Extract companyId, roleId, and userId
                return sessionCompanyData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
            }
        }
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }
        public PayrollProcessingController(RestApiUserServiceHelper userServiceHelper, RestApiMasterServiceHelper masterServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings, RestApiTransactionServiceHelper transactionServiceHelper, /*IBackgroundTaskQueue repository,*/ IHubContext<NotificationHub> hubContext)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _masterServiceHelper = masterServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _userServiceHelper = userServiceHelper;
            //_repository = repository;
            _hubContext = hubContext;
        }
        public IActionResult PayrollProcess()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }
        public IActionResult PayrollProcessList()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePayrollTransDataForProcess([FromBody] PayrollTransDataForProcessDTO payrollTransDataForProcessDTO)
        {
            await SetUserPermissions();
            if (payrollTransDataForProcessDTO == null)
            {
                return Json(new { success = false, message = "Invalid data. Please check your input." });
            }
            try
            {
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                payrollTransDataForProcessDTO.UpdatedBy = SessionUserId;
                payrollTransDataForProcessDTO.UpdatedDate = DateTime.Now;
                // Assuming you are updating the area using a service or repository
                var updateApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.PostPayrollTransDataForProcessDetailUrl}";
                var updateResponse = await _transactionServiceHelper.PostCommonWithKeyAsync<PayrollTransDataForProcessDTO, PayrollTransDataForProcess>(updateApiUrl, payrollTransDataForProcessDTO, apikey);

                if (updateResponse.IsSuccess)
                {
                    return Json(new { success = 1, message = updateResponse.Message, UpdatedRecords = updateResponse.Result?.UpdatedRecords ?? 0 });
                }
                else
                {
                    return Json(new { success = false, message = updateResponse.Message ?? "Failed to update the data." });
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception details
                Console.WriteLine($"Exception in Update: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the process." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostStartPayrollProcess([FromBody] StartPayrollProcessDTO dto)
        {
            await SetUserPermissions();

            if (dto == null)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            dto.CreatedBy = SessionUserId;
            //dto.Process_Sequence_Id = 1;
            //dto.Payroll_Process_Id = 0;

            if (dto.Mode == 2)
            {
                HttpContext.Session.SetInt32("PayrollProcessMode", 2);
            }

            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostStartPayrollProcessDetailUrl;

            var mappedModel = new StartPayrollProcess
            {
                Cmp_Id = dto.Cmp_Id,
                Month_Id = dto.Month_Id,
                Year_Id = dto.Year_Id,
                CreatedBy = dto.CreatedBy,
                Process_Sequence_Id = dto.Process_Sequence_Id,
                Payroll_Process_Id = dto.Payroll_Process_Id,
                SignalRConnectionId = dto.SignalRConnectionId
            };

            var apiResponse = await _transactionServiceHelper.PostCommonWithKeyAsync<StartPayrollProcess, StartPayrollProcess>(
                apiUrl, mappedModel, apiKey);

            var result = apiResponse?.Result;

            if (apiResponse?.IsSuccess == true && result != null)
            {
                return Json(new
                {
                    success = true,
                    message = apiResponse.Message,
                    processId = result.Payroll_Process_Id,
                    payrollHeaderId = result.Payroll_header_Id,
                    processSequenceId = result.Process_Sequence_Id
                });
            }

            return Json(new { success = false, message = apiResponse?.Message ?? "API call failed." });
        }
        [HttpPost]
        public async Task<IActionResult> PostProcessPayrollEmployees([FromBody] PayrollProcessRequestModelDTO startPayrollProcessDTO)
        {
            await SetUserPermissions();

            if (startPayrollProcessDTO == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid data. Please check your input."
                });
            }

            try
            {
                startPayrollProcessDTO.CreatedBy = SessionUserId;

                // Generate API Key for downstream service
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                // Prepare API endpoint
                var endpointUrl = _apiSettings.PayrollTransactionEndpoints.PostStartPayrollProcessDetailUrl;

                // Call the downstream service API
                var response = await _transactionServiceHelper.PostCommonWithKeyAsync<PayrollProcessRequestModelDTO, PayrollProcessResultModel>(
                    endpointUrl,
                    startPayrollProcessDTO,
                    apiKey
                );

                // Evaluate response
                if (response != null && response.IsSuccess)
                {
                    return Json(new
                    {
                        success = true,
                        message = response.Message,
                        processId = response.Result?.PayrollProcessId ?? 0,
                        Data = response.Result
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = response?.Message ?? "Failed to process payroll employees."
                    });
                }
            }
            catch (Exception ex)
            {
                // Ideally log the exception here (e.g., _logger.LogError(ex, "..."))
                Console.WriteLine($"Exception in PostProcessPayrollEmployees: {ex.Message}");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while processing the payroll request."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPayrollProcessusingSignalR([FromBody] PayrollProcessusingSignalRRequest payrollProcessusingSignalRRequest)
        {
            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayrollProcessusingSignalRDetail}?cmp_Id={payrollProcessusingSignalRRequest.Cmp_Id}&month_Id={payrollProcessusingSignalRRequest.Month_Id}&year_Id={payrollProcessusingSignalRRequest.Year_Id}";

            var response = await _transactionServiceHelper.GetCommonAsync<List<PayrollProcessusingSignalRDTO>>(
                apiUrl
            );

            var responseList = response?.Result;

            if (responseList == null || !responseList.Any())
            {
                return Json(new { success = false, message = "No data found." });

            }
            return Json(new { success = true, data = response.Result });

        }

        [HttpPost]
        public async Task<IActionResult> GetPhaseWiseComponentPayrollProcess(
            [FromBody] PhaseWiseComponentPayrollProcessDTO model)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetPhaseWiseComponentPayrollProcessDetail}";

            var apiResponse = await _transactionServiceHelper.PostPayrollProcessCommonWithKeyAsync<PhaseWiseComponentPayrollProcessDTO, List<Dictionary<string, object>>>(apiUrl, model, apiKey);

            var responseList = apiResponse?.Result;

            if (!apiResponse.IsSuccess || responseList == null || !responseList.Any())
            {
                return Json(new { success = false, message = apiResponse?.Message ?? "No data found." });
            }

            return Json(new { success = true, data = responseList });
        }


    }
}
