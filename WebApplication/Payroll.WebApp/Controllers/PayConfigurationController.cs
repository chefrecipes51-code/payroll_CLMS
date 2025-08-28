using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.Extensions.Options;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;
using static Payroll.WebApp.Helpers.SessionHelper;
using Payroll.WebApp.Extensions;

namespace Payroll.WebApp.Controllers
{
    [ServiceFilter(typeof(MenuAuthorizationFilter))]
    public class PayConfigurationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
        private readonly RestApiTransactionServiceHelper _transactionServiceHelper;
        private readonly RestApiUserServiceHelper _userServiceHelper;
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

                //return sessionCompanyData?.Company_Id ?? 0;
            }
        }
        private int SessionRoleId
        {
            get
            {
                var sessionCompanyData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
                return sessionCompanyData.RoleDetails.FirstOrDefault()?.Role_Id ?? 0;
            }
        }
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }
        public PayConfigurationController(RestApiUserServiceHelper userServiceHelper, RestApiMasterServiceHelper masterServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings, RestApiTransactionServiceHelper transactionServiceHelper)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _masterServiceHelper = masterServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _userServiceHelper = userServiceHelper;
        }

        #region Pay Component Functionality
        public async Task<IActionResult> PayComponents()
        {
            await SetUserPermissions();
            var response = new ApiResponseModel<PayComponentDTO> { IsSuccess = false };
            try
            {
                var Company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayComponentsUrl}/{Company_Id}";
                var apiResponse = await _transactionServiceHelper.GetByIdCommonAsync<IEnumerable<PayComponentMaster>>(apiUrl, apikey);
                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    IEnumerable<PayComponentDTO> payComponentDTOs = _mapper.Map<IEnumerable<PayComponentDTO>>(apiResponse.Result);
                    ViewBag.PayComponentCount = payComponentDTOs.Count();
                    return View(payComponentDTOs);
                }
                else
                {
                    ViewBag.PayComponentCount = 0;
                    return View(new List<PayComponentDTO>());
                }

            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ApiResponseModel<PayComponentDTO>
                {
                    IsSuccess = false,
                    Message = MessageConstants.TechnicalIssue,
                    StatusCode = ApiResponseStatusConstant.InternalServerError
                });
            }
        }

        public async Task<IActionResult> PayComponentList()
        {
            await SetUserPermissions();
            var response = new ApiResponseModel<PayComponentDTO> { IsSuccess = false };
            try
            {
                var Company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayComponentsUrl}/{Company_Id}";
                var apiResponse = await _transactionServiceHelper.GetByIdCommonAsync<IEnumerable<PayComponentMaster>>(apiUrl, apikey);
                if (apiResponse?.Result == null)
                {
                    return Json(new
                    {
                        html = await this.RenderViewAsync("_PayComponentListPartialView", new List<PayComponentDTO>(), true),
                        count = 0
                    });
                }

                var dtoList = apiResponse.Result.Select(p => new PayComponentDTO
                {
                    EarningDeduction_Id = p.EarningDeduction_Id,
                    Company_Id = p.Company_Id,
                    EarningDeductionType = p.EarningDeductionType,
                    EarningDeductionName = p.EarningDeductionName,
                    FormulaName = p.FormulaName,
                    CalculationType = p.CalculationType,
                    EarningDeductionTypeName = p.EarningDeductionTypeName,
                    CalculationTypeName = p.CalculationTypeName,
                    MinimumUnit_value = p.MinimumUnit_value,
                    MaximumUnit_value = p.MaximumUnit_value,
                    Amount = p.Amount,
                    Is_Child = p.Is_Child,
                    IsChild = p.IsChild,
                    Parent_EarningDeduction_Id = p.Parent_EarningDeduction_Id,
                    IsEditable = p.IsEditable,
                    IsActive = p.IsActive
                }).ToList();
                ViewBag.PayComponentCount = dtoList.Count;
                var html = await this.RenderViewAsync("_PayComponentListPartialView", dtoList, true);

                return Json(new
                {
                    html = html,
                    count = dtoList.Count
                });
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });
            }
            catch (Exception ex)
            {
                // Consider logging the error
                return Json(new
                {
                    html = await this.RenderViewAsync("_PayComponentListPartialView", new List<PayComponentDTO>(), true),
                    count = 0
                });
            }
        }

        [Route("PayConfiguration/[action]/{Id}")]
        [HttpGet]
        public async Task<IActionResult> GetPayComponentDetailsById([FromRoute] int Id)
        {
            try
            {
                int company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                var getApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetByIdPayComponentsUrl}/{Id}";
                var response = await _transactionServiceHelper.GetByIdCommonAsync<PayComponentDTO>(getApiUrl, apikey);

                if (!response.IsSuccess || response.Result == null)
                {
                    return Json(new { success = false, message = response.Message });
                }
                return Json(new { success = true, data = response.Result });
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });
            }
            catch (Exception ex)
            {
                // Log error
                return Json(new { success = false, message = "An error occurred while fetching area details." });
            }
        }

        [Route("PayConfiguration/[action]/{Id}")]
        [HttpGet]
        public async Task<IActionResult> GetPaySubComponentDetailsById([FromRoute] int Id)
        {
            try
            {
                int company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                var getApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetByIdPaySubComponentsUrl}/{Id}";
                var response = await _transactionServiceHelper.GetByIdCommonAsync<PayComponentDTO>(getApiUrl, apikey);

                if (!response.IsSuccess || response.Result == null)
                {
                    return Json(new { success = false, message = response.Message });
                }
                return Json(new { success = true, data = response.Result });
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });
            }
            catch (Exception ex)
            {
                // Log error
                return Json(new { success = false, message = "An error occurred while fetching area details." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPayComponents([FromBody] PayComponentDTO payComponentDTO)
        {
            await SetUserPermissions();
            if (payComponentDTO == null || string.IsNullOrWhiteSpace(payComponentDTO.EarningDeductionName))
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                payComponentDTO.Company_Id = SessionCompanyId;
                payComponentDTO.CreatedBy = SessionUserId; // Assuming this is hardcoded or passed from session/user context
                payComponentDTO.CreatedDate = DateTime.Now;
                payComponentDTO.IsActive = true;

                var payComponentRequest = _mapper.Map<PayComponentMaster>(payComponentDTO);
                // Define API URL
                var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostPayComponentsUrl;
                // Call the generic PostAsync method to post company data
                var apiResponse = await _transactionServiceHelper
                                    .PostCommonAsync<PayComponentMaster, PayComponentDTO>(apiUrl, payComponentRequest, apikey);
                // Handle response
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message ?? "Record created successfully." });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePayComponents([FromBody] PayComponentDTO payComponentDTO)
        {
            await SetUserPermissions();
            if (payComponentDTO == null || string.IsNullOrWhiteSpace(payComponentDTO.EarningDeductionName))
            {
                return Json(new { success = false, message = "Invalid data. Please check your input." });
            }
            try
            {
                var Company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                payComponentDTO.UpdatedBy = SessionUserId;
                payComponentDTO.UpdatedDate = DateTime.Now;
                payComponentDTO.Company_Id = Company_Id;
                // Assuming you are updating the area using a service or repository
                var updateApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.PutPayComponentsUrl}/{payComponentDTO.EarningDeduction_Id}";
                var updateResponse = await _transactionServiceHelper.PutCommonAsync<PayComponentDTO, PayComponentMaster>(updateApiUrl, payComponentDTO, apikey);

                if (updateResponse.IsSuccess)
                {
                    return Json(new { success = true, message = updateResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = updateResponse.Message ?? "Failed to update the area." });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });
            }
            catch (Exception ex)
            {
                // Optionally log the exception details
                Console.WriteLine($"Exception in UpdateArea: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the area." });
            }
        }

        public async Task<IActionResult> DeletePayComponent([FromBody] PayComponentDTO model)
        {
            try
            {
                await SetUserPermissions();
                // Set UpdatedBy from session
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                model.UpdatedBy = SessionUserId;
                // Construct API URL
                var deleteApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.DeletePayComponentsUrl}/{model.EarningDeduction_Id}";

                // Call the common delete method (sending the request body)
                var deleteResponse = await _transactionServiceHelper.DeleteCommonAsync<PayComponentDTO, PayComponentMaster>(deleteApiUrl, model, apikey);

                // Return appropriate response
                if (deleteResponse != null && deleteResponse.IsSuccess)
                {
                    return Json(new { success = true, message = deleteResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = deleteResponse.Message ?? "Failed to delete the pay component." });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });
            }
        }
        #endregion

        #region Pay Grade Functionality
        public async Task<IActionResult> PayGrade()
        {
            await SetUserPermissions();
            var response = new ApiResponseModel<PayGradeMasterDTO> { IsSuccess = false };
            try
            {
                var Company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayGradeUrl}";
                var apiResponse = await _transactionServiceHelper.GetByIdCommonAsync<IEnumerable<PayGradeMaster>>(apiUrl, apikey);
                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    IEnumerable<PayGradeMasterDTO> payGradeDTOs = _mapper.Map<IEnumerable<PayGradeMasterDTO>>(apiResponse.Result);
                    ViewBag.PayGradeCount = payGradeDTOs.Count();
                    return View(payGradeDTOs);
                }
                else
                {
                    ViewBag.PayGradeCount = 0;
                    return View(new List<PayGradeMasterDTO>());
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return new JsonResult(new ApiResponseModel<PayComponentDTO>
                {
                    IsSuccess = false,
                    Message = MessageConstants.TechnicalIssue,
                    StatusCode = ApiResponseStatusConstant.InternalServerError
                });
            }
        }

        public async Task<IActionResult> PayGradeList()
        {
            await SetUserPermissions();
            var response = new ApiResponseModel<PayGradeMasterDTO> { IsSuccess = false };
            try
            {
                var Company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayGradeUrl}";
                var apiResponse = await _transactionServiceHelper.GetByIdCommonAsync<IEnumerable<PayGradeMaster>>(apiUrl, apikey);
                if (apiResponse?.Result == null)
                {
                    return Json(new
                    {
                        html = await this.RenderViewAsync("_PayGradeListPartialView", new List<PayGradeMasterDTO>(), true),
                        count = 0
                    });
                }

                var payGradeList = apiResponse.Result.Select(p => new PayGradeMasterDTO
                {
                    PayGrade_Id = p.PayGrade_Id,
                    Cmp_Id = p.Cmp_Id,
                    PayGradeCode = p.PayGradeCode,
                    PayGradeName = p.PayGradeName,
                    MinSalary = p.MinSalary,
                    MaxSalary = p.MaxSalary,
                    PayGradeDesc = p.PayGradeDesc,
                    IsActive = p.IsActive
                }).ToList();
                ViewBag.PayGradeCount = payGradeList.Count;
                var html = await this.RenderViewAsync("_PayGradeListPartialView", payGradeList, true);

                return Json(new
                {
                    html = html,
                    count = payGradeList.Count
                });
            }
            catch (Exception ex)
            {
                // You can log the exception here if needed
                return Json(new
                {
                    html = await this.RenderViewAsync("_PayGradeListPartialView", new List<PayGradeMasterDTO>(), true),
                    count = 0
                });
            }
        }

        [Route("PayConfiguration/[action]/{Id}")]
        [HttpGet]
        public async Task<IActionResult> GetPayGradeDetailsById([FromRoute] int Id)
        {
            try
            {
                int company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                var getApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetByIdPayGradesUrl}/{Id}";
                var response = await _transactionServiceHelper.GetByIdCommonAsync<PayGradeMasterDTO>(getApiUrl, apikey);

                if (!response.IsSuccess || response.Result == null)
                {
                    return Json(new { success = false, message = response.Message });
                }
                return Json(new { success = true, data = response.Result });
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                // Log error
                return Json(new { success = false, message = "An error occurred while fetching area details." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPayGrades([FromBody] PayGradeMasterDTO payGradeMasterDTO)
        {
            await SetUserPermissions();
            if (payGradeMasterDTO == null || string.IsNullOrWhiteSpace(payGradeMasterDTO.PayGradeName))
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                payGradeMasterDTO.Cmp_Id = SessionCompanyId;
                payGradeMasterDTO.CreatedBy = SessionUserId; // Assuming this is hardcoded or passed from session/user context
                payGradeMasterDTO.CreatedDate = DateTime.Now;
                payGradeMasterDTO.IsActive = true;

                var payGradeRequest = _mapper.Map<PayGradeMaster>(payGradeMasterDTO);
                // Define API URL
                var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostPayGradesUrl;
                // Call the generic PostAsync method to post company data
                var apiResponse = await _transactionServiceHelper
                                    .PostCommonAsync<PayGradeMaster, PayGradeMasterDTO>(apiUrl, payGradeRequest, apikey);
                // Handle response
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message ?? "Record created successfully." });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePayGrades([FromBody] PayGradeMasterDTO payGradeMasterDTO)
        {
            await SetUserPermissions();
            if (payGradeMasterDTO == null || string.IsNullOrWhiteSpace(payGradeMasterDTO.PayGradeName))
            {
                return Json(new { success = false, message = "Invalid data. Please check your input." });
            }
            try
            {
                var Company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                payGradeMasterDTO.UpdatedBy = SessionUserId;
                payGradeMasterDTO.UpdatedDate = DateTime.Now;
                payGradeMasterDTO.Cmp_Id = Company_Id;
                // Assuming you are updating the area using a service or repository
                var updateApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.PutPayGradesUrl}/{payGradeMasterDTO.PayGrade_Id}";
                var updateResponse = await _transactionServiceHelper.PutCommonAsync<PayGradeMasterDTO, PayGradeMaster>(updateApiUrl, payGradeMasterDTO, apikey);

                if (updateResponse.IsSuccess)
                {
                    return Json(new { success = true, message = updateResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = updateResponse.Message ?? "Failed to update the area." });
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception details
                Console.WriteLine($"Exception in UpdateArea: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the area." });
            }
        }

        public async Task<IActionResult> DeletePayGrade([FromBody] PayGradeMasterDTO model)
        {
            await SetUserPermissions();
            var apikey = await _userServiceHelper.GenerateApiKeyAsync();
            model.UpdatedBy = SessionUserId;
            // Construct API URL
            var deleteApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.DeletePayGreadsUrl}/{model.PayGrade_Id}";

            // Call the common delete method (sending the request body)
            var deleteResponse = await _transactionServiceHelper.DeleteCommonAsync<PayGradeMasterDTO, PayGradeMaster>(deleteApiUrl, model, apikey);

            // Return appropriate response
            if (deleteResponse != null && deleteResponse.IsSuccess)
            {
                return Json(new { success = true, message = deleteResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = deleteResponse.Message ?? "Failed to delete the pay component." });
            }
        }
        #endregion

        #region Pay Grade Mapping Functionality
        public async Task<IActionResult> PayGradeMapping()
        {
            await SetUserPermissions();
            var response = new ApiResponseModel<PayGradeConfigDTO> { IsSuccess = false };
            try
            {
                ViewBag.SessionCompanyId = SessionCompanyId;
                ViewBag.SessionRoleId = SessionRoleId;
                return View(new List<PayGradeConfigDTO>());
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ApiResponseModel<PayComponentDTO>
                {
                    IsSuccess = false,
                    Message = MessageConstants.TechnicalIssue,
                    StatusCode = ApiResponseStatusConstant.InternalServerError
                });
            }
        }

        public async Task<IActionResult> PayGradeMappingList(int id)
        {
            await SetUserPermissions();
            var response = new ApiResponseModel<PayGradeConfigDTO> { IsSuccess = false };
            try
            {
                var Company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayGradeConfigUrl}/{id}";
                var apiResponse = await _transactionServiceHelper.GetByIdCommonAsync<IEnumerable<PayGradeConfigDTO>>(apiUrl, apikey);
                if (apiResponse?.Result == null)
                {
                    return Json(new
                    {
                        html = await this.RenderViewAsync("_PayGradeConfigListPartialView", new List<PayGradeConfigDTO>(), true),
                        count = 0
                    });
                }

                var payGradeConfigList = apiResponse.Result.Select(p => new PayGradeConfigDTO
                {
                    PayGradeConfig_Id = p.PayGradeConfig_Id,
                    Cmp_Id = p.Cmp_Id,
                    GradeConfigName = p.GradeConfigName,
                    PayGrade_Id = p.PayGrade_Id,
                    Trade_Id = p.Trade_Id,
                    SkillType_Id = p.SkillType_Id,
                    PayGradeName = p.PayGradeName,
                    Skillcategory_Name = p.Skillcategory_Name,
                    Trade_Name = p.Trade_Name,
                    EffectiveFrom = p.EffectiveFrom,
                    EffectiveFromStr = p.EffectiveFrom.ToString("yyyy-MM-dd"),
                    EffectiveTo = p.EffectiveTo,
                    EffectiveToStr = p.EffectiveTo.ToString("yyyy-MM-dd"),
                    IsActive = p.IsActive
                }).ToList();
                ViewBag.PayGradeConfigCount = payGradeConfigList.Count;
                var html = await this.RenderViewAsync("_PayGradeConfigListPartialView", payGradeConfigList, true);

                return Json(new
                {
                    html = html,
                    count = payGradeConfigList.Count
                });
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    html = await this.RenderViewAsync("_PayGradeConfigListPartialView", new List<PayGradeConfigDTO>(), true),
                    count = 0
                });
            }
        }

        [Route("PayConfiguration/[action]/{Id}")]
        [HttpGet]
        public async Task<IActionResult> GetPayGradeMappingDetailsById([FromRoute] int Id)
        {
            try
            {
                int company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                var getApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetByIdPayGradeConfigUrl}/{Id}";
                var response = await _transactionServiceHelper.GetByIdCommonAsync<PayGradeConfigMaster>(getApiUrl, apikey);

                if (!response.IsSuccess || response.Result == null)
                {
                    return Json(new { success = false, message = response.Message });
                }
                return Json(new { success = true, data = response.Result });
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });
            }
            catch (Exception ex)
            {
                // Log error
                return Json(new { success = false, message = "An error occurred while fetching area details." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPayGradeMapping([FromBody] PayGradeConfigDTO payGradeConfigDTO)
        {
            await SetUserPermissions();
            if (payGradeConfigDTO == null)
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                //payGradeConfigDTO.Cmp_Id = SessionCompanyId;
                payGradeConfigDTO.CreatedBy = SessionUserId; // Assuming this is hardcoded or passed from session/user context
                payGradeConfigDTO.CreatedDate = DateTime.Now;
                payGradeConfigDTO.IsActive = true;

                var payGradeConfigRequest = _mapper.Map<PayGradeConfigDTO>(payGradeConfigDTO);
                // Define API URL
                var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostPayGradesConfigUrl;
                // Call the generic PostAsync method to post company data
                var apiResponse = await _transactionServiceHelper
                                    .PostCommonAsync<PayGradeConfigDTO, PayGradeConfigMaster>(apiUrl, payGradeConfigRequest, apikey);
                // Handle response
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message ?? "Record created successfully." });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePayGradeMapping([FromBody] PayGradeConfigDTO payGradeConfigDTO)
        {
            await SetUserPermissions();
            if (payGradeConfigDTO == null)
            {
                return Json(new { success = false, message = "Invalid data. Please check your input." });
            }
            try
            {
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                payGradeConfigDTO.UpdatedBy = SessionUserId;
                payGradeConfigDTO.UpdatedDate = DateTime.Now;
                // Assuming you are updating the area using a service or repository
                var updateApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.PutPayGradesConfigUrl}/{payGradeConfigDTO.PayGradeConfig_Id}";
                var updateResponse = await _transactionServiceHelper.PutCommonAsync<PayGradeConfigDTO, PayGradeConfigMaster>(updateApiUrl, payGradeConfigDTO, apikey);

                if (updateResponse.IsSuccess)
                {
                    return Json(new { success = true, message = updateResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = updateResponse.Message ?? "Failed to update the area." });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });
            }
            catch (Exception ex)
            {
                // Optionally log the exception details
                Console.WriteLine($"Exception in UpdateArea: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the area." });
            }
        }

        public async Task<IActionResult> DeletePayGradeMapping([FromBody] PayGradeConfigDTO model)
        {
            try
            {
                await SetUserPermissions();
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                // Set UpdatedBy from session
                model.UpdatedBy = SessionUserId;
                // Construct API URL
                var deleteApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.DeletePayGreadConfigUrl}/{model.PayGradeConfig_Id}";
                // Call the common delete method (sending the request body)
                var deleteResponse = await _transactionServiceHelper.DeleteCommonAsync<PayGradeConfigDTO, PayGradeConfigMaster>(deleteApiUrl, model, apikey);

                // Return appropriate response
                if (deleteResponse != null && deleteResponse.IsSuccess)
                {
                    return Json(new { success = true, message = deleteResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = deleteResponse.Message ?? "Failed to delete the pay component." });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });
            }
        }
        #endregion
    }
}