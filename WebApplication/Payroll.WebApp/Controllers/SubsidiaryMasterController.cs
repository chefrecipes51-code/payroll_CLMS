using System.Drawing;
using System.Net;
using System.Net.Http;
using AutoMapper;
using DataMigrationService.BAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.Repository.Interface;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Extensions;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using UserService.BAL.Requests;
using static Payroll.Common.EnumUtility.EnumUtility;

namespace Payroll.WebApp.Controllers
{
    public class SubsidiaryMasterController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly IConfiguration _configuration;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;

        public SubsidiaryMasterController(RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper,IMapper mapper, IOptions<ApiSettings> apiSettings, IConfiguration configuration)
        {
            this._mapper = mapper;
            this._apiSettings = apiSettings.Value;
            this._configuration = configuration;
            _userServiceHelper = userServiceHelper;
            _masterServiceHelper = masterServiceHelper;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetCompanyLocationData(int? companyId)
        //{
        //    try
        //    {
        //        var data = await _masterServiceHelper.BindCompanyLocationDataAsync(companyId);

        //        if (data?.Result == null)
        //        {
        //            return Json(new { isSuccess = false, message = "No data found" });
        //        }

        //        var companyLocationData = new CompanyLocationMapDto
        //        {
        //            Countries = data.Result.Countries,
        //            States = data.Result.States,
        //            Cities = data.Result.Cities,
        //            Locations = data.Result.Locations,
        //            Roles = data.Result.Roles
        //        };

        //        return Json(new { isSuccess = true, result = companyLocationData });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}

        #region AddSubsidiaryMaster

        public async Task<IActionResult> AddSubsidiaryMaster()
        {
            return View();
        }

        [HttpPost]
       
        public async Task<IActionResult> AddSubsidiaryMaster([FromBody] SubsidiaryMasterDTO subsidiaryMasterDTO)
        {
            try
            {
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
                subsidiaryMasterDTO.CreatedBy = userId;
                subsidiaryMasterDTO.IsActive = true;
                var subsidiaryMasterRequest = _mapper.Map<SubsidiaryMaster>(subsidiaryMasterDTO);
                // Call PostUserAsync method to post user data
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostSubsidiaryMasterUrl;
                var apiResponse = await _masterServiceHelper.PostSubSidiaryMasterAsync(apiUrl, subsidiaryMasterRequest);
                if (apiResponse.IsSuccess)
                {
                    var subsidiaryMasterId = (apiResponse.Result as SubsidiaryMasterDTO)?.Subsidiary_Id;
                    return Json(new
                    {
                        success = true,
                        message = apiResponse.Message,
                        subsidiaryId = subsidiaryMasterId
                    });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }    
            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        #endregion

        #region GetSubsidiaryAll 
        public async Task<IActionResult> Index()
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
                    return View(subsidiaryMaster);
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

        #region GetSubsidiaryByID
        [HttpGet]
        public async Task<IActionResult> GetSubsidiaryById(int subsidiaryId)
        {
            try
            {
                string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetSubsidiaryMasterByIdUrl}/{subsidiaryId}";
                var apiResponse = await _masterServiceHelper.GetSubsidiaryMasterByIdAsync(apiUrl);

                if (apiResponse.IsSuccess == true && apiResponse.Result != null)
                {
                    var subsidiaryType = _mapper.Map<SubsidiaryMasterDTO>(apiResponse.Result);
                    return Json(new { IsSuccess = true, Data = subsidiaryType });
                }
                return Json(new { IsSuccess = false, Message = "Subsidiary not found" });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching subsidiary data.",
                    Error = ex.Message
                });
            }
        }
        #endregion

        #region UpdateSubsidiary
        [HttpPost]

        public async Task<IActionResult> UpdateSubsidiaryMaster([FromBody] SubsidiaryMasterDTO subsidiaryMasterDto)
        {
            try
            {
                // Map UserDTO to UserRequest using AutoMapper
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;

                subsidiaryMasterDto.UpdatedBy = userId;
                subsidiaryMasterDto.UpdatedDate = DateTime.Now;
                var subsidiaryMasterRequest = _mapper.Map<SubsidiaryMaster>(subsidiaryMasterDto);
                // Call PostUserAsync method to post user data
                int subsidiaryId = subsidiaryMasterDto.Subsidiary_Id;
                var apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.PutSubsidiaryMasterUrl}/{subsidiaryId}";
                var apiResponse = await _masterServiceHelper.PutSubsidiaryMasterAsync(apiUrl, subsidiaryMasterRequest);
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
                // Log exception if necessary
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
            
        }

        #endregion

        #region DeleteSubsidiary

        public async Task<IActionResult> DeleteSubsidiaryMaster()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> DeleteSubsidiaryMaster([FromBody] SubsidiaryMasterDTO subsidiaryMaster)
        {
            // Set CreatedBy field
            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
            subsidiaryMaster.UpdatedBy = userId;

            string apiUrl = _apiSettings.PayrollMasterServiceEndpoints.DeleteSubsidiaryMasterUrl;
            string queryParams = $"/{subsidiaryMaster.Subsidiary_Id}";
            var objsubsidiaryMaster = _mapper.Map<SubsidiaryMaster>(subsidiaryMaster);
			var response = await _masterServiceHelper.DeleteSubsidiaryMasterAsync($"{apiUrl}{queryParams}", objsubsidiaryMaster);
			return Json(new
			{
				success = response.IsSuccess,
				message = response.Message
			});
		}

        #endregion
    }
}
