using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;

namespace Payroll.WebApp.Controllers
{
    public class PayrollGlobalParametersController : SharedUtilityController
    {
        #region CTOR
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
        private int SessionCompanyId
        {
            get
            {
                var sessionCompanyData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
                return sessionCompanyData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
            }
        }
        public PayrollGlobalParametersController(IConfiguration config, RestApiTransactionServiceHelper transactionServiceHelper, RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _userServiceHelper = userServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _masterServiceHelper = masterServiceHelper;
            _configuration = config;
        }
        #endregion
        public IActionResult Index()
        {      
            ViewData["CompanyId"] = SessionCompanyId;         
            return View();
        }      
        #region PGlobalParameter Insert
        [HttpPost]
        public async Task<IActionResult> InsertPGlobalParameter([FromBody] PayrollGlobalParamDTO pGlobalDTO)
        {

            if (pGlobalDTO == null)
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
               // pGlobalDTO.Company_ID = SessionCompanyId;
                pGlobalDTO.CreatedBy = SessionUserId;
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }

                if (pGlobalDTO.Global_Param_ID > 0)
                {
                    pGlobalDTO.CopyFromCompanyId = 0;
                    return await UpdatePGlobalAsync(pGlobalDTO, apiKey);
                }
                else
                {
                    pGlobalDTO.CopyFromCompanyId = 0;
                    pGlobalDTO.IsActive = true;                  
                    return await CreatePGlobalAsync(pGlobalDTO, apiKey);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> FetchPayrollSettings(int companyId)

        {
            var settings = await GetPayrollSettingsAsync(companyId);
            if (settings != null)
                return Json(new { success = true, data = settings });
            else
                return Json(new { success = false, message = "No data found" });
        }
        #endregion

        #region Compliance Settings
        [HttpPost]
        public async Task<IActionResult> InsertComplianceSettings([FromBody] PayrollComplianceDTO pComplianceDTO)
        {

            if (pComplianceDTO == null)
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
                //pComplianceDTO.Company_ID = SessionCompanyId;
                pComplianceDTO.CreatedBy = SessionUserId;
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }

                if (pComplianceDTO.Prm_Comlliance_ID > 0)
                {
                    pComplianceDTO.IsActive = true;
                    pComplianceDTO.CopyFromCompanyId = 0;
                    return await UpdatePComplianceAsync(pComplianceDTO, apiKey);
                }
                else
                {                   
                    pComplianceDTO.IsActive = true;
                    pComplianceDTO.CopyFromCompanyId = 0;
                    return await CreatePComplianceAsync(pComplianceDTO, apiKey);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        #endregion

        #region Payslip Setting 
        [HttpPost]
        public async Task<IActionResult> InsertPaySlipSetting([FromBody] PayrollSettingDTO pSlipDTO)
        {

            if (pSlipDTO == null)
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
               // pSlipDTO.Company_ID = SessionCompanyId;
                pSlipDTO.CreatedBy = SessionUserId;
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }

                if (pSlipDTO.Payroll_Setin_ID > 0)
                {
                    pSlipDTO.IsActive = true;
                    pSlipDTO.CopyFromCompanyId = 0;
                    return await UpdatePSlipAsync(pSlipDTO, apiKey);
                }
                else
                {
                    pSlipDTO.IsActive = true;
                    pSlipDTO.CopyFromCompanyId = 0;
                    return await CreatePSlipAsync(pSlipDTO, apiKey);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        #endregion

        #region Third Party Data 
        [HttpPost]
        public async Task<IActionResult> InsertThirdPartyData([FromBody] ThirdPartyParameterDTO tPartyDTO)
        {

            if (tPartyDTO == null)
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
                //tPartyDTO.Company_Id = SessionCompanyId;
                tPartyDTO.CreatedBy = SessionUserId;
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }

                if (tPartyDTO.Clms_Param_ID > 0)
                {
                    tPartyDTO.IsActive = true;
                    tPartyDTO.CopyFromCompanyId = 0;
                    return await UpdateThirdPartyAsync(tPartyDTO, apiKey);
                }
                else
                {
                    tPartyDTO.IsActive = true;
                    tPartyDTO.CopyFromCompanyId = 0;
                    return await CreateThirdPartyAsync(tPartyDTO, apiKey);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        #endregion

        #region Pass CompanyID and SettingType 
        [HttpPost]
        public async Task<IActionResult> CopySettingsFromCompany([FromBody] CopySettingsDTO request)
        {           

            if (request == null || request.CopyToCompanyID == 0 || string.IsNullOrEmpty(request.SelectParam))
            {
                return Json(new { success = false, message = "Invalid input." });
            }
            try
            {
                request.CopyFromCompanyID = SessionCompanyId;
                request.CreatedBy = SessionUserId;
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }
                return await CreateCopyToFromCompanyAsync(request, apiKey);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        #endregion

        #region Private Method CRUD  

        
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }       
       
        private async Task<PayrollSettingsWrapper> GetPayrollSettingsAsync(int cId)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

            string fullUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayrollGlobalUrl}?companyId={cId}";

            var response = await _transactionServiceHelper.GetCommonKeyAsync<PayrollSettingsWrapper>(fullUrl, apiKey);

            if (response.IsSuccess && response.Result != null)
            {
                return response.Result;
            }

            return null;
        }
      
        #region GLOBAL
        private async Task<IActionResult> CreatePGlobalAsync(PayrollGlobalParamDTO globalParameter, string apiKey)
        {
            var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostPayrollGlobalParameterUrl;
            var globalParameterRequest = _mapper.Map<PayrollGlobalParamRequest>(globalParameter);
            var apiResponse = await _transactionServiceHelper.PostSingleCommonWithKeyAsync(apiUrl, globalParameterRequest, apiKey);

            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task<IActionResult> UpdatePGlobalAsync(PayrollGlobalParamDTO globalParameter, string apiKey)
        {
            globalParameter.UpdatedBy = SessionUserId;
            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.PutPayrollGlobalParameterUrl}/{globalParameter.Global_Param_ID}";
            var globalParameterRequest = _mapper.Map<PayrollGlobalParamRequest>(globalParameter);

            var apiResponse = await _transactionServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, globalParameterRequest, apiKey);

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
        #region Compliance
        private async Task<IActionResult> CreatePComplianceAsync(PayrollComplianceDTO globalParameter, string apiKey)
        {
            var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostPayrollComplianceUrl;
            //globalParameter.Esi_Based_on = globalParameter.Esic_Applicable;
            var globalParameterRequest = _mapper.Map<PayrollComplianceRequest>(globalParameter);
            var apiResponse = await _transactionServiceHelper.PostSingleCommonWithKeyAsync(apiUrl, globalParameterRequest, apiKey);

            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task<IActionResult> UpdatePComplianceAsync(PayrollComplianceDTO globalParameter, string apiKey)
        {
            globalParameter.UpdatedBy = SessionUserId;
            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.PutPayrollComplianceUrl}/{globalParameter.Prm_Comlliance_ID}";
            var globalParameterRequest = _mapper.Map<PayrollComplianceRequest>(globalParameter);

            var apiResponse = await _transactionServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, globalParameterRequest, apiKey);

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
        #region PaySlip Setting
        private async Task<IActionResult> CreatePSlipAsync(PayrollSettingDTO globalParameter, string apiKey)
        {
            var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostPayrollSlipUrl;
            //globalParameter.Esi_Based_on = globalParameter.Esic_Applicable;
            var globalParameterRequest = _mapper.Map<PayrollSettingRequest>(globalParameter);
            var apiResponse = await _transactionServiceHelper.PostSingleCommonWithKeyAsync(apiUrl, globalParameterRequest, apiKey);

            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task<IActionResult> UpdatePSlipAsync(PayrollSettingDTO globalParameter, string apiKey)
        {
            globalParameter.UpdatedBy = SessionUserId;
            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.PutPayrollSlipUrl}/{globalParameter.Payroll_Setin_ID}";
            var globalParameterRequest = _mapper.Map<PayrollSettingRequest>(globalParameter);

            var apiResponse = await _transactionServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, globalParameterRequest, apiKey);

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
        #region Third Party 
        private async Task<IActionResult> CreateThirdPartyAsync(ThirdPartyParameterDTO globalParameter, string apiKey)
        {
            var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostThirdPartyUrl;
            //globalParameter.Esi_Based_on = globalParameter.Esic_Applicable;
            var globalParameterRequest = _mapper.Map<ThirdPartyParameterRequest>(globalParameter);
            var apiResponse = await _transactionServiceHelper.PostSingleCommonWithKeyAsync(apiUrl, globalParameterRequest, apiKey);

            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task<IActionResult> UpdateThirdPartyAsync(ThirdPartyParameterDTO globalParameter, string apiKey)
        {
            globalParameter.UpdatedBy = SessionUserId;
            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.PutThirdPartyUrl}/{globalParameter.Clms_Param_ID}";
            var globalParameterRequest = _mapper.Map<ThirdPartyParameterRequest>(globalParameter);

            var apiResponse = await _transactionServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, globalParameterRequest, apiKey);

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
        #region Copy From One Company to Another Company 
        private async Task<IActionResult> CreateCopyToFromCompanyAsync(CopySettingsDTO globalParameter, string apiKey)
        {
            var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostCopyFromOneToAnotherCompanyUrl;
            var globalParameterRequest = _mapper.Map<CopySettingsRequest>(globalParameter);
            var apiResponse = await _transactionServiceHelper.PostSingleCommonWithKeyAsync(apiUrl, globalParameterRequest, apiKey);

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
        #endregion
    }
}
