using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.WebApp.Helpers;
using System.Text;
using Microsoft.Extensions.Options;
using Payroll.WebApp.Models.DTOs;
using Payroll.Common.CommonDto;
using UserService.BAL.Models;
using Newtonsoft.Json;
using UserService.BAL.Requests;
using PayrollMasterService.BAL.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Payroll.WebApp.Models;
using Payroll.Common.Repository.Interface;
using static Payroll.Common.EnumUtility.EnumUtility;
using System.Net.Http;
using System.Text.Json;
using Payroll.Common.ViewModels;
using System.Net;
using System.Diagnostics.Contracts;
using Payroll.Common.Helpers;
using Microsoft.IdentityModel.Tokens;
using PayrollTransactionService.BAL.Models;
using static Payroll.WebApp.Helpers.SessionHelper;

namespace Payroll.WebApp.Controllers
{
    public class UserController : SharedUtilityController
    {
        private readonly IConfiguration _configuration;
        // private readonly HttpClient _httpClient;
        // private readonly CommonHelper _commonHelper;
        private readonly ICachingServiceRepository _cachingService;
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
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
                //var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                var sessionUserData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");


                if (sessionUserData == null)
                {
                    return 0;
                }
                if (sessionUserData.CompanyDetails == null || !sessionUserData.CompanyDetails.Any())
                {
                    return 0; 
                }

