/// Developed By:- Harshida Parmar
/// Date:- 14-04-2025
/// Note:- Perform all the necessary operation regarding Payroll Month 
///         1) Add Payroll month based on Company
///         2) View Payroll month based on company AND custom Group.

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Extensions;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;
using System.Globalization;
using System.Net;
using UserService.BAL.Models;

namespace Payroll.WebApp.Controllers
{
    //[ServiceFilter(typeof(MenuAuthorizationFilter))]
    public class PayrollPeriodController : SharedUtilityController
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        private readonly RestApiTransactionServiceHelper _transactionServiceHelper;
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
        public PayrollPeriodController(IConfiguration config, RestApiTransactionServiceHelper transactionServiceHelper, RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _userServiceHelper = userServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _masterServiceHelper = masterServiceHelper;
            _configuration = config;
        }
       
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //await SetUserPermissions();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPayMonthBySDate([FromBody] PayrollRequest request)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            if (apiKey != null && request.CompanyId != null)
            {
                string baseUrl = _apiSettings.PayrollTransactionEndpoints.GetPayrollMonthBySdateUrl;
                string dateStr = request.SelectedDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                string apiUrl = $"{baseUrl}?id={request.CompanyId}&createdBy={SessionUserId}";
                if (!string.IsNullOrEmpty(dateStr))
                {
                    apiUrl += $"&fGroupDate={dateStr}";
                }

                //var response = await _transactionServiceHelper.GetListDataAndMessageWithKeyAsync<PeriodRequest>(apiUrl, apiKey);
                var (payPeriods, message) = await _transactionServiceHelper.GetListDataAndMessageWithKeyAsync<PeriodRequest>(apiUrl, apiKey);
                if (payPeriods != null && payPeriods.Any())
                {
                    return Json(new { success = true, result = payPeriods });
                }
                return Json(new { success = false, message = message });
            }

