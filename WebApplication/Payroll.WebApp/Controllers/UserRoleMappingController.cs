using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using System.Text;
using UserService.BAL.Models;
using UserService.BAL.Requests;

namespace Payroll.WebApp.Controllers
{
    /// <summary>
    /// Controller for managing user role mappings within the application.
    /// </summary>
    /// <remarks>
    /// This controller facilitates displaying the user role mapping view 
    /// and handling user role mapping data submission using HTTP POST requests.
    /// Created By: Harshida D. Parmar
    /// Created Date: 18-11-2024
    /// </remarks>
    public class UserRoleMappingController : Controller
    {
        #region CTOR
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        //private readonly HttpClient _httpClient;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        public UserRoleMappingController(RestApiUserServiceHelper userServiceHelper, IOptions<ApiSettings> apiSettings, IMapper mapper)//,HttpClient httpClient
        {
            _apiSettings = apiSettings.Value;
            _mapper = mapper;
            _userServiceHelper = userServiceHelper;
            // _httpClient = httpClient;
        }
        #endregion
        #region Add

        #region OLD CODE WITHOUT COMMON METHOD INSERT
        //[HttpPost]
        //public async Task<IActionResult> Index([FromBody] UserRoleMappingDTO userRoleMappingDto)
        //{
        //    var responseModel = new ApiResponseModel<UserRoleMappingDTO>
        //    {
        //        IsSuccess = false
        //    };
        //    userRoleMappingDto.CreatedBy = 1;
        //    var userRoleMappingEntity = _mapper.Map<UserRoleMapping>(userRoleMappingDto);

        //    try
        //    {
        //        var userRoleMappingApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.PostUserRoleMappingUrl;

        //        var jsonContent = JsonConvert.SerializeObject(userRoleMappingEntity);
        //        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //        var response = await _httpClient.PostAsync(userRoleMappingApiUrl, content);
        //        var apiResponse = await response.Content.ReadAsStringAsync();

        //        var result = JsonConvert.DeserializeObject<ApiResponseModel<UserRoleMappingDTO>>(apiResponse);

