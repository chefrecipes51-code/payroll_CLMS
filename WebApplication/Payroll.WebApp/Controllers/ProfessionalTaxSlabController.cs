using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;

namespace Payroll.WebApp.Controllers
{
    public class ProfessionalTaxSlabController : SharedUtilityController
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
        public ProfessionalTaxSlabController(IConfiguration config, RestApiTransactionServiceHelper transactionServiceHelper, RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _userServiceHelper = userServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _masterServiceHelper = masterServiceHelper;
            _configuration = config;
        }       
        #endregion

        #region PTaxSlab List     
        public async Task<IActionResult> Index()
        {
            await SetUserPermissions();
            var (isSuccess, ptaxslablist, errorMessage) = await FetchAllPTaxSlabAsync(); 
            if (isSuccess)
            {
                ViewBag.PayTaxSlabCount = ptaxslablist?.Count() ?? 0;
                return View(ptaxslablist);
            }
            else
            {
                return View(new List<ProfessionalTaxSlabViewModel>());
            }
        }    
        public async Task<IActionResult> GetTaxParamsByState(int stateId)
        {
            try
            {
                int taxType = 2;
                var (isSuccess, result, errorMessage) = await FetchAllTaxParamAsync(taxType, stateId);
                if (isSuccess)
                {
                    return Ok(new ApiResponseModel<List<TaxParamDTO>>
                    {
                        IsSuccess = true,
                        Result = result
                    });
                }
                else
                {
                    return Ok(new ApiResponseModel<List<TaxParamDTO>>
                    {
                        IsSuccess = false,
                        Message = errorMessage
                    });
                }
            }
            catch (Exception ex)
            {
                // Return a generic error message in case of exception
                return StatusCode(500, new ApiResponseModel<List<TaxParamDTO>>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    StatusCode = ApiResponseStatusConstant.InternalServerError
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUpdatedPTaxSlabTableRows()
        {
            var (isSuccess, ptaxslablist, errorMessage) = await FetchAllPTaxSlabAsync();
            ViewBag.PayTaxSlabCount = ptaxslablist?.Count() ?? 0;
            if (isSuccess)
                return PartialView("_PTaxSlabTableRows", ptaxslablist);

            return PartialView("_PTaxSlabTableRows", new List<ProfessionalTaxSlabViewModel>());
        } 
        #endregion

        #region PTaxSlab Insert
        [HttpPost]
        public async Task<IActionResult> InsertPtaxSlab([FromBody] PtaxSlabDTO ptaxSlabDTO)
        {

            if (ptaxSlabDTO == null)
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
                ptaxSlabDTO.Company_Id = SessionCompanyId;
                ptaxSlabDTO.CreatedBy = SessionUserId;
                ptaxSlabDTO.CreatedDate = DateTime.Now;

                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (apiKey == null)
                {
                    return Json(new { success = false, message = "Generate Key Failed" });
                }

                if (ptaxSlabDTO.Ptax_Slab_Id > 0)
                {
                    return await UpdatePtaxSlabAsync(ptaxSlabDTO, apiKey);
                }
                else
                {
                    ptaxSlabDTO.IsActive = true;
                    ptaxSlabDTO.Is_YearEnd_Adjustment = false;
                    return await CreatePtaxSlabAsync(ptaxSlabDTO, apiKey);
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        #endregion

        #region PTaxSlab Delete
        public async Task<IActionResult> DeletePTaxSlab([FromBody] PtaxSlabDTO model)
        {

            var apikey = await _userServiceHelper.GenerateApiKeyAsync();
            model.UpdatedBy = SessionUserId;
            // Construct API URL
            var deleteApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.DeletePTaxSlabUrl}/{model.Ptax_Slab_Id}";

            // Call the common delete method (sending the request body)
            var deleteResponse = await _transactionServiceHelper.DeleteCommonAsync<PtaxSlabDTO, PtaxSlabRequest>(deleteApiUrl, model, apikey);

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

        #region PTaxSlab Based on ID
        [HttpGet]
        public async Task<IActionResult> GetPTaxSlabDetail(int id)
        {
            var result = await FetchPTaxSlabByIdAsync(id);
            if (result.IsSuccess && result.Data != null)
            {
                return Json(new { isSuccess = true, data = result.Data });
            }
            else
            {
                return Json(new { isSuccess = false, message = result.ErrorMessage });
            }
        }


        #endregion
        #region Private Method CRUD  
        private async Task<IActionResult> CreatePtaxSlabAsync(PtaxSlabDTO formula, string apiKey)
        {
            var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostPTaxSlabUrl;
            var apiResponse = await _transactionServiceHelper.PostSingleCommonWithKeyAsync(apiUrl, formula, apiKey);

            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message});
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task<IActionResult> UpdatePtaxSlabAsync(PtaxSlabDTO formula, string apiKey)
        {
            formula.UpdatedBy = SessionUserId;
            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.PutPTaxSlabUrl}/{formula.Ptax_Slab_Id}";

            var apiResponse = await _transactionServiceHelper.PutSingleCommonWithKeyAsync(apiUrl, formula, apiKey);

            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }
        private async Task<(bool isSuccess, List<ProfessionalTaxSlabViewModel> result, string? errorMessage)> FetchAllPTaxSlabAsync()
        {
            try
            {
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                if (string.IsNullOrWhiteSpace(apiKey))
                    return (false, new List<ProfessionalTaxSlabViewModel>(), "API Key generation failed.");

                string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPTaxSlabUrl}?company_Id={SessionCompanyId}";

                var apiResponse = await _transactionServiceHelper.GetListWithKeyAsync<ProfessionalTaxSlabViewModel>(apiUrl, apiKey);

                if (apiResponse != null && apiResponse.Any())
                {
                    return (true, apiResponse, null); // Return the formula list
                }

                return (false, new List<ProfessionalTaxSlabViewModel>(), "No data returned from API.");
            }
            catch (Exception ex)
            {
                return (false, new List<ProfessionalTaxSlabViewModel>(), $"Error fetching Tax Slab: {ex.Message}");
            }
        }
        private async Task<(bool isSuccess, List<TaxParamDTO> result, string? errorMessage)> FetchAllTaxParamAsync(int Tax_Type, int State_Id)
        {
            try
            {
                // Get the API Key
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                if (apiKey != null)
                {
                    // Build the API URL with the provided parameters
                    string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetTaxParamUrl}?id={Tax_Type}&sid={State_Id}";

                    // Fetch data from the API
                    var apiResponse = await _transactionServiceHelper.GetListWithKeyAsync<TaxParamRequest>(apiUrl, apiKey);

                    // Check if the response is valid or empty
                    if (apiResponse == null || !apiResponse.Any())
                    {
                        return (false, new List<TaxParamDTO>(), "No data returned from API.");
                    }

                    // Map the API response to DTO
                    var apiResponseDTO = _mapper.Map<List<TaxParamDTO>>(apiResponse);

                    return (true, apiResponseDTO, null);
                }

                // Return error if the API key is null
                return (false, new List<TaxParamDTO>(), "API Key generation failed.");
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return the error message
                return (false, new List<TaxParamDTO>(), $"Error fetching tax param: {ex.Message}");
            }
        }
        private async Task<(bool IsSuccess, PtaxSlabDTO? Data, string? ErrorMessage)> FetchPTaxSlabByIdAsync(int pTaxSlabId)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            if (apiKey != null)
            {
                string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetByIdPTaxSlabUrl}/{pTaxSlabId}";
                var apiResponse = await _masterServiceHelper.GetByIdCommonWithKeyAsync<PtaxSlabDTO>(apiUrl, pTaxSlabId, apiKey);

                if (apiResponse.IsSuccess && apiResponse.Result != null)
                {
                    return (true, apiResponse.Result, null);
                }
                else
                {
                    return (false, null, apiResponse.Message ?? "Failed to load slab details.");
                }
            }
            else
            {
                return (false, null, "API Key generation failed.");
            }
        }
        #endregion
    }
}
