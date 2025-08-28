using AutoMapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;

namespace Payroll.WebApp.Controllers
{
    public class ContractorValidationController : Controller
    {
        #region CTOR
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        private readonly RestApiTransactionServiceHelper _transactionServiceHelper;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
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
        public ContractorValidationController(RestApiTransactionServiceHelper transactionServiceHelper, RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _userServiceHelper = userServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _masterServiceHelper = masterServiceHelper;
        }
		
		#endregion

		#region Contractor [FIRST TAB]
		public async Task<IActionResult> Index()
        {
			await SetUserPermissions();
			ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ContractorFilter([FromBody] ContractorValidationRequestModel request)
        {
            ContractorValidationRequestModel model = new ContractorValidationRequestModel
            {
                CompanyId = request.CompanyId,
                LocationIds = request.LocationIds,
                WorkOrderIds = request.WorkOrderIds,                
                Month_Id = request.Month_Id, 
                Year = request.Year           
            };
            HttpContext.Session.SetString("ContractorValidationSession", JsonConvert.SerializeObject(model));
            var (isSuccess, contractorList, errorMessage) = await FetchAllContractorsAsync(model);
            if (!isSuccess)
            {
                return Json(new { success = false, message = errorMessage });
            }
            return Json(new { success = true, data = contractorList });
        }

        [HttpGet]
        public async Task<IActionResult> GetContractorValidationData()
        {
            ContractorValidationRequestModel model = new ContractorValidationRequestModel
            {
                CompanyId = (byte)SessionCompanyId,
                LocationIds = null,
                WorkOrderIds = null,
                Month_Id = 0,
                Year=0
            };
            // Store in session as JSON
            HttpContext.Session.SetString("ContractorValidationSession", JsonConvert.SerializeObject(model));
            var (isSuccess, contractorList, errorMessage) = await FetchAllContractorsAsync(model);
            if (!isSuccess)
            {
                return Json(new { success = false, message = errorMessage });
            }
            return Json(new { success = true, data = contractorList });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSelectedContractors([FromBody] List<int> contractorIds)
        {
            try
            {
                ValidateContractorRequest obj = new ValidateContractorRequest();
                obj.ContractorIds = contractorIds;
                obj.CompanyId = SessionUserId;
                if (contractorIds == null || !contractorIds.Any())
                    return BadRequest("No contractor IDs provided.");
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }
                return await UpdateContractorMissingParaAsync(obj, apiKey);
            }
            catch (Exception ex)
            {
                return Ok(new { success = true, message = "Contractors processed successfully." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetGradeConfigDropdown()
        {
            var sessionData = HttpContext.Session.GetString("ContractorValidationSession");

            if (string.IsNullOrEmpty(sessionData))
                return Json(new { success = false, message = "Session expired. Please reapply filters." });

            try
            {
                var model = JsonConvert.DeserializeObject<ContractorValidationRequestModel>(sessionData);
                int companyId = model.CompanyId;

                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                if (apiKey != null)
                {
                    string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayGradeConfigUrl}/{companyId}";
                    var apiResponse = await _transactionServiceHelper.GetByIdCommonForContractorAsync<IEnumerable<PayGradeConfigDTO>>(apiUrl, companyId, apiKey);
                    //var apiResponse = await _transactionServiceHelper.GetByIdListCommonAsync<IEnumerable<PayGradeConfigDTO>>(apiUrl, companyId, apiKey);

                    if (apiResponse.IsSuccess && apiResponse.Result != null)
                    {
                        var dropdownItems = apiResponse.Result
                            .Select(x => new
                            {
                                Id = x.PayGradeConfig_Id,
                                Name = x.GradeConfigName
                            }).ToList();

                        return Json(new { success = true, result = dropdownItems });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Unable to fetch grade configuration from API." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "API key generation failed." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Unable to fetch grade configuration from API." });
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetPreviousMonthYearPeriod(int companyId)
        {
            try
            {
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (string.IsNullOrEmpty(apiKey))
                {
                    return Json(new { isSuccess = false, message = "API Key generation failed." });
                }

                string apiUrl = _apiSettings.PayrollTransactionEndpoints.GetPreviousMonthYearCompanyUrl;

                // Append companyId to URL as query param
                var fullUrl = $"{apiUrl}?companyId={companyId}";

                var apiResponse = await _transactionServiceHelper
                    .GetWithKeyAsync<CompanyPreviousMonthYearRequest>(fullUrl, apiKey);

                if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Result != null)
                {
                    return Json(new { isSuccess = true, data = apiResponse.Result });
                }
                else
                {
                    return Json(new { isSuccess = false, message = apiResponse?.Message ?? "No data found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = $"Error fetching previous month/year: {ex.Message}" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> BindPreviousMonthYearAtPageLoad()
        {
            try
            {
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (string.IsNullOrEmpty(apiKey))
                {
                    return Json(new { isSuccess = false, message = "API Key generation failed." });
                }

                string apiUrl = _apiSettings.PayrollTransactionEndpoints.GetPreviousMonthYearCompanyUrl;

                // Append companyId to URL as query param
                var fullUrl = $"{apiUrl}?companyId={SessionCompanyId}";

                var apiResponse = await _transactionServiceHelper
                    .GetWithKeyAsync<CompanyPreviousMonthYearRequest>(fullUrl, apiKey);

                if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Result != null)
                {
                    ContractorValidationRequestModel model = new ContractorValidationRequestModel
                    {
                        CompanyId = (byte)SessionCompanyId,
                        LocationIds = null,
                        WorkOrderIds = null,
                        Month_Id = apiResponse.Result.Month_Id,
                        Year = apiResponse.Result.Year
                    };
                    // Store in session as JSON
                    HttpContext.Session.SetString("ContractorValidationSession", JsonConvert.SerializeObject(model));
                    return Json(new { isSuccess = true, data = apiResponse.Result });
                }
                else
                {
                    return Json(new { isSuccess = false, message = apiResponse?.Message ?? "No data found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = $"Error fetching previous month/year: {ex.Message}" });
            }
        }
        #endregion

        #region Entity [SECOND TAB]
        [HttpPost]
        public async Task<IActionResult> GetEntityValidationData([FromBody] EntityValidationRequestModel model)
        {
            try
            {
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                if (apiKey == null)
                {
                    return Json(new { success = false, message = "API Key generation failed." });
                }

                string apiUrl = _apiSettings.PayrollTransactionEndpoints.GetContractorEntityUrl;
                model.CompanyId = (byte)SessionCompanyId;

                var apiResponse = await _transactionServiceHelper
                    .PostCommonWithKeyAsync<EntityValidationRequestModel, List<EntityValidationRequest>>(apiUrl, model, apiKey);

                if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Result != null && apiResponse.Result.Any())
                {
                    return Json(new { success = true, data = apiResponse.Result });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse?.Message ?? "No Entity validation data found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error fetching entity data: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSelectedEntities([FromBody] EntityUpdateRequest request)
        {
            var sessionData = HttpContext.Session.GetString("ContractorValidationSession");
            if (string.IsNullOrEmpty(sessionData))
                return Json(new { success = false, message = "Session expired. Please reapply filters." });
            try
            {
                var model = JsonConvert.DeserializeObject<ContractorValidationRequestModel>(sessionData);
                int companyId = model.CompanyId;
                request.UpdatedBy = SessionUserId;
                //UPDATE ENTITY HERE 
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }
                request.CompanyId = (byte)companyId;
                var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.UpdateValidateEntityUrl}";
                var apiResponse = await _transactionServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, request, apiKey);
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while submitting entity data." });
            }
        }

        #endregion

        #region Pay Calculation [Third Tab]
        [HttpPost]
        public async Task<IActionResult> SelectedEntitiesForPayValidation([FromBody] List<int> entityIds)
        {
            var sessionData = HttpContext.Session.GetString("ContractorValidationSession");
            if (entityIds == null || !entityIds.Any())
                return Json(new { success = false, message = "No entities selected." });

            try
            {

                var model = JsonConvert.DeserializeObject<ContractorValidationRequestModel>(sessionData);
                int companyId = model.CompanyId;

                var payload = new EntityPayValidationRequestModel
                {
                    CompanyId = (byte)companyId,
                    EntityIds = entityIds
                };

                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                if (string.IsNullOrEmpty(apiKey))
                    return Json(new { success = false, message = "API key generation failed." });

                var apiUrl = _apiSettings.PayrollTransactionEndpoints.GetPayCalculationUrl;

                var apiResponse = await _transactionServiceHelper
                    .PostCommonWithKeyAsync<EntityPayValidationRequestModel, List<EntityPayValidationRequest>>(apiUrl, payload, apiKey);

                if (apiResponse != null && apiResponse.IsSuccess)
                {
                    return Json(new { success = true, data = apiResponse.Result });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse?.Message ?? "No data received from Pay Calculation API." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred while validating entity pay structure." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalaryConfigDropdown()
        {
            var sessionData = HttpContext.Session.GetString("ContractorValidationSession");

            if (string.IsNullOrEmpty(sessionData))
                return Json(new { success = false, message = "Session expired. Please reapply filters." });

            try
            {
                var model = JsonConvert.DeserializeObject<ContractorValidationRequestModel>(sessionData);
                int companyId = model.CompanyId;
                string queryParams = $"/{companyId}";
                string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetSalaryStructureGridUrl}";
                var apiResponse = await _masterServiceHelper.GetAllRecordsAsync<SalaryStructureGrid>($"{apiUrl}{queryParams}");
               
                if (apiResponse.IsSuccess && apiResponse.Result != null)
                {
                    var dropdownItems = apiResponse.Result
                      .Select(x => new
                      {
                          Id = x.SalaryStructure_Hdr_Id,
                          Name = x.SalaryStructureName
                      }).ToList();

                    return Json(new { success = true, result = dropdownItems });
                }
                else
                {
                    return Json(new { success = false, message = "Unable to fetch salary configuration from API." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Unable to fetch salary configuration from API." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSelectedPayCal([FromBody] PayCalculationUpdateRequest request)
        {
            var sessionData = HttpContext.Session.GetString("ContractorValidationSession");
            if (string.IsNullOrEmpty(sessionData))
                return Json(new { success = false, message = "Session expired. Please reapply filters." });
            try
            {
                var model = JsonConvert.DeserializeObject<ContractorValidationRequestModel>(sessionData);
                int companyId = model.CompanyId;
                request.UpdatedBy = SessionUserId;
                //UPDATE ENTITY HERE 
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }
                request.CompanyId = (byte)companyId;
                var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.UpdateValidatePayCalUrl}";
                var apiResponse = await _transactionServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, request, apiKey);
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while submitting entity data." });
            }
        }
        #endregion

        #region Compliance [Fourth Tab]
        [HttpPost]
        public async Task<IActionResult> SelectedEntitiesForComplianceValidation([FromBody] List<int> entityIds)
        {
            var sessionData = HttpContext.Session.GetString("ContractorValidationSession");

            if (string.IsNullOrEmpty(sessionData))
                return Json(new { success = false, message = "Session expired. Please reapply filters." });

            if (entityIds == null || !entityIds.Any())
                return Json(new { success = false, message = "No entities selected." });

            try
            {
                var model = JsonConvert.DeserializeObject<ContractorValidationRequestModel>(sessionData);
                int companyId = model.CompanyId;

                var payload = new ComplianceValidationRequest
                {
                    CompanyId = companyId,
                    EntityIds = entityIds
                };

                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                if (string.IsNullOrEmpty(apiKey))
                    return Json(new { success = false, message = "API key generation failed." });

                var apiUrl = _apiSettings.PayrollTransactionEndpoints.GetComplianceUrl;

                var apiResponse = await _transactionServiceHelper
                    .PostCommonWithKeyAsync<ComplianceValidationRequest, List<EntityComplianceValidationRequest>>(apiUrl, payload, apiKey);

                if (apiResponse != null && apiResponse.IsSuccess)
                {
                    return Json(new { success = true, data = apiResponse.Result });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse?.Message ?? "No data received from Compliance API." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred while fetching compliance data." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSelectedCompliance([FromBody] EntityComplianceUpdateRequest request)
        {
            var sessionData = HttpContext.Session.GetString("ContractorValidationSession");

            if (string.IsNullOrEmpty(sessionData))
                return Json(new { success = false, message = "Session expired. Please reapply filters." });

            try
            {
                var model = JsonConvert.DeserializeObject<ContractorValidationRequestModel>(sessionData);
                int companyId = model.CompanyId;

                // API key
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (string.IsNullOrEmpty(apiKey))
                {
                    return Json(new { success = false, message = "API key generation failed." });
                }

                // Assign company ID to the request
                request.CompanyId = companyId;
                request.UpdatedBy = SessionUserId;
               // Call the API
                var apiUrl = _apiSettings.PayrollTransactionEndpoints.UpdateValidatecomplianceUrl;

                var apiResponse = await _transactionServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, request, apiKey);

                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while submitting compliance data." });
            }
        }
        #endregion

        #region Attendance [Fifth Tab]
        [HttpPost]
        public async Task<IActionResult> SelectedEntitiesForAttendanceValidation([FromBody] List<int> entityIds)
        {
            var sessionData = HttpContext.Session.GetString("ContractorValidationSession");

            if (string.IsNullOrEmpty(sessionData))
                return Json(new { success = false, message = "Session expired. Please reapply filters." });

            if (entityIds == null || !entityIds.Any())
                return Json(new { success = false, message = "No entities selected." });

            try
            {
                var model = JsonConvert.DeserializeObject<ContractorValidationRequestModel>(sessionData);
                int companyId = model.CompanyId;

                var payload = new ComplianceValidationRequest
                {
                    CompanyId = companyId,
                    EntityIds = entityIds,
                    PayrollMonth= model.Month_Id,
                    PayrollYear= model.Year
                };

                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                if (string.IsNullOrEmpty(apiKey))
                    return Json(new { success = false, message = "API key generation failed." });

                var apiUrl = _apiSettings.PayrollTransactionEndpoints.GetAttendanceUrl;

                var apiResponse = await _transactionServiceHelper
                    .PostCommonWithKeyAsync<ComplianceValidationRequest, List<EntityAttendanceRequest>>(apiUrl, payload, apiKey);

                if (apiResponse != null && apiResponse.IsSuccess)
                {
                    return Json(new { success = true, data = apiResponse.Result });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse?.Message ?? "No data received from Attendance API." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred while fetching attendance data." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SubmitSelectedAttendance([FromBody] EntityAttendanceUpdateRequest request)
        {
            var sessionData = HttpContext.Session.GetString("ContractorValidationSession");

            if (string.IsNullOrEmpty(sessionData))
                return Json(new { success = false, message = "Session expired. Please reapply filters." });

            try
            {
                var model = JsonConvert.DeserializeObject<ContractorValidationRequestModel>(sessionData);
                int companyId = model.CompanyId;

                // API key
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (string.IsNullOrEmpty(apiKey))
                {
                    return Json(new { success = false, message = "API key generation failed." });
                }

                // Set company ID and updated by
                request.CompanyId = companyId;
                request.UpdatedBy = SessionUserId;

                // Call the Attendance API
                var apiUrl = _apiSettings.PayrollTransactionEndpoints.UpdateValidateAttendanceUrl;

                var apiResponse = await _transactionServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, request, apiKey);

                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while submitting attendance data." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveAttendanceValidation([FromBody] List<AttendanceSaveModel> attendanceList)
        {
            var sessionData = HttpContext.Session.GetString("ContractorValidationSession");

            var model = JsonConvert.DeserializeObject<ContractorValidationRequestModel>(sessionData);

            if (attendanceList == null || !attendanceList.Any())
            {
                return Json(new { success = false, message = "No data received." });
            }

            if (string.IsNullOrEmpty(sessionData))
                return Json(new { success = false, message = "Session expired or not found." });

            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            if (string.IsNullOrEmpty(apiKey))
            {
                return Json(new { success = false, message = "API key generation failed." });
            }
            var apiUrl = _apiSettings.PayrollTransactionEndpoints.AddPayrollTranStgDataUrl;

            var stagingRequest = new SavePayrollStagingRequestModel
            {
                CreatedBy = SessionUserId,
                Month_Id = model.Month_Id,
                Year_Id = model.Year,
                PayrollData = attendanceList
                      .Select(x => new PayrollStgData
                      {
                          Contractor_ID = x.ContractorId,
                          Entity_ID = x.EntityId
                      })
                        .ToList()
            };
            var apiResponse = await _transactionServiceHelper
                                                    .PostSingleCommonWithKeyAsync(apiUrl, stagingRequest, apiKey);
           
            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }

        #endregion

        #region Private Method 
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }
        private async Task<(bool isSuccess, List<ContractorValidationRequest> result, string? errorMessage)> FetchAllContractorsAsync(ContractorValidationRequestModel model)
        {
            try
            {
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                if (apiKey != null)
                {
                    string apiUrl = _apiSettings.PayrollTransactionEndpoints.GetContractorsUrl;

                    // Pass model to API
                    var apiResponse = await _transactionServiceHelper
                        .PostCommonWithKeyAsync<ContractorValidationRequestModel, List<ContractorValidationRequest>>(apiUrl, model, apiKey);

                    if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Result != null)
                    {
                        return (true, apiResponse.Result.ToList(), null);
                    }
                    else
                    {
                        return (false, new List<ContractorValidationRequest>(), apiResponse?.Message ?? "No Contractor found.");
                    }
                }
                else
                {
                    return (false, new List<ContractorValidationRequest>(), "API Key generation failed.");
                }
            }
            catch (Exception ex)
            {
                return (false, new List<ContractorValidationRequest>(), $"Error fetching Contractor: {ex.Message}");
            }
        }
        private async Task<IActionResult> UpdateContractorMissingParaAsync(ValidateContractorRequest conDetails, string apiKey)
        {
            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.UpdateValidateContractorsUrl}";
            conDetails.UpdatedBy = SessionUserId;
            var apiResponse = await _transactionServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, conDetails, apiKey);

            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        #endregion
    }
}