                return sessionUserData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
            }
        }
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }
        //public UserController(RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings, IConfiguration configuration,
        public UserController(RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings, IConfiguration configuration,
             //CommonHelper commonHelper,
             ICachingServiceRepository cachingService
            //, IHttpClientFactory httpClientFactory
            )
        {
            this._mapper = mapper;
            this._apiSettings = apiSettings.Value;
            this._configuration = configuration;
            //this._httpClient = httpClient;
            //this._commonHelper = commonHelper;
            this._cachingService = cachingService;
            //this._dropdownHelper = dropdownHelper;
            //_httpClientFactory = httpClientFactory;
            _userServiceHelper = userServiceHelper;
            _masterServiceHelper = masterServiceHelper;
        }

        #region User Crud Functionality // Rohit Tiwri NOTE :- Not tested in web controller side because I have no UI designs.

        /// <summary>
        ///  Developer Name :- Rohit Tiwari
        ///  Message detail :- It is user index page that show all user based on it's org... . 
        ///  Created Date   :- 09-Sep-'24
        ///  Change By      :- Harshida Parmar
        ///  Change Date    :- 31-Dec-'24
        ///  Change detail  :- Earlier created for testing purpose now fetching all USER data. 
        /// </summary>
        /// <returns>(Index.cshtml)</returns>
        public async Task<IActionResult> Index()
        {
            //Commented By Harshida: -30-12-'24 :- Start
            //ViewBag.PageTitle = RouteUrlConstants.ManageUser;
            //return View();
            //await SetUserPermissions();
            //Commented By Harshida: -30-12-'24 :- End
            var response = new ApiResponseModel<UserInfoDTO> { IsSuccess = false };
            try
            {
                string apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetAllUserUrl;
                var apiResponse = await _userServiceHelper.GetUsersAsync($"{apiUrl}");
                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    IEnumerable<UserInfoDTO> users = _mapper.Map<IEnumerable<UserInfoDTO>>(apiResponse.Result);
                    // Count total, active, and suspended users
                    int totalUsers = users.Count();
                    int activeUsers = users.Count(u => u.IsActive == true);
                    int suspendedUsers = users.Count(u => u.IsActive == false);

                    // Pass counts to view
                    ViewBag.TotalUsers = totalUsers;
                    ViewBag.ActiveUsers = activeUsers;
                    ViewBag.SuspendedUsers = suspendedUsers;
                    return View(users);
                }
                else
                {
                    ViewBag.TotalUsers = 0;
                    ViewBag.ActiveUsers = 0;
                    ViewBag.SuspendedUsers = 0;
                    return View(new List<UserInfoDTO>());
                }
            }
            catch (Exception ex)
            {
                var logErrorMessage = ex.Message;

                return new JsonResult(new ApiResponseModel<UserInfoDTO>
                {
                    IsSuccess = false,
                    Message = MessageConstants.TechnicalIssue,
                    StatusCode = ApiResponseStatusConstant.InternalServerError
                });
            }
        }


        /// <summary>
        ///  Developer Name :- Rohit Tiwari
        ///  Message detail :- Load users data into index page. 
        ///  Created Date   :- 09-Sep-2024
        ///  Change Date    :- 09-Sep-2024
        ///  Change detail  :- NOT YET
        /// </summary>
        /// <returns>(Index.cshtml)</returns>
        [HttpPost]
        public async Task<IActionResult> GetUsers()
        {
            var response = new ApiResponseModel<UserDTO> { IsSuccess = false };
            try
            {
                string apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetAllUserUrl;
                var apiResponse = await _userServiceHelper.GetUsersAsync($"{apiUrl}");
                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    IEnumerable<UserDTO> users = _mapper.Map<IEnumerable<UserDTO>>(apiResponse.Result);
                    return View(users);
                }
                else
                {
                    return View(new List<UserDTO>());
                }
            }
            catch (Exception ex)
            {
                var logErrorMessage = ex.Message;

                return new JsonResult(new ApiResponseModel<UserDTO>
                {
                    IsSuccess = false,
                    Message = MessageConstants.TechnicalIssue,
                    StatusCode = ApiResponseStatusConstant.InternalServerError
                });
            }
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of user details using the mapping based on the provided organization data.
        ///  Created Date   :- 26-Dec-2024
        ///  Change Date    :- 26-Dec-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="model"> user detail to be added.</param>
        /// <returns>Returns a model response with the result of the operation.</returns>

        public async Task<IActionResult> AddRecord()
        {
            //await SetUserPermissions();
            return View();
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of user details using the mapping based on the provided organization data.
        ///  Created Date   :- 26-Dec-2024
        ///  Change Date    :- 26-Dec-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="model"> user detail to be added.</param>
        /// <returns>Returns a model response with the result of the operation.</returns>

        [HttpPost]
        public async Task<IActionResult> AddRecord([FromBody] UserDTO userDto)
        {
            try
            {
                // Map UserDTO to UserRequest using AutoMapper
                userDto.CreatedBy = SessionUserId;
                var userRequest = _mapper.Map<UserRequest>(userDto);
                // Call PostUserAsync method to post user data
                var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.PostUserUrl;
                var apiResponse = await _userServiceHelper.PostUserAsync(apiUrl, userRequest);
                if (apiResponse.IsSuccess)
                {
                    var userId = (apiResponse.Result as UserDTO)?.UserId;  //Added By Harshida Get and Return USERID in JSON:- (13-01-'25)
                    var usereffectiveFromDt = (apiResponse.Result as UserDTO)?.EffectiveFromDt;  //Added By Harshida Get and Return USERID in JSON:- (20-01-'25)
                    return Json(new
                    {
                        success = true,
                        message = apiResponse.Message,
                        userId = userId,
                        usereffectiveFromDt = usereffectiveFromDt
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

        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API is used for Change password for User.
        ///  Created Date   :- 12-Feb-2025
        ///  Change Date    :- 
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>

        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangepwdDTO ChangePwdDto)
        {
            try
            {
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                // Extract companyId, roleId, and userId
                int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
                if (ChangePwdDto.UserId == 0)
                {
                    ChangePwdDto.UserId = userId;
                }
                //ChangePwdDto.CreatedBy = 1;
                var userRequest = _mapper.Map<UserRequest>(ChangePwdDto);


                var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.UpdateUserPassword;
                var apiResponse = await _userServiceHelper.PostUserAsync(apiUrl, userRequest);
                if (apiResponse.IsSuccess)
                {
                    var userIds = (apiResponse.Result as UserDTO)?.UserId;
                    var usereffectiveFromDt = (apiResponse.Result as UserDTO)?.EffectiveFromDt;  //Added By Harshida Get and Return USERID in JSON:- (20-01-'25)
                    return Json(new
                    {
                        success = true,
                        message = apiResponse.Message
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

        #region Fetch and Save Data for User Permission Added By Priyanshi Jain 
        [HttpGet]
        public async Task<IActionResult> FetchUserRoleMenuByRoleId(int roleId, int company_Id)
        {
            var response = new ApiResponseModel<UserRoleMenuDTO> { IsSuccess = false };
            string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetUserRoleMenuByroleIdUrl}/{roleId}?Company_id={company_Id}&IsRenderInMenu=true";
            // Call the dynamic GetAsync method to fetch the data
            var apiResponse = await _userServiceHelper.GetCommonAsync<List<UserRoleMenuDTO>>(apiUrl);
            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                return Json(new { success = true, data = new { result = apiResponse.Result } });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }

        /// <summary>
        /// Developer Name : Krunali gohil - payroll : 377
        /// Message Details : Select role to view permission 
        /// Created date :  15/01/2025
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> FetchUserRoleMenuByUserId(int userId)
        {
            var response = new ApiResponseModel<UserRequest> { IsSuccess = false };

            string apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetUserMapDetailsById}/{userId}";

            // Call the dynamic GetAsync method to fetch the data
            var apiResponse = await _userServiceHelper.GetUserRoleByIdAsync(apiUrl);
            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                return Json(new { success = true, data = new { result = apiResponse.Result.maproleDetails } });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }

        }

        /// <summary>
        /// Developer Name : Krunali gohil - payroll : 377
        /// Message Details : Select role to view permission 
        /// Created date :  15/01/2025
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> FetchUserRoleMenuByUserIdRoleIdCompanyId(int companyId, int roleId, int userId)
        {
            //var session = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            var session = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
            //var companyDetailId = session.CompanyDetails.FirstOrDefault().Company_Id;
            int companyDetailId = session.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
            //var companyDetailId = 32;
            var response = new ApiResponseModel<UserRoleBasedMenuRequest> { IsSuccess = false };
            string apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetUserRoleMenuUrl}?companyId={companyDetailId}&roleid={roleId}&userid={userId}&IsRenderInMenu=true";

            // Call the dynamic GetAsync method to fetch th.e data
            var apiResponse = await _userServiceHelper.GetUserRoleMenuAsync(apiUrl);
            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                return Json(new { success = true, data = new { result = apiResponse.Result } });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }

        }

        [HttpGet]
        public async Task<IActionResult> FetchUserRoleMenuEditByUserIdRoleIdCompanyId(int companyId, int roleId, int userId, int roleMenuHeaderId,int correspondanceId)
        {
            var response = new ApiResponseModel<UserRoleBasedMenuRequest> { IsSuccess = false };
            string apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetUserRoleMenuEditUrl}?companyId={companyId}&roleid={roleId}&userid={userId}&roleMenuHeaderId={roleMenuHeaderId}&correspondanceId={correspondanceId}";

            // Call the dynamic GetAsync method to fetch th.e data
            var apiResponse = await _userServiceHelper.GetUserRoleMenuAsync(apiUrl);
            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                return Json(new { success = true, data = new { result = apiResponse.Result } });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }

        }

        [HttpGet]
        public async Task<IActionResult> FetchUserLocationWiseRole(int userId, int companyId, int? correspondanceId)
        {
            var apiResponse = new ApiResponseModel<object> { IsSuccess = false };

            string apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetUserLocationWiseRoleEditUrl}?userId={userId}&companyId={companyId}&correspondanceId={correspondanceId}";

            // Call the dynamic GetAsync method to fetch the data
            apiResponse = await _userServiceHelper.GetUserLocationWiseRoleAsync<object>(apiUrl);

            if (apiResponse.IsSuccess && apiResponse.Result != null)
            {
                // Ensure the API response is correctly structured
                Console.WriteLine($"API Response: {JsonConvert.SerializeObject(apiResponse.Result)}");

                // Convert Result into a JSON string first, then deserialize into a strongly typed model
                var jsonString = JsonConvert.SerializeObject(apiResponse.Result);

                var resultData = JsonConvert.DeserializeObject<UserRoleResponseModel>(jsonString);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        locationWiseRoles = resultData?.LocationWiseRoles ?? new List<LocationWiseRole>(),
                        roleMenuHeaders = resultData?.RoleMenuHeaders ?? new List<RoleMenuHeader>()
                    }
                });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves save User Role Menu By RoleId details from the database 
        ///                    using a stored procedure. It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 10-Jan-2025
        ///  Change Date    :- 17-Jan-2025
        ///  Change detail  :- (By Harshida) As recevied the data and call the API. 
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>

        [HttpPost]
        public async Task<IActionResult> SaveUserRoleMenuPermissions([FromBody] RoleMenuPermissionsModelDTO request)
        {
            var session = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            //int userId = int.TryParse(session.UserId, out var parsedUserId) ? parsedUserId : 0;
            var apiRequest = RoleMenuPermissionsMapper.MapToApiRequest(request, request.UserId, DateTime.Now); //Based on Session Instead 1 we will pass Session
            var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.PostOrPutUserRoleMenuUrl;
            var responseModel = await _userServiceHelper.PostUserRoleMenuMappingAsync(apiUrl, apiRequest);
            //if (!string.IsNullOrEmpty(responseModel.Message))
            if (responseModel.Message == "Record Created Successfully")
            {
                return Ok(new { message = responseModel.Message, type = "success" });
            }
            else
            {
                return Ok(new { message = ApiResponseMessageConstant.SomethingWrong, type = "danger" });
            }
        }

        #endregion
        public async Task<IActionResult> GetUserRecordById(int userId)
        {
            try
            {
                string apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetEditUserMapDetailsById}/{userId}";
                var apiResponse = await _userServiceHelper.GetByIdCommonAsync<UserMapDetailsDTO>(apiUrl, userId);
                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    var user = _mapper.Map<UserMapDetailsDTO>(apiResponse.Result);
                    ViewBag.EditSessionUserId = SessionUserId;
                   // return Json(new { IsSuccess = true, Data = user, EditSessionUserId = SessionUserId });
                    return Json(new { IsSuccess = true, Data = user});
                }

                return Json(new { IsSuccess = false, Message = apiResponse.Message });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Message = "An error occurred while fetching user data.",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRoleStatusMaster([FromBody] UpdateUserRoleStatusDTO updateUserRoleStatusDTO)
        {
            updateUserRoleStatusDTO.UpdatedBy = SessionUserId; //Added By Priyanshi
            updateUserRoleStatusDTO.UpdatedDate = DateTime.Now;
            var userRoleStatusRequest = _mapper.Map<UpdateUserRoleStatusModel>(updateUserRoleStatusDTO);
            // Define API URL
            var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.UpdateUserRoleStatusMaster;
            // Call the generic PostAsync method to post company data
            var apiResponse = await _userServiceHelper
                                .PutCommonAsync<UpdateUserRoleStatusModel, UpdateUserRoleStatusDTO>(apiUrl, userRoleStatusRequest);
            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserLocationStatusMaster([FromBody] UpdateUserLocationStatusDTO updateUserLocationStatusDTO)
        {
            updateUserLocationStatusDTO.UpdatedBy = SessionUserId;
            updateUserLocationStatusDTO.UpdatedDate = DateTime.Now;
            var userLocationStatusRequest = _mapper.Map<UpdateUserLocationStatusModel>(updateUserLocationStatusDTO);
            // Define API URL
            var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.UpdateUserLocationStatusMaster;
            // Call the generic PostAsync method to post company data
            var apiResponse = await _userServiceHelper
                                .PutCommonAsync<UpdateUserLocationStatusModel, UpdateUserLocationStatusDTO>(apiUrl, userLocationStatusRequest);
            if (apiResponse.IsSuccess)
            {
                return Json(new { success = true, message = apiResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = apiResponse.Message });
            }

        }

        public async Task<IActionResult> AddMapUserLocationRecord([FromBody] MapUserLocationDTO mapUserLocationDTO)
        {
            try
            {
                if (mapUserLocationDTO == null || mapUserLocationDTO.UserMapLocations == null || !mapUserLocationDTO.UserMapLocations.Any())
                {
                    return Json(new { success = false, message = "Invalid data. UserMapLocations cannot be empty." });
                }

                // Set CreatedBy field (assuming 1 is a placeholder)
                mapUserLocationDTO.CreatedBy = SessionUserId;

                // Map DTO to Request Model
                var mapUserLocationRequest = _mapper.Map<MapUserLocation>(mapUserLocationDTO);

                // API URL for posting the user location
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostMapUserLocationDetail;

                // Call API via helper
                var apiResponse = await _masterServiceHelper.PostCommonAsync<MapUserLocation, MapUserLocationDTO>(apiUrl, mapUserLocationRequest);

                if (apiResponse != null && apiResponse.IsSuccess)
                {
                    return Json(new
                    {
                        success = true,
                        message = apiResponse.Message,
                    });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse?.Message ?? "Failed to save user location." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> AddMapUserRoleRecord([FromBody] UserRoleMappingDTO userRoleMappingDTO)
        {
            try
            {
                if (userRoleMappingDTO == null || userRoleMappingDTO.Role_Ids == null || !userRoleMappingDTO.Role_Ids.Any())
                {
                    return Json(new { success = false, message = "Invalid data. UserMapLocations cannot be empty." });
                }

                // Set CreatedBy field (assuming 1 is a placeholder)
                userRoleMappingDTO.CreatedBy = SessionUserId;

                // Map DTO to Request Model
                var mapUserroleRequest = _mapper.Map<UserRoleMapping>(userRoleMappingDTO);

                // API URL for posting the user location
                var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.PostUserRoleMappingUrl;

                // Call API via helper
                var apiResponse = await _masterServiceHelper.PostCommonAsync<UserRoleMapping, UserRoleMappingDTO>(apiUrl, mapUserroleRequest);

                if (apiResponse != null && apiResponse.IsSuccess )
                {
                    return Json(new
                    {
                        success = true,
                        message = apiResponse.Message,
                    });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse?.Message ?? "Failed to save user location." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> UpdateRecord()
        {
            //await SetUserPermissions();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRecord([FromBody] UserDTO userDto)
        {
            var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.PutUserUrl;
            var userEntity = _mapper.Map<UserRequest>(userDto);
            var responseModel = await _userServiceHelper.PutUserAsync(apiUrl, userEntity);
            return Json(responseModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDeactivateUserRecord([FromBody] DeactivateUserDTO deactivateUserDTO)
        {
            await SetUserPermissions(); //Added By Chirag
            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
            try
            {
                deactivateUserDTO.UpdatedBy = SessionUserId;
                deactivateUserDTO.Effective_On = DateTime.Now;
                // Assuming you are updating the area using a service or repository
                var updateApiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.DeactivateUserUrl}";
                var updateResponse = await _userServiceHelper.PutCommonAsync<DeactivateUserDTO, DeactivateUser>(updateApiUrl, deactivateUserDTO);

                if (updateResponse.IsSuccess)
                {
                    return Json(new { success = true, message = updateResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = updateResponse.Message ?? "Failed to update the area." });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                // Optionally log the exception details
                Console.WriteLine($"Exception in UpdateArea: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the area." });
            }
        }

        public async Task<IActionResult> DeleteRecord()
        {
            //await SetUserPermissions();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerifyResetPasswordLink([FromQuery] string encryptedCode)
        {
            var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.PostVerificationForResetPasswordUrl;
            string queryParams = $"?encryptedCode={encryptedCode}";
            var responseModel = await _userServiceHelper.VerifyResetPasswordLinkAsync($"{apiUrl}{queryParams}");

            // Test Case for Link For Password Reset.
            //responseModel.Result.LinkExpireDate = new DateTime(2024, 12, 05);
            //responseModel.Result = null;

            if (responseModel.Result != null && responseModel.Result.UserCurrentDateTime > responseModel.Result.LinkExpireDate)
            {
                // Link expired or invalid
                return View("_LinkExpiredPage");
            }
            else if (responseModel.Result == null)
            {
                // Not found page
                return View("_LinkNotFoundPage");
            }
            else if (responseModel.Result.IsLinkActivated == true)
            {
                // Link already used.
                return View("_LinkAlreadyUsedPage");
            }
            else
            {
                // Rohit Tiwari :- store in session not in hidden field.
                HttpContext.Session.SetString("UserEmailForResetPasswordRequest", responseModel.Result.Email);
                return View("_PasswordResetPage");
            }
        }
        #endregion

        #region Update User Functionality

        /// <summary>
        ///  Developer Name :- Rohit Tiwari
        ///  Message detail :- Change password with two way ForgotPassword & ResetPassword.
        ///  Created Date   :- 24-Sep-2024
        ///  Change Date    :- NONE CHANGES
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> UpdateUserPassword([FromBody] SendEmailDTO model)
        {
            var responseModel = new ApiResponseModel<SendEmailDTO>
            {
                IsSuccess = false
            };
            try
            {
                if (model.Email == "")
                {
                    model.Email = HttpContext.Session.GetString("UserEmailForResetPasswordRequest");
                }
                var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.ChangePasswordUrl;
                var entity = _mapper.Map<SendEmailModel>(model);
                responseModel = await _userServiceHelper.PostUpdateUserPasswordAsync(apiUrl, entity);
                if (responseModel.IsSuccess == true)
                {
                    responseModel.IsSuccess = true;
                    //responseModel.RedirectUrl = RouteUrlConstants.LoginPageUrl; // Commented By Harshida 05-02-25
                    responseModel.RedirectUrl = _configuration["RoutingUrl:SiteUrl"]; // Added By Harshida 05-02-25
                    responseModel.Message = MessageConstants.PasswordChangeSuccess;
                    return Json(responseModel);
                }
                else
                {
                    responseModel.Message = responseModel.Message;
                    return Json(responseModel);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (implement logging here)
                responseModel.Message = "An error occurred while processing your request.";
            }

            return Json(responseModel);
        }

        #endregion

        #region User Profile Page Added by Priyanshi Jain

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- Manage user info setting from here.
        ///  Created Date   :- 10-Jan-2025
        ///  Change Date    :- NONE CHANGES
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UserProfile(string userId, bool isFromMenu = false)
        {
            //await SetUserPermissions();
            ViewBag.IsFromMenu = isFromMenu;
            int? decrypteduserId = null;
            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    string decryptedIdStr = SingleEncryptionHelper.Decrypt(userId);

                    if (int.TryParse(decryptedIdStr, out int parseduserId))
                    {
                        decrypteduserId = parseduserId;
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
            var response = new ApiResponseModel<UserInfoDTO> { IsSuccess = false };
            try
            {
                string apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetUserMapDetailsById}/{decrypteduserId}";
                var apiResponse = await _userServiceHelper.GetUserByIdAsync(apiUrl);

                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    var user = _mapper.Map<UserInfoDTO>(apiResponse.Result); // Map to a single object
                    user.Username = apiResponse.Result.Username;
                    return View(user); // Pass a single object to the view
                }
                else
                {
                    return View(new UserInfoDTO()); // Return an empty object in case of failure
                }

            }
            catch (Exception ex)
            {
                var logErrorMessage = ex.Message;

                return new JsonResult(new ApiResponseModel<UserDTO>
                {
                    IsSuccess = false,
                    Message = MessageConstants.TechnicalIssue,
                    StatusCode = ApiResponseStatusConstant.InternalServerError
                });
            }
        }

        #endregion

        #region Delete User Record [Added By Harshida 31-12-'24]

        public async Task<IActionResult> DeleteUser(UserInfoDTO model)
        {
            #region Read Session for Updated By 

            ////Note: - Rohit has wrote UserID as string in SessionViewModel
            model.UpdatedBy = SessionUserId;
            #endregion

            string apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.DeleteUserByIdUrl;
            string queryParams = $"/{model.UserId}";
            var userRequest = _mapper.Map<UserRequest>(model);
            HttpResponseMessage response = await _userServiceHelper.DeleteUserAsync($"{apiUrl}{queryParams}", userRequest);
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                return Json(MessageConstants.RecordDeleted);
            }
            else
            {
                return Json(MessageConstants.TechnicalIssue);
            }
        }
        /// <summary>
        /// Created By :-Harshida Parmar
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="roleId"></param>
        /// <param name="defaultType"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateDefaultLocationRole(int? locationId, int? roleId, string defaultType)
        {
            var session = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            int userId = int.TryParse(session.UserId, out var parsedUserId) ? parsedUserId : 0;
            RoleOrLocationDTO defaultRoleLocationDetails = new RoleOrLocationDTO
            {
                UpdateType = defaultType,
                UserId = userId, //Later I will USER SESSION 
                Role_User_Id = roleId,
                UserMapLocation_Id = locationId
            };
            string apiUrl = null;
            try
            {
                // Call the API to update the default location
                if (locationId != null)
                {
                    apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.UpdateUserRoleLocation}/{locationId}";
                }
                else
                {
                    apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.UpdateUserRoleLocation}/{roleId}";
                }
                var defaultRequest = _mapper.Map<RoleOrLocationRequest>(defaultRoleLocationDetails);

                var response = await _userServiceHelper.PutDefaultLocationRole($"{apiUrl}", defaultRequest);
                if (response.MessageType == 1 && response.UpdateType== "Location")//Why "Location" Because in SP IT HAS if condition
                {
                    // var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                    var sessionData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");

                    if (sessionData != null)
                    {
                        sessionData.LocationDetails = response.LocationDetails.Select(ld => new Payroll.Common.ViewModels.UserLocationDetails
                        {
                            UserMapLocation_Id = ld.UserMapLocation_Id,
                            Location_ID = ld.Location_ID,
                            LocationName = ld.LocationName,
                            CompanyFullLocation = ld.CompanyFullLocation,
                            Default_Location = ld.Default_Location,
                            Primary_Email_Id = ld.Primary_Email_Id,
                            Primary_Phone_No = ld.Primary_Phone_No
                        }).ToList();

                        sessionData.RoleDetails = response.RoleDetails?.Select(rd => new Payroll.Common.ViewModels.UserRoleDetails
                        {
                            Role_User_Id = rd.Role_User_Id,
                            IsDefault_Role = rd.IsDefault_Role,
                            Role_Id = rd.Role_Id,
                            RoleName = rd.RoleName
                        }).ToList() ?? new List<Payroll.Common.ViewModels.UserRoleDetails>();
                        //SessionHelper.SetSessionObject(HttpContext, "UserSessionData", sessionData); 
                        HttpContext.Session.SetInt32("IsDefaultLocationChanges", 1);
                        HttpContext.Session.SetInt32("IsLocationChanges", 0);
                        SessionHelper.SetSessionObject(HttpContext, "UserDetailData", sessionData);
                    }                  
                    return Json(new
                    {
                        success = true,
                        message = response.StatusMessage
                    });
                }
                else if (response.MessageType == 1 && response.UpdateType == "Role") //Why "Role" Because in SP IT HAS if condition
                {
                    //var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                    var sessionData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");

                    if (sessionData != null)
                    {
                        sessionData.RoleDetails = response.RoleDetails?.Select(rd => new Payroll.Common.ViewModels.UserRoleDetails
                        {
                            Role_User_Id = rd.Role_User_Id,
                            IsDefault_Role = rd.IsDefault_Role,
                            Role_Id = rd.Role_Id,
                            RoleName = rd.RoleName
                        }).ToList() ?? new List<Payroll.Common.ViewModels.UserRoleDetails>();
                        HttpContext.Session.SetInt32("IsDefaultLocationChanges", 1);
                        HttpContext.Session.SetInt32("IsLocationChanges", 0);
                        HttpContext.Session.SetInt32("IsDefaultRoleChanges", 1);
                        HttpContext.Session.SetInt32("IsRoleChanges", 0);
                        // SessionHelper.SetSessionObject(HttpContext, "UserSessionData", sessionData); // Save session
                        SessionHelper.SetSessionObject(HttpContext, "UserDetailData", sessionData); // Save session
                    }                   
                    return Json(new
                    {
                        success = true,
                        message = response.StatusMessage
                    });
                }
                else //Technical Issue
                {
                    return Json(MessageConstants.TechnicalIssue);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> CheckUserExist(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new
                {
                    success = false,
                    message = ApiResponseMessageConstant.InvalidData
                });
            }
            string apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.CheckUserEmail}?email={email}";
            // Call the dynamic GetAsync method to fetch the data            
            string responseMessage = await _userServiceHelper.GetCheckUserEmailAsync(apiUrl);
            if (responseMessage == ApiResponseMessageConstant.NoIssues)
            {
                return Json(new
                {
                    success = true,
                    message = responseMessage
                });
            }
            else
            {

                return Json(new
                {
                    success = false,
                    message = responseMessage
                });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetRLWithoutDefaultUpdate(int locationId)
        {
            var response = new ApiResponseModel<UserCompanyRoleLocation> { IsSuccess = false };          
            try
            {
                string apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetUserRoleLocation}?userId={SessionUserId}&companyId={SessionCompanyId}&userMapLocationId={locationId}";
                var apiResponse = await _userServiceHelper.FetchApiResponseAsync<UserCompanyRoleLocation>(apiUrl);

                if (apiResponse.IsSuccess && apiResponse.Result != null)
                {
                    var userDetails = apiResponse.Result;

                    //var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                    HttpContext.Session.SetInt32("SelectedLocationId", locationId);

                    var sessionData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");

                    if (sessionData != null)
                    {  
                        // Update RoleDetails in session
                        sessionData.RoleDetails = userDetails.RoleDetails?.Select(rd => new Payroll.Common.ViewModels.UserRoleDetails
                        {
                            Role_User_Id = rd.Role_User_Id,
                            IsDefault_Role = rd.IsDefault_Role,
                            Role_Id = rd.Role_Id,
                            RoleName = rd.RoleName
                        }).ToList() ?? new List<Payroll.Common.ViewModels.UserRoleDetails>();

                        // Save updated session data
                        SessionHelper.SetSessionObject(HttpContext, "UserDetailData", sessionData);
                    }
                    HttpContext.Session.SetInt32("IsDefaultLocationChanges", 0);
                    HttpContext.Session.SetInt32("IsLocationChanges", 1); //Session to check Only Location Changed FROM DP 

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetRoleWithoutDefaultUpdate(int roleId)
        {
            // Retrieve the session object
            var sessionData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");

            if (sessionData?.RoleDetails != null)
            {
                var matchingRole = sessionData.RoleDetails.FirstOrDefault(r => r.Role_User_Id == roleId);

                if (matchingRole != null)
                {
                    // Extract the Role_Id and store it in session
                    HttpContext.Session.SetInt32("SelectedRoleId", matchingRole.Role_Id);
                    HttpContext.Session.SetInt32("IsDefaultRoleChanges", 0);
                    HttpContext.Session.SetInt32("IsRoleChanges", 1);  //Session to check Only Role Changed FROM DP 
                }
            }
           
            return Json(new { success = true });          
        }
        #endregion


        [HttpGet]
        public async Task<IActionResult> GetBreadcrumbByMenuIdAsync(int menu_Id)
        {
            string apiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetBreadCrumbsUrl}?menu_Id={menu_Id}";

            try
            {
                var apiResponse = await _userServiceHelper.GetAsync<ApiResponseModel<List<BreadCrumbDTO>>>(apiUrl);

                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    return PartialView("BreadCrumbPartial", apiResponse.Result); // Return the partial view
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching breadcrumb: " + ex.Message);
            }

            return PartialView("BreadCrumbPartial", new List<BreadCrumbDTO>());
        }
    }
}
