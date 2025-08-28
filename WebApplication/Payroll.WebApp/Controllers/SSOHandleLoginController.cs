using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Options;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.Helpers;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using UserService.BAL.Requests;
using Payroll.Common.CommonRequest;
using NuGet.Common;
using UserService.BAL.Models;
using System.Text.Json;


namespace Payroll.WebApp.Controllers
{
    public class SSOHandleLoginController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;

        private readonly IConfiguration _configuration;

        private readonly RestApiUserServiceHelper _userServiceHelper;
        public SSOHandleLoginController(RestApiUserServiceHelper userServiceHelper, IOptions<ApiSettings> apiSettings, IMapper mapper, IConfiguration configuration)
        {
            this._apiSettings = apiSettings.Value;
            this._mapper = mapper;
            this._configuration = configuration;
            _userServiceHelper = userServiceHelper;
        }
        [HttpPost]
        public async Task<IActionResult> LoginFromCLMS(int UserId, string UserName, string UserStatus)
        {
            var loginResponse = new ApiResponseModel<string> { IsSuccess = false };
            var authResponse = new ApiResponseModel<UserRequest> { IsSuccess = false };
            LoginDTO loginDto = new LoginDTO();
            try
            {
                if (string.IsNullOrEmpty(UserName))
                {
                    loginResponse.Message = MessageConstants.InvalidLoginRequest;
                    return Ok(loginResponse);
                }
                string userAuthApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetByIdAuth;
                //string queryParams = $"/{UserName}";
                string queryParams = $"?id={UserName}&isClmsUser={1}";
                var userAuthResult = await _userServiceHelper.GetUsersRecordAsync($"{userAuthApiUrl}{queryParams}");
				#region If Location/Role NULL 
				if (userAuthResult == null || userAuthResult.Result == null || !userAuthResult.IsSuccess)
				{
					return RedirectToAction("Access_Denied", "Account");
				}
				#endregion

				var token = JwtTokenGeneratorHelper.GetToken(userAuthResult.Result.Email, new List<string> { userAuthResult.Result.RoleName }, _configuration);

                userAuthResult.Token = token;
                authResponse.Result = userAuthResult.Result;
                //Console.WriteLine(JsonSerializer.Serialize(authResponse.Result, new JsonSerializerOptions
                //{
                //    WriteIndented = true, // makes the JSON output pretty
                //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // optional
                //}));
                if (HandleLoginResponse(userAuthResult))
                {
                    string userLoginTranPostUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.PostLoginHistoryUrl;

                    LoginHistory loginHistory = new LoginHistory
                    {
                        UserId = UserId,
                        LogInTime = DateTime.Now,
                        IsLoggedOut = true,
                        IpAddress = GetClientIpAddress()
                    };
                    var result = await _userServiceHelper.PostSignleAsync(userLoginTranPostUrl, loginHistory);
                    var isHttps = HttpContext.Request.IsHttps;
                    //HttpContext.Session.SetInt32("CLMSLogin", 1);
                    HttpContext.Response.Cookies.Append("CLMSLogin", "1", new CookieOptions
                    {
                        HttpOnly = true, //document.cookie cannot read it, preventing cross-site scripting (XSS) attacks
                        Secure = isHttps,  // Required if using HTTPS
                        IsEssential = true,//Marks the cookie as essential, meaning it is sent even if users decline non-essential cookies
                        Path = "/", // the cookie available for all paths of your domai
                        SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax // Use Lax for non-SSL environments
                    });


                    await HttpContext.Session.CommitAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                loginResponse.Message = MessageConstants.TechnicalIssue;
                return Json(loginResponse);
            }
            return View();
        }
        private string GetClientIpAddress()
        {
            var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor;
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }

            return ipAddress;
        }


        public bool HandleLoginResponse(ApiResponseModel<UserRequest> apiResult)
        {
            if (!string.IsNullOrEmpty(apiResult.Token))
            {
                // Store session.
                var sessionModel = new SessionViewModel
                {
                    UserId = apiResult.Result.UserId.ToString(),
                    Username = apiResult.Result.Email,
                    Pin = apiResult.Result.Pin,
                    Token = apiResult.Token,
                };
                var userSession = new UserSessionViewModel
                {
                    #region Added By Harshida 23-12-'24
                    #region Get User Additional Details ALSO KEEP Payroll.Common.ViewModels to resolve ambiguous
                    CompanyDetails = apiResult.Result.CompanyDetails?.Select(cd => new Payroll.Common.ViewModels.UserCompanyDetails
                    {
                        Company_Id = cd.Company_Id,
                        UserMapCompany_Id = cd.UserMapCompany_Id,
                        CompanyName = cd.CompanyName,
                        StartDate = cd.StartDate,
                        EndDate = cd.EndDate,
                        FinYear = cd.FinYear,
                        Currency_Id = cd.Currency_Id
                    }).ToList() ?? new List<Payroll.Common.ViewModels.UserCompanyDetails>(),

                    LocationDetails = apiResult.Result.LocationDetails?.Select(ld => new Payroll.Common.ViewModels.UserLocationDetails
                    {
                        UserMapLocation_Id = ld.UserMapLocation_Id,
                        Location_ID = ld.Location_ID,
                        LocationName = ld.LocationName,
                        CompanyFullLocation = ld.CompanyFullLocation,
                        Default_Location = ld.Default_Location,
                        Primary_Email_Id = ld.Primary_Email_Id,
                        Primary_Phone_No = ld.Primary_Phone_No
                    }).ToList() ?? new List<Payroll.Common.ViewModels.UserLocationDetails>(),

                    RoleDetails = apiResult.Result.RoleDetails?.Select(rd => new Payroll.Common.ViewModels.UserRoleDetails
                    {
                        Role_User_Id = rd.Role_User_Id,
                        IsDefault_Role = rd.IsDefault_Role,
                        Role_Id = rd.Role_Id,
                        RoleName = rd.RoleName
                    }).ToList() ?? new List<Payroll.Common.ViewModels.UserRoleDetails>()
                    #endregion
                    #endregion
                };
                // Save session data
                HttpContext.Session.SetString("UserSessionActive", "Active");
                SessionHelper.SetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData", userSession);
                SessionHelper.SetSessionObject<SessionViewModel>(HttpContext, "UserSessionData", sessionModel);

                return true; // Login process handled successfully
            }

            return false; // Token is missing, operation failed
        }
    }
}