        //        if (result?.IsSuccess == true)
        //        {
        //            responseModel.IsSuccess = true;
        //            responseModel.Message = "User-role mapping saved successfully!";
        //            responseModel.Result = result.Result;
        //            return Json(responseModel);
        //        }
        //        else
        //        {
        //            responseModel.Message = result?.Message ?? "An error occurred while saving user-role mapping.";
        //            return Json(responseModel);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception here if needed
        //        responseModel.Message = "An unexpected error occurred while processing your request.";
        //        return Json(responseModel);
        //    }
        //}
        #endregion
        public IActionResult AddRecord()
        {
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> AddRecord([FromBody] UserRoleMappingDTO userRoleMappingDto)
        //{
        //    userRoleMappingDto.CreatedBy = 1;
        //    var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.PostUserRoleMappingUrl;
        //    var userRoleMappingEntity = _mapper.Map<UserRoleMapping>(userRoleMappingDto);
        //    var responseModel = await _userServiceHelper.PostUserRoleMappingAsync(apiUrl, userRoleMappingEntity);
        //    return Json(responseModel);
        //}

        #endregion
        #region SELECT
        #region OLD CODE WITHOUT COMMON METHOD SELECT
        //[HttpGet]
        //public async Task<IActionResult> GetUserRoleMappings(int? roleUserId = null, bool? isActive = null)
        //{
        //    var apiResponse = new ApiResponseModel<IEnumerable<UserRoleMappingRequest>>();

        //    try
        //    {
        //        var userRoleMappingApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetUserRoleMappingsUrl;

        //        var response = await _httpClient.GetAsync($"{userRoleMappingApiUrl}?roleUserId={roleUserId}&isActive={isActive}");

        //        response.EnsureSuccessStatusCode();

        //        var apiContent = await response.Content.ReadAsStringAsync();
        //        apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<UserRoleMappingRequest>>>(apiContent);

        //        if (apiResponse.IsSuccess && apiResponse.Result != null)
        //        {
        //            var userRoleMappings = _mapper.Map<IEnumerable<UserRoleMapDTO>>(apiResponse.Result);

        //            return View(userRoleMappings);
        //        }
        //        else
        //        {
        //            return View(new List<UserRoleMapDTO>());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return View(new List<UserRoleMapDTO>());
        //    }
        //}
        #endregion

        // [HttpGet]
        //public async Task<IActionResult> GetUserRoleMappings(int? roleUserId = null, bool? isActive = null)
        //{
        //    //var responseModel = new ApiResponseModel<IEnumerable<UserRoleMappingRequest>>();         
        //    string userRoleMappingApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetUserRoleMappingsUrl;
        //    string queryParams = $"?roleUserId={roleUserId}&isActive={isActive}";
        //    var apiResponse = await RestApiUserServiceHelper.Instance.GetUserRoleMappingsAsync($"{userRoleMappingApiUrl}{queryParams}");
        //    if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
        //    {
        //        IEnumerable<UserRoleMapDTO> userRoleMappings = _mapper.Map<IEnumerable<UserRoleMapDTO>>(apiResponse.Result);
        //        return View(userRoleMappings);
        //    }
        //    else
        //    {
        //        return View(new List<UserRoleMapDTO>());
        //    }           
        //}
        #endregion
        #region UPDATE THIS IS SAMPLE DATA CHECKING

        //public async Task<IActionResult> UpdateUserRoleMappingWithRoles()
        //{
        //    var responseModel = new ApiResponseModel<UserRoleMappingDTO>
        //    {
        //        IsSuccess = false
        //    };

        //    try
        //    {
        //        // Sample data for updating a user-role mapping
        //        var userRoleMappingDto = new UserRoleMappingDTO
        //        {
        //            Role_User_Id = 25, 
        //            Role_Ids = new List<int> {1}, 
        //            Role_Menu_Header_Id = 1,
        //            Company_Id = 1,    
        //            User_Id = 10,    
        //            Effective_From = DateTime.Parse("2024-11-21"),
        //            UpdatedBy = 3,
        //            UpdatedDate= DateTime.Parse("2024-11-21")
        //        };
        //        var userRoleMappingEntity = _mapper.Map<UserRoleMapping>(userRoleMappingDto);
        //        //var userRoleMappingApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.UpdateUserRoleMappingUrl;
        //        var userRoleMappingApiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.UpdateUserRoleMappingUrl}/{userRoleMappingDto.Role_User_Id}";
        //        var jsonContent = JsonConvert.SerializeObject(userRoleMappingEntity); 
        //        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //        var response = await _httpClient.PutAsync(userRoleMappingApiUrl, content);
        //        var apiResponse = await response.Content.ReadAsStringAsync();
        //        var result = JsonConvert.DeserializeObject<ApiResponseModel<UserRoleMappingDTO>>(apiResponse);
        //        if (result?.IsSuccess == true)
        //        {
        //            responseModel.IsSuccess = true;
        //            responseModel.Message = "User-role mapping updated successfully!";
        //          //responseModel.Data = result.Data;
        //            return Json(responseModel);
        //        }
        //        else
        //        {
        //            responseModel.Message = result?.Message ?? "An error occurred while updating user-role mapping.";
        //            return Json(responseModel);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        responseModel.Message = "An unexpected error occurred while processing your request.";
        //        return Json(responseModel);
        //    }
        //}
        #endregion
        #region DELETE
        //public async Task<IActionResult> DeleteUserRoleMapping(int roleUserId)
        //{
        //    ApiResponseModel<UserRoleMappingDTO> apiResponse = new ApiResponseModel<UserRoleMappingDTO>();


        //    try
        //    {
        //        // Call the helper method to delete the user role mapping
        //        var apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.DeleteUserRoleMappingUrl}/{roleUserId}";
        //        var deleteResponse = await RestApiUserServiceHelper.Instance.DeleteUserRoleMappingAsync(apiUrl);

        //        // Check the response and update the result accordingly
        //        if (deleteResponse?.IsSuccess == true)
        //        {
        //            deleteResponse.IsSuccess = true;
        //            deleteResponse.Message = "User-role mapping deleted successfully!";
        //        }
        //        else
        //        {
        //            deleteResponse.Message = deleteResponse?.Message ?? "An error occurred while deleting the user-role mapping.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        responseModel.Message = "An unexpected error occurred while processing your request.";
        //        // Log the exception if necessary
        //    }

        //    return Json(responseModel);
        //}
        #endregion
    }
}
