using AutoMapper;
using DataMigrationService.BAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.Helpers;
using Payroll.Common.Repository.Interface;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Extensions;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;
using RabbitMQ.Client;
using System.ComponentModel.Design;
using System.Net;
using System.Net.Http;
using UserService.BAL.Requests;

namespace Payroll.WebApp.Controllers
{
    /// <summary>
    /// Developer Name:- Harshida Parmar
    /// </summary>
    /// 
    [ServiceFilter(typeof(MenuAuthorizationFilter))]
    public class CompanyController : SharedUtilityController
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        //private readonly BindDropdownDataHelper _dropdownHelper;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
        //public CompanyController(RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings, BindDropdownDataHelper dropdownHelper)
        public CompanyController(RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings)
        {
            this._mapper = mapper;
            this._apiSettings = apiSettings.Value;
            //_dropdownHelper = dropdownHelper;
            _userServiceHelper = userServiceHelper;
            _masterServiceHelper = masterServiceHelper;
        }
        #region Add Company Details [Added By Priyanshi]

        //Payroll-494 Mechanism to grant page level rights
        //SetUserPermissions method created by chirag gurjar 4-mar-25
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }
        //Not in Use as per requirment 13-03-2025 by Abhishek

        //public async Task<IActionResult> AddCompanyDetails()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddCompanyDetails([FromBody] CompanyMasterDTO companyMasterDTO)
        //{
        //    try
        //    {
        //        // Set CreatedBy field
        //        var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
        //        int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
        //        companyMasterDTO.CreatedBy = userId;

        //        // Map CompanyMasterDTO to CompanyMaster using AutoMapper
        //        var companyRequest = _mapper.Map<CompanyMaster>(companyMasterDTO);

        //        // Define API URL
        //        var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostCompanyMasterDetailsUrl;

        //        // Call the generic PostAsync method to post company data
        //        var apiResponse = await _userServiceHelper
        //                            .PostCommonAsync<CompanyMaster, CompanyMasterDTO>(apiUrl, companyRequest);

        //        // Handle response
        //        if (apiResponse.IsSuccess)
        //        {
        //            return Json(new { success = true, message = "Company details added successfully!" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = apiResponse.Message });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception if necessary
        //        return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
        //    }
        //}
        #endregion

        #region Added By Krunali 
        #region Get Company Subsidiary All 
        [HttpGet]
        public async Task<IActionResult> GetCompanySubsidiary()
        {
            try
            {
                var response = new ApiResponseModel<SubsidiaryMasterDTO> { IsSuccess = false };
                string apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetSubsidiaryMasterUrl;

                // Call the dynamic GetAsync method to fetch the data
                var apiResponse = await _masterServiceHelper.GetSubsidiaryMasterAllAsync(apiUrl);
                int result = apiResponse.Count;

                if (result != 0)
                {
                    IEnumerable<SubsidiaryMasterDTO> subsidiaryMaster = _mapper.Map<IEnumerable<SubsidiaryMasterDTO>>(apiResponse);
                    return PartialView("_CompanySubsidiary", subsidiaryMaster);
                }
                else
                {
                    return View(new List<SubsidiaryMasterDTO>());
                }
            }
            catch (Exception ex)
            {
                var logErrorMessage = ex.Message;

                return new JsonResult(new ApiResponseModel<SubsidiaryMasterDTO>
                {
                    IsSuccess = false,
                    Message = MessageConstants.TechnicalIssue,
                    StatusCode = ApiResponseStatusConstant.InternalServerError
                });
            }
        }
        #endregion
        #endregion

        #region [Added By Harshida]     

        /// <summary>
        /// Developer Name :- Harshida Parmar
        ///  Created Date   :- 24-Jan-2025
        ///  Note:- Instead of Demographic NOW as per sir innpur we will use the word Company Master
        /// </summary>
        /// <returns></returns>
        #region Company Master Update
        [HttpPost]
        public async Task<IActionResult> UpdateCompanyDemographicDetails([FromBody] CompanyDemographicDetails model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received");
            }

            var companyMaster = MapToCompanyMaster(model);

            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
            companyMaster.UpdatedBy = userId;

            // Define API URL            
            var apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.PutCompanyMasterDetailsUrl}/{model.Company_Id}";

            var apiResponse = await _masterServiceHelper.PutCommonAsync<CompanyMaster, ApiResponseModel<CompanyMaster>>(apiUrl, companyMaster);

            if (apiResponse.IsSuccess)
            {
                return Ok(new { success = true, message = apiResponse.Message, data = apiResponse.Data });
            }
            else
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { success = false, message = "Failed to update company details.", errors = "apiResponse.Errors" });
            }
        }
        /// <summary>
        /// Created By:- Harshida Parmar
        /// Note:- Map the Model Value 
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public static CompanyMaster MapToCompanyMaster(CompanyDemographicDetails details)
        {
            return new CompanyMaster
            {
                Company_Id = details.Company_Id,
                CompanyType_ID = details.CompanyType_ID,
                Company_Code = details.Company_Code,
                CompanyName = details.CompanyName,
                CompanyPrintName = details.CompanyPrintName,
                IsParent = details.IsParent,
                CompanyShortName = details.CompanyShortName,
                ParentCompany_Id = details.ParentCompany_Id,
                //ParentCompany_Id = details.IsParent ? details.ParentCompany_Id : (byte)0,
                Has_Subsidary = details.Has_Subsidary,

                // Set default values for missing properties
                Group_Id = 0,          // Default value; adjust based on logic
                CompanyLevel = 0,      // Default value; adjust as needed
                Location_ID = 0        // Default value; modify accordingly
            };
        }
        /// <summary>
        ///   Note:- Instead of Demographic NOW as per sir innpur we will use the word Company Basic Details
        /// </summary>
        /// <param name="companyDemographicId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCompanyDemographicDetailsPartialByCompanyId(int companyDemographicId)
        {
            //int companyId = 5;
            //var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            var sessionData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
            int companyId = sessionData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
            string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetCompanyDemographicDetailsUrl}/{companyDemographicId}";

            // var apiResponse = await RestApiMasterServiceHelper.Instance.GetAllRecordsAsync<PayrollMasterService.BAL.Models.CompanyCorrespondance>(apiUrl);
            var apiResponse = await _masterServiceHelper.GetAllApiResponseByIdAsync<PayrollMasterService.BAL.Models.CompanyProfile>(apiUrl);
            //if (apiResponse.Message == "Record retrieved successfully." && apiResponse.Result != null)
            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                PayrollMasterService.BAL.Models.CompanyProfile companyDemographic =
                            _mapper.Map<PayrollMasterService.BAL.Models.CompanyProfile>(apiResponse.Result);
                return PartialView("_CompanyMaster", companyDemographic); // Ensure this partial view is correct
            }
            return PartialView("_CompanyMaster", new PayrollMasterService.BAL.Models.CompanyProfile());
        }
		[HttpGet]
		public async Task<IActionResult> GetCompanyForResetRecord(int companyDemographicId)
		{
            //var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            var sessionData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
            int companyId = sessionData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
			string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetCompanyDemographicDetailsUrl}/{companyDemographicId}";

			// var apiResponse = await RestApiMasterServiceHelper.Instance.GetAllRecordsAsync<PayrollMasterService.BAL.Models.CompanyCorrespondance>(apiUrl);
			var apiResponse = await _masterServiceHelper.GetAllApiResponseByIdAsync<PayrollMasterService.BAL.Models.CompanyProfile>(apiUrl);

			if (apiResponse.IsSuccess)
			{
				return Ok(new { success = true, message = apiResponse.Message, data = apiResponse.Result });
			}
			return Json(new { success = false, message = apiResponse.Message });
		}
		#endregion
		#region CompanyCorrespondance CRUD 
		//public async Task<IActionResult> Index(int? companyId)
        public async Task<IActionResult> Index(string companyId)
        {
            await SetUserPermissions(); //Added By Chirag

            int? decryptedCompanyId = null;
            if (!string.IsNullOrEmpty(companyId))
            {
                try
                {
                    string decryptedIdStr = SingleEncryptionHelper.Decrypt(companyId);

                    if (int.TryParse(decryptedIdStr, out int parsedCompanyId))
                    {
                        decryptedCompanyId = parsedCompanyId;
                    }
                    else
                    {
                        //return BadRequest("Invalid decrypted company ID format");
                    }
                }
                catch (Exception ex)
                {                 
                   // return BadRequest("Invalid encrypted company ID");
                }
            }


            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            // Extract companyId, roleId, and userId
            //int companyId = sessionData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0; // Getting from URL Not from Session
            //int companyId = 32;
            string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetCompanyDetailsByIdUrl}/{decryptedCompanyId}";
            //var apiResponse = await RestApiMasterServiceHelper.Instance.GetAllRecordsByCompanyIdAsync(apiUrl);
            var apiResponse = await _masterServiceHelper.GetAllRecordsByCompanyIdAsync(apiUrl);

            if (apiResponse == null || apiResponse.Result == null)
            {
                return View(new CompanyProfile());
            }
            return View(apiResponse.Result);
        }
        [HttpGet]
        public async Task<IActionResult> GetCorrespondanceDetails([FromQuery] int correspondanceId)
        {
            int companyCorrespondanceId = correspondanceId;
            string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetCompanyCorrespondanceByIdUrl}/{companyCorrespondanceId}";
            //var apiResponse = await RestApiMasterServiceHelper.Instance.GetCompanyCorrespondanceByIdAsync(apiUrl);
            var apiResponse = await _masterServiceHelper.GetCompanyCorrespondanceByIdAsync(apiUrl);
            if (!apiResponse.IsSuccess || apiResponse.Result == null)
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
            return Json(new { success = true, data = apiResponse.Result });
        }
        [HttpPost]
        public async Task<IActionResult> AddCompanyCorrespondance(CompanyCorrespondanceDTO companycorrespondanceDTO)
        {
            if (companycorrespondanceDTO == null)
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
                companycorrespondanceDTO.CreatedBy = userId;
                companycorrespondanceDTO.CreatedDate = DateTime.Now;
                companycorrespondanceDTO.IsActive = true;

                var areaRequest = _mapper.Map<PayrollMasterService.BAL.Models.CompanyCorrespondance>(companycorrespondanceDTO);
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostCompanyCorrespondanceMasterUrl;
                var apiResponse = await _masterServiceHelper
                                    .PostCommonAsync<PayrollMasterService.BAL.Models.CompanyCorrespondance, CompanyCorrespondanceDTO>(apiUrl, areaRequest);
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
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCompanyCorrespondance(CompanyCorrespondanceDTO model)
        {
            //var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");

            // Set CreatedBy field
            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
            model.UpdatedBy = userId;

            // Map CompanyCorrespondanceDTO to CompanyCorrespondance using AutoMapper
            var companyRequest = _mapper.Map<PayrollMasterService.BAL.Models.CompanyCorrespondance>(model);

            // Define API URL  
            var apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.PutCompanyCorrespondanceMasterUrl}/{model.Correspondance_ID}";
            // Call the generic PostAsync method to post company data
            var apiResponse = await _masterServiceHelper
                                .PutCommonAsync<PayrollMasterService.BAL.Models.CompanyCorrespondance, CompanyCorrespondanceDTO>(apiUrl, companyRequest);

            // Handle response
            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
            //return Json(new { success = true });
        }
        [HttpGet]
        public async Task<IActionResult> GetCorrespondancePartialByCompanyId(int companytCorrespondanceId)
        {
            //var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            var sessionData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
            int companyId = sessionData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;

            string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetCorrespondanceByCompanyIdUrl}/{companytCorrespondanceId}";

            // var apiResponse = await RestApiMasterServiceHelper.Instance.GetAllRecordsAsync<PayrollMasterService.BAL.Models.CompanyCorrespondance>(apiUrl);
            var apiResponse = await _masterServiceHelper.GetAllRecordsAsync<PayrollMasterService.BAL.Models.CompanyCorrespondance>(apiUrl);
            if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
            {
                IEnumerable<PayrollMasterService.BAL.Models.CompanyCorrespondance> departmentMappings = _mapper.Map<IEnumerable<PayrollMasterService.BAL.Models.CompanyCorrespondance>>(apiResponse.Result);
                return PartialView("_CompanyCorrespondances", departmentMappings); // Ensure this partial view is correct
            }
            return PartialView("_CompanyCorrespondances", new List<PayrollMasterService.BAL.Models.CompanyCorrespondance>());
        }
        #endregion
        #region Company LIST
        public async Task<IActionResult> CompanyList()        
        {
            await SetUserPermissions();//Added By Chirag

            ////added by chirag temperary to security check.
            //#region Get Permission
            //var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            //string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            //var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));          
            //ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
            //#endregion

            var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllCompanyListUrl;
            List<CompanyProfile> companyProfiles = new List<CompanyProfile>();

            try
            {
                var apiResponse = await _masterServiceHelper.GetListAsync<CompanyProfile>(apiUrl);

                if (apiResponse != null && apiResponse.Any())
                {
                    companyProfiles = apiResponse;
                }
                else
                {
                    //TempData["ErrorMessage"] = "No company data found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error fetching company list: {ex.Message}";
            }

            return View(companyProfiles);
        }
        #endregion
        #region Company Configuration CRUD
        [HttpGet]
        public async Task<IActionResult> GetCompanyCurrencyId(int companyId)
        {
            string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetCompanyDemographicDetailsUrl}/{companyId}";

            // var apiResponse = await RestApiMasterServiceHelper.Instance.GetAllRecordsAsync<PayrollMasterService.BAL.Models.CompanyCorrespondance>(apiUrl);
            var apiResponse = await _masterServiceHelper.GetAllApiResponseByIdAsync<PayrollMasterService.BAL.Models.CompanyProfile>(apiUrl);
            //if (apiResponse.Message == "Record retrieved successfully." && apiResponse.Result != null)
            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                PayrollMasterService.BAL.Models.CompanyProfile companyBasicDetails =
                            _mapper.Map<PayrollMasterService.BAL.Models.CompanyProfile>(apiResponse.Result);
                return Ok(new
                {
                    success = true,
                    Currency_ID = companyBasicDetails.Currency_ID
                });
            }
            else
            {
                return NotFound(new
                {
                    success = false,
                    Currency_ID = 0
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCompanyConfiguration([FromBody] CompanyConfigurationDTO model)
        {
            if (model == null)
            {
                return BadRequest(new { success = false, message = "Invalid data received" });
            }
            try
            {
                if (model.StartDate == DateTime.MinValue || model.EndDate == DateTime.MinValue)
                {
                    return BadRequest(new { success = false, message = "Invalid dates provided" });
                }

                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                model.CreatedBy = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;                
                var companyRequest = _mapper.Map<CompanyConfigurationRequest>(model);
                var apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.PostCompanyConfigurationUrl}";
                var apiResponse = await _masterServiceHelper
                                    .PostCompanyFinancialDetailsAsync<CompanyConfigurationRequest, CompanyConfigurationDTO>(apiUrl, companyRequest);
                if (apiResponse.IsSuccess)
                {
                    return Json(new
                    {
                        success = true,
                        message = apiResponse.Message,
                        startDate = model.StartDate.ToString("yyyy-MM-dd"),
                        endDate = model.EndDate.ToString("yyyy-MM-dd")
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = apiResponse.Message,
                        startDate = model.StartDate.ToString("yyyy-MM-dd"),
                        endDate = model.EndDate.ToString("yyyy-MM-dd")
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
        #endregion
        #region Company Encrypt 
        //[HttpGet]
        //public IActionResult EncryptId(string companyId)
        //{
        //    if (string.IsNullOrEmpty(companyId))
        //        return BadRequest("Invalid company ID");

        //    string encryptedId = SingleEncryptionHelper.Encrypt(companyId);
        //    return Ok(encryptedId);
        //}
        #endregion
        #endregion
    }

}