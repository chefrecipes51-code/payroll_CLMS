/// <summary>
/// Developer Name:- Harshida Parmar
/// Note:- Perform CRUD operation for formula master. 
/// Jira Ticket:- 753,754,755,756,781 [ALL are in Sprint 15]
/// </summary>
using AutoMapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.Helpers;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;
using System.Linq.Expressions;
using System.Net;
using UserService.BAL.Requests;
using NCalc;
using Payroll.WebApp.Extensions;

namespace Payroll.WebApp.Controllers
{
    [ServiceFilter(typeof(MenuAuthorizationFilter))]
    public class FormulaMasterController : SharedUtilityController
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
        public FormulaMasterController(IConfiguration config, RestApiTransactionServiceHelper transactionServiceHelper, RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _userServiceHelper = userServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
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
        #region Formula List       
        public async Task<IActionResult> Index()
        {
            await SetUserPermissions();
            var (isSuccess, formulaList, errorMessage) = await FetchAllFormulasAsync(); // Use the common method

            if (isSuccess)
            {
                return View(formulaList);
            }
            else
            {               
                return View(new List<FormulaMaster>());
            }
        }

        #endregion
        #region Formula Add/Update 
        public async Task<IActionResult> AddUpdateFormula(string? formulaId)
        {
            await SetUserPermissions();
            FormulaMaster model = new();

            var (isSuccess, result, error) = await FetchFormulaByEncryptedIdAsync(formulaId);

            if (isSuccess && result != null)
            {
                model = result;
            }
            else if (!string.IsNullOrEmpty(error))
            {
                //TempData["Error"] = error;
            }
            return View(model);
        }

       // [HttpGet("getformulabyid/{formulaId}")]
        public async Task<IActionResult> GetFormulaById(int formulaId)
        {
            if (formulaId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid formula ID." });
            }

            try
            {
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return StatusCode(500, new { success = false, message = "API Key generation failed." });
                }

                string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetWageFormulaByIdUrl}/{formulaId}";
                var apiResponse = await _masterServiceHelper.GetByIdCommonWithKeyAsync<FormulaMaster>(apiUrl, formulaId, apiKey);

                if (apiResponse.IsSuccess && apiResponse.Result != null)
                {
                    return Ok(new { success = true, data = apiResponse.Result });
                }
                else
                {
                    return NotFound(new { success = false, message = apiResponse.Message ?? "Formula not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching the formula.", error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddFormula([FromBody] FormulaMaster formula)
        {

            if (formula == null)
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
                formula.Cmp_Id = SessionCompanyId;
                formula.CreatedBy = SessionUserId;
                formula.CreatedDate = DateTime.Now;
                
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }

                if (formula.Formula_Id > 0)
                {
                    return await UpdateFormulaAsync(formula, apiKey);
                }
                else
                {
                    formula.IsActive = true;
                    return await CreateFormulaAsync(formula, apiKey);
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetFormulaData(string formulaId)
        {
            var (isSuccess, result, error) = await FetchFormulaByEncryptedIdAsync(formulaId);

            if (isSuccess && result != null)
            {
                return Json(result);
            }
            else
            {
                return Json(new { success = false, message = error });
            }
        }
        #endregion
        #region Get All Pay Component List
        [HttpGet]
        public async Task<IActionResult> GetPayComponentList()
        {
            var apikey = await _userServiceHelper.GenerateApiKeyAsync();
            if (apikey != null)
            {
                var payrollgroup = await GetPayComponentList(apikey);

                var filtered = payrollgroup.Select(x => new
                {
                    x.EarningDeduction_Id,
                    x.EarningDeductionType,
                    x.EarningDeductionName,
                    x.IsActive
                });

                //return Json(filtered);
                return Json(new { success = true, data = filtered });
            }
            return Json(new { success = false, message = "Generate Key Failed " });

        }
        [HttpGet]
        public async Task<IActionResult> ActivateInActivePayComponentList(bool IsActive)
        {
            var apikey = await _userServiceHelper.GenerateApiKeyAsync();
            if (apikey != null)
            {
                var payrollgroup = await GetActivateDeActivePayComponentList(apikey,IsActive);
                var filtered = payrollgroup.Select(x => new
                {
                    x.EarningDeduction_Id,
                    x.EarningDeductionType,
                    x.EarningDeductionName,
                    x.IsActive
                });
                return Json(new { success = true, data = filtered });
            }
            return Json(new { success = false, message = "Generate Key Failed " });
        }
        #endregion
        #region Formula Delete
        public async Task<IActionResult> DeleteFormula([FromBody] FormulaMaster model)
        {
            model.UpdatedBy = SessionUserId;
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            if (apiKey != null)
            {
                var deleteApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.DeleteFormulaByIdUrl}/{model.Formula_Id}";

                var deleteResponse = await _masterServiceHelper.DeleteCommonWithKeyAsync<FormulaMaster, FormulaMaster>(deleteApiUrl, model, apiKey);

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
        #endregion
        #region Private Method CRUD 
        private async Task<IActionResult> CreateFormulaAsync(FormulaMaster formula, string apiKey)
        {
            var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostWageFormulaUrl;
            var apiResponse = await _masterServiceHelper.PostSingleCommonWithKeyAsync(apiUrl, formula, apiKey);
           
            if (apiResponse.IsSuccess)
            {

                return Json(new { success = true, message = apiResponse.Message, resultFormula=apiResponse.Result });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task<IActionResult> UpdateFormulaAsync(FormulaMaster formula, string apiKey)
        {
            formula.UpdatedBy = SessionUserId;
            var apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.PutWageFormulaUrl}/{formula.Formula_Id}";

            var apiResponse = await _masterServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, formula, apiKey);

            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task<(bool isSuccess, FormulaMaster? result, string? errorMessage)> FetchFormulaByEncryptedIdAsync(string? formulaId)
        {
            if (!string.IsNullOrEmpty(formulaId))
            {
                try
                {
                    string decryptedIdStr = SingleEncryptionHelper.Decrypt(formulaId);
                    if (int.TryParse(decryptedIdStr, out int parsedFormulaId))
                    {
                        int decryptedFormulaId = parsedFormulaId;

                        var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                        if (apiKey != null)
                        {
                            string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetWageFormulaByIdUrl}/{decryptedFormulaId}";
                            var apiResponse = await _masterServiceHelper.GetByIdCommonWithKeyAsync<FormulaMaster>(apiUrl, decryptedFormulaId, apiKey);

                            if (apiResponse.IsSuccess && apiResponse.Result != null)
                            {
                                return (true, apiResponse.Result, null);
                            }
                            else
                            {
                                return (false, null, apiResponse.Message ?? "Failed to load formula.");
                            }
                        }
                        else
                        {
                            return (false, null, "API Key generation failed.");
                        }
                    }
                    else
                    {
                        return (false, null, "Invalid formula ID.");
                    }
                }
                catch
                {
                    return (false, null, "Invalid formula ID.");
                }
            }

            return (false, null, "No formula ID provided.");
        }
        private async Task<(bool isSuccess, List<FormulaMaster> result, string? errorMessage)> FetchAllFormulasAsync()
        {
            try
            {
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync(); // Get API Key

                if (apiKey != null)
                {
                    string apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllFormulaUrl;

                    // Fetching the formula list from the API
                    var apiResponse = await _masterServiceHelper.GetListWithKeyAsync<FormulaMaster>(apiUrl, apiKey);

                    if (apiResponse != null && apiResponse.Any())
                    {
                        return (true, apiResponse, null); // Return the formula list
                    }
                    else
                    {
                        return (false, new List<FormulaMaster>(), "No formulas found.");
                    }
                }
                else
                {
                    return (false, new List<FormulaMaster>(), "API Key generation failed.");
                }
            }
            catch (Exception ex)
            {
                return (false, new List<FormulaMaster>(), $"Error fetching formulas: {ex.Message}");
            }
        }
        private async Task<List<PayComponentMaster>> GetActivateDeActivePayComponentList(string apiKey,bool? IsActive)
        {            
            string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetActiveInActivePayComponentsUrl}?companyId={SessionCompanyId}&IsActivate={IsActive}";
            var payComponent = new List<PayComponentMaster>();
            try
            {
                var apiResponse = await _transactionServiceHelper.GetListWithKeyAsync<PayComponentMaster>(apiUrl, apiKey);
                if (apiResponse != null && apiResponse.Any())
                {
                    payComponent = apiResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching component: " + ex.Message);
            }
            return payComponent;
        }
        private async Task<List<PayComponentMaster>> GetPayComponentList(string apiKey)
        {
            string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllActivePayComponentsUrl}?companyId={SessionCompanyId}&isActive={true}";
            var payComponent = new List<PayComponentMaster>();

            try
            {
                var apiResponse = await _transactionServiceHelper.GetListWithKeyAsync<PayComponentMaster>(apiUrl, apiKey);
                if (apiResponse != null && apiResponse.Any())
                {
                    payComponent = apiResponse;
                }
            }
            catch (Exception ex)
            {
                // Log error or handle accordingly
                Console.WriteLine("Error fetching component: " + ex.Message);
            }

            return payComponent;
        }
        private async Task<List<FormulaMaster>> GetFormulaList(string searchFormula)
        {
            var apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.FormulaSuggestionsUrl}?searchParam={searchFormula}";
            var formulaList = new List<FormulaMaster>();
            try
            {
                var apiResponse = await _masterServiceHelper.GetListAsync<FormulaMaster>(apiUrl);
                if (apiResponse != null && apiResponse.Any())
                {
                    formulaList = apiResponse;
                }
            }
            catch (Exception ex)
            {
                // Log error or handle accordingly
                Console.WriteLine("Error fetching component: " + ex.Message);
            }
            return formulaList;
        }
        #endregion
        #region Update Pay Component Status
        [HttpPost]
        public async Task<IActionResult> ActivateComponent(int componentId)
        {
            if (componentId <= 0)
            {
                return Json(new { success = false, message = "Invalid component ID." });
            }

            try
            {
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }
                else 
                {
                    PayComponentActivationDTO obj = new PayComponentActivationDTO();
                    obj.EarningDeduction_Id = componentId;
                    obj.UpdatedBy = SessionUserId;
                    var componentRequest = _mapper.Map<PayComponentActivationRequest>(obj);
                    var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.PutPayComponentActivationUrl}/{obj.EarningDeduction_Id}";
                    var apiResponse = await _transactionServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, componentRequest, apiKey);

                    if (apiResponse.IsSuccess)
                    {
                        return Json(new { success = true, message = apiResponse.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = apiResponse.Message });
                    }
                }               
            }
            catch (Exception ex)
            {
                // Log the exception here if you want
                return Json(new { success = false, message = "An error occurred while activating component." });
            }
        }

        #endregion
        #region Validate Formula         
        public IActionResult ValidateFormula(string formulaVal)
        {
            try
            {
                formulaVal = formulaVal.Replace(" ", "_");
                NCalc.Expression expression = new NCalc.Expression(formulaVal);
                var result = expression.HasErrors();
                if (result)
                {
                    return Ok(new { success = false, message = "Formula syntax is invalid according to VBODMAS." });
                }
                return Ok(new { success = true, message = "Formula is valid and respects VBODMAS." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Invalid formula: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetFormulaSuggestions(string searchParam)
        {
            try
            {
                var formulaListgroup = await GetFormulaList(searchParam);

                var filtered = formulaListgroup.Select(x => new
                {
                    x.Formula_Computation
                });
                return Json(new { success = true, data = filtered });
            }
            catch (Exception ex)
            {
                return Ok(new { success = true, message = "Formula is valid and respects VBODMAS." });
            }
        }

        #endregion
    }
}