            return Json(new { success = false, message = "API key missing" });
        }
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }
        #region Old Code
        #region Bind Company List
        [HttpGet]
        public async Task<IActionResult> GetCompanyProfilesListJson()
        {
            var companies = await GetCompanyProfilesListAsync();

            var filtered = companies.Select(x => new
            {
                x.Company_Id,
                x.CompanyName
            });

            return Json(filtered);
        }
        private async Task<List<CompanyProfile>> GetCompanyProfilesListAsync()
        {
            var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllCompanyListUrl;
            var companyProfiles = new List<CompanyProfile>();

            try
            {
                var apiResponse = await _masterServiceHelper.GetListAsync<CompanyProfile>(apiUrl);
                if (apiResponse != null && apiResponse.Any())
                {
                    companyProfiles = apiResponse;
                }
            }
            catch (Exception ex)
            {
                // Log error or handle accordingly
                Console.WriteLine("Error fetching companies: " + ex.Message);
            }
            return companyProfiles;
        }
        #endregion
        #region Bind Company Financial Year 
        [HttpGet]
        public async Task<IActionResult> GetcompanyfinYeardetailsbyid(string companyId)
        {
            if (!string.IsNullOrEmpty(companyId))
            {
                string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetCompanyDetailsByIdUrl}/{companyId}";
                var response = await _masterServiceHelper.GetByIdCommonAsync<CompanyFinYear>(apiUrl, Convert.ToInt32(companyId));
                if (!response.IsSuccess || response.Result == null)
                {
                    return Json(new { success = false, message = response.Message });
                }
                return Json(new { success = true, result = response.Result });
            }
            else
            {
                return Json(new { success = false, message = "Company Id Is Null" });
            }
        }
		#endregion
		#region Generate Payroll Month
		[HttpGet]
		public async Task<IActionResult> GeneratePayrollMonth()
		{
			return View();
		}
		[HttpPost]
        public async Task<IActionResult> GeneratePayrollPeriod(string PeriodType, string CompanyId, string PeriodFDate, string PeriodEDate)
        {
            if (string.IsNullOrEmpty(PeriodType) || string.IsNullOrEmpty(CompanyId) || string.IsNullOrEmpty(PeriodFDate) || string.IsNullOrEmpty(PeriodEDate))
            {
                return Json(new { success = false, message = "Please provide all required fields." });
            }
            try
            {
                var apikey = await _userServiceHelper.GenerateApiKeyAsync(); // GET API KEY 
                if (apikey != null)
                {                   
                    var payrollMonthData = new PeriodRequest {
                        PeriodType = PeriodType,
                        Company_Id= CompanyId,
                        PeriodFrom_Date = Convert.ToDateTime(PeriodFDate),
                        PeriodTo_Date = Convert.ToDateTime(PeriodEDate),
                        CreatedBy = SessionUserId
                    };                    
                    var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostPayrollMonthDetailUrl;                   
                    var apiResponse = await _transactionServiceHelper
                                        .PostSingleCommonWithKeyAsync(apiUrl, payrollMonthData, apikey);
                    if (apiResponse.IsSuccess)
                    {
                        return Json(new { success = true, message = apiResponse.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = apiResponse.Message, type=apiResponse.MessageType });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Generate Key Failed " });
                }               
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        #endregion
        #region Bind Payroll Group 
        [HttpGet]
        public async Task<IActionResult> GetPayrollMonthGroupListJson(int companyid)
        {
            var apikey = await _userServiceHelper.GenerateApiKeyAsync(); // GET API KEY 
            if (apikey != null)
            {
                var payrollgroup = await GetPayrollMonthGroupListAsync(companyid, apikey);

                var filtered = payrollgroup.Select(x => new
                {
                    x.FYearDate
                });

                return Json(filtered);
            }
            return Json(new { success = false, message = "Generate Key Failed " });
        }
        private async Task<List<PeriodRequest>> GetPayrollMonthGroupListAsync(int id, string apiKey)
        {
            string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetPayrollMonthGroupUrl}?id={id}";
            var companyProfiles = new List<PeriodRequest>();

            try
            {
                var apiResponse = await _transactionServiceHelper.GetListWithKeyAsync<PeriodRequest>(apiUrl, apiKey);
                if (apiResponse != null && apiResponse.Any())
                {
                    companyProfiles = apiResponse;
                }
            }
            catch (Exception ex)
            {
                // Log error or handle accordingly
                Console.WriteLine("Error fetching companies: " + ex.Message);
            }

            return companyProfiles;
        }
        #endregion
        #region LIST  Payroll Month based on Company ID and Pay Group
        [HttpGet]
        public async Task<IActionResult> GetPayrollMonthListJson(int companyid,string payrollGroup)
        {
            var apikey = await _userServiceHelper.GenerateApiKeyAsync(); // GET API KEY 
            if (apikey != null)
            {
                string encodedGroup = Uri.EscapeDataString(payrollGroup);
                if (companyid <= 0 && string.IsNullOrEmpty(payrollGroup))
                {
                    return Json(new { success = false, message = "Please provide all required fields." });
                }

                var payrollgroup = await GetPayrollMonthListAsync(companyid, encodedGroup, apikey);

                var filtered = payrollgroup.Select(x => new
                {
                    x.Period_Code,
                    x.Period_Name,
                    x.CustomGroupName,
                    x.PeriodFrom_Date,
                    x.PeriodTo_Date,
                    x.Days
                });

                return Json(filtered);
            }
            return Json(new { success = false, message = "Generate Key Failed " });
        }
        private async Task<List<PeriodRequest>> GetPayrollMonthListAsync(int id,string encodedGroup, string apiKey)
        {
            string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetPayrollMonthGroupUrl}?id={id}&fGroupDate={encodedGroup}";

            var companyProfiles = new List<PeriodRequest>();

            try
            {
                var apiResponse = await _transactionServiceHelper.GetListWithKeyAsync<PeriodRequest>(apiUrl, apiKey);
                if (apiResponse != null && apiResponse.Any())
                {
                    foreach (var period in apiResponse)
                    {
                        period.CustomGroupName = period.CustomGroup_Id == 0 ? "Default" : "Custom";
                    }

                    companyProfiles = apiResponse;
                }
            }
            catch (Exception ex)
            {
                // Log error or handle accordingly
                Console.WriteLine("Error fetching companies: " + ex.Message);
            }

            return companyProfiles;
        }
        #endregion
        #region View Generated Payroll Month
        [HttpGet]
		public async Task<IActionResult> PayrollPeriodList()
		{
            //await SetUserPermissions();
            return View();
		}
        #endregion
        #endregion
    }
}
