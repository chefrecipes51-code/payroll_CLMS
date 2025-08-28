using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ApplicationModel;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Extensions;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Requests;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;

namespace Payroll.WebApp.Controllers
{
    public class ApprovalListController : Controller
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
        private int SessionDefaultLocationId
        {
            get
            {
                var sessionData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
                return sessionData?.LocationDetails?.FirstOrDefault(x => x.Default_Location)?.Location_ID ?? 0;
            }
        }
        public ApprovalListController(IConfiguration config, RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _userServiceHelper = userServiceHelper;          
            _masterServiceHelper = masterServiceHelper;
            _configuration = config;
        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetApprovalList(DateTime? requestDate)
        {
            var apikey = await _userServiceHelper.GenerateApiKeyAsync();
            if (apikey != null)
            {
                var payrollgroup = await GetApprovalsListAsync(apikey, requestDate);
                return Json(new { success = true, data = payrollgroup });
            }
            return Json(new { success = false, message = "Generate Key Failed " });
        }
        [HttpPost]
        public async Task<IActionResult> VerifyApprovalsFromUI([FromBody] List<ApprovalDetailRequest> selectedApprovals)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            if (apiKey == null)
            {
                return BadRequest(new { message = "Unable to generate API Key." });
            }
            foreach (var approval in selectedApprovals)
            {
                approval.Approval_Status = 2; // ✅ Mark as Verified
                approval.Approver_ID = SessionUserId;

                string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.UpdateApprovalUrl}/{approval.Approval_ID}";
                var response = await _masterServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, approval, apiKey); 

                if (!response.IsSuccess)
                {
                    return BadRequest(new { message = $"Approval failed for ID {approval.Approval_ID}: {response.Message}" });
                }
            }
            return Ok(new { message = "All selected approvals verified successfully." });
        }

        #region Private Method CRUD  
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }     
        private async Task<ApprovalListViewModel> GetApprovalsListAsync(string apiKey, DateTime? requestDate)
        {
            string formattedDate = (requestDate ?? DateTime.Now).ToString("yyyy-MM-dd") + " 00:00:00.000";
            string fullUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetApprovalUrl}?approverId={SessionUserId}&correspondanceId={SessionDefaultLocationId}&requestDate={formattedDate}";
            var response = await _masterServiceHelper.GetDataAndCountWithKeyAsync<ApprovalListViewModel>(fullUrl, apiKey);
            return response; 
        }

        #endregion
    }
}
