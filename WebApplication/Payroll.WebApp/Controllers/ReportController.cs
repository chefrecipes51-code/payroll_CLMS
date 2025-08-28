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
    public class ReportController : Controller
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
        public ReportController(RestApiUserServiceHelper userServiceHelper, RestApiMasterServiceHelper masterServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings, RestApiTransactionServiceHelper transactionServiceHelper)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _masterServiceHelper = masterServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _userServiceHelper = userServiceHelper;
        }

        #region Pay Register 
        public IActionResult PayRegisterReport()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> FetchPayRegisterReport([FromBody] RegisterReportDTO dto)
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

        #endregion

        #region LOSS AND DAMAGE
        public IActionResult LossDamageRegisterData()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> FetchLossDamageRegisterReport([FromBody] CommonFilterReportDTO dto)
        {
            try
            {
                var filter = new CommonFilterReportDTO
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
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetLossDamageReportUrl;
                var response = await _masterServiceHelper.PostCommonAsync<CommonFilterReportDTO, IEnumerable<LossDamageRegisterReport>>(apiUrl, filter);
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
        #endregion

        #region TAX Deduction 
        public IActionResult TaxDeductionReport()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GetTaxDeductionReport([FromBody] CommonFilterReportDTO dto)
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

                var filter = new CommonFilterReportDTO
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
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetTaxDeductionReportUrl;
                var response = await _masterServiceHelper
                    .PostCommonAsync<CommonFilterReportDTO, IEnumerable<TaxDeductionReport>>(apiUrl, filter);
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
        #endregion

        #region Contractor Payment Register

        public IActionResult ContractorPaymentRegisterReport()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetContractorPaymentRegister([FromBody] CommonFilterReportDTO dto)
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

                var filter = new CommonFilterReportDTO
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

                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetContractorPaymentRegisterUrl;

                var response = await _masterServiceHelper
                    .PostCommonAsync<CommonFilterReportDTO, IEnumerable<ContractorPaymentRegisterReport>>(apiUrl, filter);

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

        #endregion

    }
}
