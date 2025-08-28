using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using payrollmasterservice.BAL.Models;
using PayrollMasterService.BAL.Models;

namespace Payroll.WebApp.Controllers
{
    public class RegisterWagesReportController : SharedUtilityController
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
            }
        }
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }
        public RegisterWagesReportController(RestApiUserServiceHelper userServiceHelper, RestApiMasterServiceHelper masterServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings, RestApiTransactionServiceHelper transactionServiceHelper)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _masterServiceHelper = masterServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _userServiceHelper = userServiceHelper;
        }

        public IActionResult Index()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> FetchSalaryReport([FromBody] RegisterReportDTO dto)
        {
            try
            {
                // 1. Map DTO to Filter object
                var filter = new PayRegisterFilter
                {
                    CompanyID = dto.companyID,
                    ContractorIDs = dto.contractorIDs,
                    PayrollMonth = dto.payrollMonth,
                    PayrollYear = dto.payrollYear,
                    FinancialYearStart = DateTime.TryParse(dto.financialYearStart, out var fyStart) ? fyStart : (DateTime?)null
                };

                // 2. Set API URL
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetPayRegisterReportUrl;

                // 3. Call your internal API
                var response = await _masterServiceHelper.PostCommonAsync<PayRegisterFilter, IEnumerable<PayRegisterReport>>(apiUrl, filter);

                // 4. Return data if success
                if (response != null && response.IsSuccess && response.Result != null)
                {
                    return Json(response.Result);
                }
                else
                {
                    return Json(new { isSuccess = false, message = response?.Message ?? "Failed to fetch report." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = $"Error: {ex.Message}" });
            }
        }

        public IActionResult FineRegisterReport()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GetFineRegisterReport([FromBody] FinePayRegisterReportDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = "Failed to fetch report."
                    });
                }
                // 1. Map DTO to Filter object expected by the API
                var filter = new FineRegisterFilter
                {
                    CompanyID = dto.CompanyID,
                    CompanyLocationIDs = dto.CompanyLocationIDs,
                    ContractorIDs = dto.ContractorIDs,
                    EntityIDs = dto.EntityIDs,
                    PayrollMonth = dto.PayrollMonth,
                    PayrollYear = dto.PayrollYear,
                    ProcessedDate = dto.ProcessedDate,
                    FinancialYearStart = dto.FinancialYearStart
                };

                // 2. Set API URL (Ensure it ends with /getfineregisterreport or mapped in your config)
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetFineRegisterReportUrl;

                // 3. Call API via helper method
                var response = await _masterServiceHelper
                    .PostCommonAsync<FineRegisterFilter, IEnumerable<FineRegisterReport>>(apiUrl, filter);

                // 4. Handle response
                if (response != null && response.IsSuccess && response.Result != null)
                {
                    return Json(new { isSuccess = true, data = response.Result });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = response?.Message ?? "Failed to fetch report."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = $"Error: {ex.Message}" });
            }
        }

        public IActionResult SalarySlipReport()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetSalarySlipReport([FromBody] SalarySlipReportDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = "Failed to fetch report."
                    });
                }
                // 1. Map DTO to Filter object expected by the API
                var filter = new SalarySlipFilter
                {
                    CompanyID = dto.CompanyID,
                    CompanyLocationIDs = dto.CompanyLocationIDs,
                    ContractorIDs = dto.ContractorIDs,
                    EntityIDs = dto.EntityIDs,
                    PayrollMonth = dto.PayrollMonth,
                    PayrollYear = dto.PayrollYear,
                };

                // 2. Set API URL (Ensure it ends with /getfineregisterreport or mapped in your config)
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetSalarySlipReportUrl;

                // 3. Call API via helper method
                var response = await _masterServiceHelper
                    .PostCommonAsync<SalarySlipFilter, IEnumerable<SalarySlipReport>>(apiUrl, filter);

                // 4. Handle response
                if (response != null && response.IsSuccess && response.Result != null)
                {
                    return Json(new { isSuccess = true, data = response.Result });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = response?.Message ?? "Failed to fetch report."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = $"Error: {ex.Message}" });
            }
        }

        public IActionResult OvertimeReport()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetOvertimeReport([FromBody] OvertimeReportDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = "Failed to fetch report."
                    });
                }
                // 1. Map DTO to Filter object expected by the API
                var filter = new OvertimeFilter
                {
                    CompanyID = dto.CompanyID,
                    CompanyLocationIDs = dto.CompanyLocationIDs,
                    ContractorIDs = dto.ContractorIDs,
                    EntityIDs = dto.EntityIDs,
                    PayrollMonth = dto.PayrollMonth,
                    PayrollYear = dto.PayrollYear,
                    ProcessedDate = dto.ProcessedDate,
                    FinancialYearStart = dto.FinancialYearStart
                };

                // 2. Set API URL (Ensure it ends with /getfineregisterreport or mapped in your config)
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetOvertimeReportUrl;

                // 3. Call API via helper method
                var response = await _masterServiceHelper
                    .PostCommonAsync<OvertimeFilter, IEnumerable<OvertimeReport>>(apiUrl, filter);

                // 4. Handle response
                if (response != null && response.IsSuccess && response.Result != null)
                {
                    return Json(new { isSuccess = true, data = response.Result });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = response?.Message ?? "Failed to fetch report."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = $"Error: {ex.Message}" });
            }
        }

        public IActionResult ComplianceReport()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }

        public async Task<JsonResult> GetComplianceReport([FromBody] ComplianceReportDTO dto)
        {
            try
            {
                // 1. Map DTO to Filter object expected by the API
                var filter = new ComplianceFilter
                {
                    CompanyID = dto.CompanyID,
                    CompanyLocationIDs = dto.CompanyLocationIDs,
                    ContractorIDs = dto.ContractorIDs,
                    EntityIDs = dto.EntityIDs,
                    PayrollMonth = dto.PayrollMonth,
                    PayrollYear = dto.PayrollYear,

                };

                // 2. Set API URL (Ensure it ends with /getcompliancereport or mapped in your config)
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetComplianceReportUrl;

                // 3. Call API via helper method
                var response = await _masterServiceHelper
                    .PostCommonAsync<ComplianceFilter, IEnumerable<ComplianceReport>>(apiUrl, filter);

                // 4. Handle response
                if (response != null && response.IsSuccess && response.Result != null)
                {
                    return Json(new { isSuccess = true, data = response.Result });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = response?.Message ?? "Failed to fetch report."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = $"Error: {ex.Message}" });
            }
        }

        public IActionResult LoanandAdvanceReport()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetLoanandAdvanceReport([FromBody] LoanandAdvanceReportDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = "Failed to fetch report."
                    });
                }
                // 1. Map DTO to Filter object expected by the API
                var filter = new LoanandAdvanceFilter
                {
                    CompanyID = dto.CompanyID,
                    CompanyLocationIDs = dto.CompanyLocationIDs,
                    ContractorIDs = dto.ContractorIDs,
                    EntityIDs = dto.EntityIDs,
                    PayrollMonth = dto.PayrollMonth,
                    PayrollYear = dto.PayrollYear,
                    ProcessedDate = dto.ProcessedDate,
                    FinancialYearStart = dto.FinancialYearStart
                };

                // 2. Set API URL (Ensure it ends with /getfineregisterreport or mapped in your config)
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetLoanandAdvanceReportUrl;

                // 3. Call API via helper method
                var response = await _masterServiceHelper
                    .PostCommonAsync<LoanandAdvanceFilter, IEnumerable<LoanandAdvanceReport>>(apiUrl, filter);

                // 4. Handle response
                if (response != null && response.IsSuccess && response.Result != null)
                {
                    return Json(new { isSuccess = true, data = response.Result });
                }
                else
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = response?.Message ?? "Failed to fetch report."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = $"Error: {ex.Message}" });
            }
        }

    }
}
