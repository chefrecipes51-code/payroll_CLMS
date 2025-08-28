using AutoMapper;
using System.Text;
using Payroll.Common.Helpers;
using Payroll.WebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Models.DTOs;
using Microsoft.Extensions.Options;
using Payroll.Common.ApplicationModel;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.CommonRequest;
using UserService.BAL.Requests;
using Newtonsoft.Json;
using Payroll.Common.CommonDto;
using System.Net;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Payroll.WebApp.Common;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using UserService.BAL.Models;

namespace Payroll.WebApp.Controllers
{
    /// <summary>
    ///  Developer Name :- Rohit Tiwari
    ///  Message detail :- LoginController that manage web application user login activity.
    ///  Created Date   :- 09-Sep-2024
    /// </summary>
    public class AccountController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly IConfiguration _configuration;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly HttpClient _httpClient123;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        //public AccountController(IOptions<ApiSettings> apiSettings, IMapper mapper, IConfiguration configuration,
        //    IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ICompositeViewEngine viewEngine) : base(viewEngine)
        public AccountController(RestApiUserServiceHelper userServiceHelper, IOptions<ApiSettings> apiSettings, IMapper mapper, IConfiguration configuration,
           IHttpContextAccessor httpContextAccessor, ICompositeViewEngine viewEngine) : base(viewEngine)
        {
            this._apiSettings = apiSettings.Value;
            this._mapper = mapper;
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
            this._viewEngine = viewEngine;
            _userServiceHelper = userServiceHelper;
            //this._httpClient123 = httpClient123;
        }

        #region User Login Functionality

        /// <summary>
        ///  Developer Name :- Rohit Tiwari
        ///  Message detail :- This LoginPage() fun give application default Login View Page.
        ///  Created Date   :- 09-Sep-2024
        ///  Change Date    :- 09-Sep-2024
        ///  Change detail  :- Change in Ui part, remove border on login page.
        /// </summary>
        /// <returns>(_LoginPage.cshtml)</returns>
        [HttpGet]
        public IActionResult LoginPage()
        {
            var model = new LoginDTO();
            try
            {
                var session = SessionHelper.GetSessionObject<SessionViewModel>(_httpContextAccessor.HttpContext, "UserSessionData");
                //string username = HttpContext.Session.GetString("UserName");
                if (session.UserId != null && session.Username != null)
                {
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    var encryptedUsername = HttpContext.Request.Cookies["UserLoginUsername"];
                    if (!string.IsNullOrEmpty(encryptedUsername))
                    {
                        model = new LoginDTO
                        {
                            Username = EncryptionHelper.Decrypt(encryptedUsername),
                            RememberMe = true
                        };
                        ViewBag.Username = model.Username; //Added By Harshida 11-02-'25 Major Change so Used ViewBag
                                                           //return PartialView("_LoginPage", model);
                        return View("_LoginPage", model);
                    }
                }
            }
            catch (Exception ex)
            {
                // log ex.Message error message in db.
                //return PartialView("_LoginPage", null);
                return View("_LoginPage", model);
            }            
            return View("_LoginPage", model);
        }


        /// <summary>
        ///  Developer Name :- Rohit Tiwari
        ///  Message detail :- Auth() method authenticates user login requests and receives a token.
        ///  Created Date   :- 12-Sep-2024
        ///  Change Date    :- 18-Sep-2024
        ///  Change detail  :- Manage user authentication process.
        ///  Changed        :- 5-jan-2025 chirag gurjar payroll-359. Removed dependency of authidentity project.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Auth([FromBody] LoginDTO loginDto)
        {
            var loginResponse = new ApiResponseModel<string> { IsSuccess = false };
            var authResponse = new ApiResponseModel<UserRequest> { IsSuccess = false };

            try
            {
                if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                {
                    loginResponse.Message = MessageConstants.InvalidLoginRequest;
                    return Ok(loginResponse);
                }

                //Code commented  due to unnessesary by Abhishek 21-02-25
                //TempData["TemplateType"] = _configuration["EmailTemplateSettings:LoginWithOTP"];
                ////SetTemplateType("LoginWithOTP");
                //var session = SessionHelper.GetSessionObject<SessionViewModel>(_httpContextAccessor.HttpContext, "UserSessionData");

                //if (session == null)
                //{
                //    loginResponse.Result = Url.Action("Account", "LoginPage");
                //    return Json(loginResponse);
                //}
                //if (session.Captcha != loginDto.Captcha)
                //{
                //   // loginResponse.Message = MessageConstants.InvalidCaptcha;
                //   // return Json(loginResponse);
                //}

                // string userAuthApiUrl = _apiSettings.UserAuthServiceEndpoints.AuthUrl;
                string userAuthApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetByIdAuth;
                var userEntity = _mapper.Map<LoginRequest>(loginDto);

                /////////////////////////////

                // var apiResult = await RestApiLoginServiceHelper.Instance.AuthUsersAsync(userAuthApiUrl, userEntity);
                //string queryParams = $"/{userEntity.Username}";
                string queryParams = $"?id={userEntity.Username}&isClmsUser={0}"; // Used these to allow these parameter for CLMS parameter
                var userAuthResult = await _userServiceHelper.GetUsersRecordAsync($"{userAuthApiUrl}{queryParams}");

                #region Added By Harshida [Jira Ticket 450]
                if (userAuthResult.Message == MessageConstants.RecordNotFoundInAPI)
                {
                    loginResponse.Message = MessageConstants.InvalidLoginRequest;
                    return Json(loginResponse);
                }               
                if (userAuthResult.Message == "One or more user additional details are missing.")
                {
                    loginResponse.Message = MessageConstants.UserRoleLocationNotFound;
                    return Json(loginResponse);                  
                }                
                #endregion

                string userLoginActivityApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.UpdateLoginActivity;
                //var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.PostUserRoleMappingUrl;
                if (userAuthResult.Result.LockAccount)
                {
                    int lockDurationMinutes = _configuration.GetValue<int>("ApplicationUserSettings:UserAccountLockTime");
                    if ((userAuthResult.Result.UserCurrentDateTime - Convert.ToDateTime(userAuthResult.Result.LockTime)).TotalMinutes > lockDurationMinutes)
                    {
                        userAuthResult.Result.LockAccount = false;
                        userAuthResult.Result.MaxAttempts = _configuration.GetValue<int>("Authentication:Attempts");
                        // await _repository.UpdateLoginActivityAsync(DbConstants.UpdateUserLoginActivity, apiResult.Result);

                        //await RestApiLoginServiceHelper.Instance.UpdateLoginActivityAsync(userLoginActivityApiUrl, userAuthResult.Result);
                        await _userServiceHelper.UpdateLoginActivityAsync(userLoginActivityApiUrl, userAuthResult.Result);

                    }
                    else
                    {
                        loginResponse.Message = MessageConstants.AccountInactiveTemporary;
                        return Json(loginResponse);
                    }
                }

                var hashPassword = Payroll.Common.Helpers.GenerateHashKeyHelper.HashKey(loginDto.Password);
                if (hashPassword != userAuthResult.Result.Password)
                {
                    userAuthResult.Result.MaxAttempts--;
                    userAuthResult.Result.LockAccount = userAuthResult.Result.MaxAttempts <= 0;

                    //await RestApiLoginServiceHelper.Instance.UpdateLoginActivityAsync(userLoginActivityApiUrl, userAuthResult.Result);
                    await _userServiceHelper.UpdateLoginActivityAsync(userLoginActivityApiUrl, userAuthResult.Result);
                    loginResponse.Message = userAuthResult.Result.LockAccount ? MessageConstants.AccountInactiveTemporary : userAuthResult.Result.MaxAttempts == 1
                                          ? MessageConstants.PasswordInfoMessage : MessageConstants.InvalidLoginRequest;

                    return Json(loginResponse);
                }

                userAuthResult.Result.MaxAttempts = _configuration.GetValue<int>("Authentication:Attempts");
                //await RestApiLoginServiceHelper.Instance.UpdateLoginActivityAsync(userLoginActivityApiUrl, userAuthResult.Result);
                await _userServiceHelper.UpdateLoginActivityAsync(userLoginActivityApiUrl, userAuthResult.Result);


                var token = JwtTokenGeneratorHelper.GetToken(userAuthResult.Result.Email, new List<string> { userAuthResult.Result.RoleName }, _configuration);

                if (userAuthResult.Result.NextPasswordChangeDate.HasValue)
                {
                    DateTime reminderStartDate = userAuthResult.Result.NextPasswordChangeDate.Value.AddDays(-userAuthResult.Result.PasswordExpiryReminderDays);
                    DateTime currentDateTime = userAuthResult.Result.UserCurrentDateTime;

                    if (reminderStartDate <= userAuthResult.Result.NextPasswordChangeDate)
                    {
                        authResponse.IsSuccess = true;
                        authResponse.Message = $"Please change your password before the date {userAuthResult.Result.NextPasswordChangeDate:yyyy-MM-dd}";

                        if (userAuthResult.Result.NextPasswordChangeDate == currentDateTime)
                        {
                            loginResponse.IsSuccess = true;
                            loginResponse.Message = "Today is the last day to change your password. Please change it.";
                            return Json(loginResponse);
                        }
                        else if (userAuthResult.Result.NextPasswordChangeDate.HasValue &&
         userAuthResult.Result.NextPasswordChangeDate.Value >= currentDateTime)
                        {
                            userAuthResult.Result.LockAccount = true;
                            //await RestApiLoginServiceHelper.Instance.UpdateLoginActivityAsync(userLoginActivityApiUrl, userAuthResult.Result);
                            await _userServiceHelper.UpdateLoginActivityAsync(userLoginActivityApiUrl, userAuthResult.Result);
                            loginResponse.Message = MessageConstants.AccountBlockedMessage;
                            return Json(loginResponse);
                        }
                    }
                }

                userAuthResult.Token = token;
                authResponse.Result = userAuthResult.Result;
                authResponse.IsSuccess = true;
                // return Ok(authResponse);

                ///////////////////////////// 

                if (authResponse.IsSuccess && authResponse.Result != null && authResponse.Result.PayrollConfigAuthType == true)
                {
                    var decryptedAuthType = userAuthResult.Result.AuthType == null
                        ? _configuration["ApplicationSettings:LoginWithNo2FA"]
                        : EncryptionHelper.Decrypt(userAuthResult.Result.AuthType.ToString());

                    //decryptedAuthType = "8790"; // Note :- LOGIN IN @2FA testing purpose remove it in future.

                    loginResponse.Message = userAuthResult.Message;
                    loginResponse.Result = userAuthResult.Result.Email;
                    loginResponse.VerifyUserCode = decryptedAuthType;
                    if (decryptedAuthType == _configuration["ApplicationSettings:LoginWithOTP"])
                    {
                        loginDto.Email = userAuthResult.Result.Email;
                        string partialHtml = RenderPartialViewToString("_OTPVerification", loginDto);
                        loginResponse.AuthCode = _configuration["ApplicationSettings:LoginWithOTP"];
                        loginResponse.IsSuccess = true;
                        loginResponse.TemplateType = _configuration["EmailTemplateSettings:LoginWithOTP"]; ;
                        loginResponse.Data = partialHtml;
                        return Json(loginResponse);
                    }
                    else if (decryptedAuthType == _configuration["ApplicationSettings:LoginWithBiometric"])
                    {
                        string partialHtml = RenderPartialViewToString("_BiometricLogin", loginDto);
                        loginResponse.AuthCode = _configuration["ApplicationSettings:LoginWithBiometric"];
                        loginResponse.IsSuccess = true;
                        loginResponse.Data = partialHtml;
                        return Json(loginResponse);
                    }
                    else
                    {
                        HandleLoginResponse(userAuthResult, loginDto);
                        loginResponse.IsSuccess = true;
                        loginResponse.Result = Url.Action("Index", "Home");
                        return Json(loginResponse);
                    }
                }
                else
                {
                    loginResponse.Message = userAuthResult.Message;
                    return Json(loginResponse);
                }
            }
            catch (Exception ex)
            {
                loginResponse.Message = MessageConstants.TechnicalIssue;
                return Json(loginResponse);
            }
        }

        public bool HandleLoginResponse(ApiResponseModel<UserRequest> apiResult, LoginDTO loginDto)
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

                // Handle "Remember Me" functionality
                //loginDto.RememberMe = true;
                if (loginDto.RememberMe)
                {
                    var encryptedUsername = EncryptionHelper.Encrypt(loginDto.Username);
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(_configuration.GetValue<int>("LoginConfiguration:RememberMeValidity"))
                    };
                    Response.Cookies.Append("UserLoginUsername", encryptedUsername, cookieOptions);
                }
                else
                {
                    Response.Cookies.Delete("UserLoginUsername");
                }

                return true; // Login process handled successfully
            }

            return false; // Token is missing, operation failed
        }

        [HttpPost]
        public IActionResult StoreMenuIdInSession(int menuId)
        {
            HttpContext.Session.SetInt32("SelectedMenuId", menuId);
            return Json(new { success = true, message = "Menu ID set successfully." });
        }


        #endregion

        #region Email Send & OTP Verification code

        /// <summary>
        ///  Developer Name :- Rohit Tiwari
        ///  Message detail :- SendEmail() action send email into tbl_emailprocess table.
        ///  Created Date   :- 24-Sep-2024
        ///  Change Date    :- NONE CHANGES
        /// </summary>
        /// <returns>JsonResult</returns>
        /// 
        #region Rohit OLD CODE 
        //[HttpPost]
        //public async Task<JsonResult> SendEmail([FromBody] SendEmailModel model)
        //{
        //    var responseModel = new ApiResponseModel<UserRequest>
        //    {
        //        IsSuccess = false
        //    };

        //    try
        //    {
        //        var ApiUrl = _configuration["ApiSettings:SendEmailUrl"];
        //        var json = JsonConvert.SerializeObject(model);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");
        //        var httpResponse = await _httpClient123.PostAsync(ApiUrl, content);
        //        var apiResponse = await httpResponse.Content.ReadAsStringAsync();
        //        var apiResult = JsonConvert.DeserializeObject<ApiResponseModel<ResponseModel>>(apiResponse);

        //        if (!apiResult.IsSuccess)
        //        {
        //            responseModel.Message = apiResult.Message;
        //        }
        //        else
        //        {
        //            responseModel.Message = apiResult.Message;
        //            responseModel.IsSuccess = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (implement logging here)
        //        responseModel.Message = "An error occurred while processing your request.";
        //    }

        //    return Json(responseModel);
        //}
        #endregion
        [HttpPost]
        public async Task<JsonResult> SendEmail([FromBody] SendEmailModel model)
        {
            var responseModel = new ApiResponseModel<UserRequest>
            {
                IsSuccess = false
            };

            try
            {
                var apiUrl = _configuration["ApiSettings:SendEmailUrl"];

                // Use _userServiceHelper to make the POST request
                var apiResult = await _userServiceHelper.PostAsync<SendEmailModel, ApiResponseModel<ResponseModel>>(apiUrl, model);

                if (!apiResult.IsSuccess)
                {
                    responseModel.Message = apiResult.Message;
                }
                else
                {
                    responseModel.Message = apiResult.Message;
                    responseModel.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging here)
                responseModel.Message = "An error occurred while processing your request.";
            }

            return Json(responseModel);
        }

        /// <summary>
        ///  Developer Name :- Rohit Tiwari
        ///  Message detail :- VerifyOTP() action verify otp status. For till now use in LoginWithOTP and Forgot Password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        #region Rohit Code 
        //[HttpPost]
        //public async Task<IActionResult> VerifyOTP([FromBody] SendEmailModel model)
        //{
        //    var responseModel = new ApiResponseModel<AuthConfigModel>
        //    {
        //        IsSuccess = false
        //    };
        //    try
        //    {
        //        Get user by email.
        //        var templateId = TempData["TemplateType"] as string;
        //        var apiUrl = _configuration["ApiSettings:VerifyOTPUrl"];
        //        var json = JsonConvert.SerializeObject(model);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");
        //        var response = await _httpClient123.PostAsync(apiUrl, content);
        //        var apiResponse = await response.Content.ReadAsStringAsync();
        //        var authResponse = JsonConvert.DeserializeObject<ApiResponseModel<AuthConfigModel>>(apiResponse);

        //        responseModel.Message = authResponse.Message;

        //        if (authResponse.IsSuccess == true)
        //        {
        //            responseModel.IsSuccess = true;
        //            responseModel.Result = authResponse.Result;
        //            responseModel.TemplateType = templateId;
        //            if (templateId == _configuration["EmailTemplateSettings:ForgotPassword"])
        //            {
        //                LoginDTO loginDto = new LoginDTO();
        //                loginDto.Email = model.Email;
        //                string partialHtml = RenderPartialViewToString("_UpdatePassword", loginDto);
        //                responseModel.Data = partialHtml;
        //            }
        //            else
        //            {
        //                responseModel.RedirectUrl = RouteUrlConstants.HomePageUrl;
        //            }
        //            return Json(responseModel);
        //        }
        //        else
        //        {
        //            responseModel.IsSuccess = false;
        //            return Json(responseModel);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log the exception(implement logging here)
        //        responseModel.Message = "An error occurred while processing your request.";
        //    }

        //    return Json(responseModel);
        //}
        [HttpPost]
        public async Task<IActionResult> VerifyOTP([FromBody] SendEmailModel model)
        {
            var responseModel = new ApiResponseModel<AuthConfigModel>
            {
                IsSuccess = false
            };

            try
            {
                // Get user by email.
                var templateId = TempData["TemplateType"] as string;
                var apiUrl = _configuration["ApiSettings:VerifyOTPUrl"];

                // Use _userServiceHelper to make the POST request
                var authResponse = await _userServiceHelper.PostAsync<SendEmailModel, ApiResponseModel<AuthConfigModel>>(apiUrl, model);

                responseModel.Message = authResponse.Message;

                if (authResponse.IsSuccess)
                {
                    responseModel.IsSuccess = true;
                    responseModel.Result = authResponse.Result;
                    responseModel.TemplateType = templateId;

                    if (templateId == _configuration["EmailTemplateSettings:ForgotPassword"])
                    {
                        LoginDTO loginDto = new LoginDTO { Email = model.Email };
                        string partialHtml = RenderPartialViewToString("_UpdatePassword", loginDto);
                        responseModel.Data = partialHtml;
                    }
                    else
                    {
                        responseModel.RedirectUrl = RouteUrlConstants.HomePageUrl;
                    }

                    return Json(responseModel);
                }
                else
                {
                    responseModel.IsSuccess = false;
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
        #endregion

        #region Captcha Code Verification Functionality

        /// <summary>
        ///  Developer Name :- Rohit Tiwari
        ///  Message detail :- GenerateCaptcha() this action is use for genrate OTP image. 
        ///  Created Date   :- 23-04-2024
        ///  Change Date    :- NONE
        ///  Change detail  :- NONE
        /// </summary>
        /// <returns>6 digit OTP code in Image</returns>
        /// 
        public IActionResult GenerateCaptcha()
        {
            var captchaCode = GenerateRandomCodeHelper.GenerateCode();
            var captchaImage = CommonFunctions.GenerateCaptchaImage(captchaCode);

            // Note:- Clear the previous captcha code from the session
            SessionHelper.ClearSession(HttpContext);

            // Store the captcha code in session or another secure place
            var sessionModel = new SessionViewModel
            {
                Captcha = captchaCode,
            };
            SessionHelper.SetSessionObject<SessionViewModel>(HttpContext, "UserSessionData", sessionModel);

            using (var stream = new MemoryStream())
            {
                captchaImage.Save(stream, ImageFormat.Png);
                return File(stream.ToArray(), "image/png");
            }
        }

        #endregion


        #region Partial Views Action

        public IActionResult ForgotPassword()
        {
            var responseModel = new ApiResponseModel<string> { IsSuccess = false };

            try
            {
                LoginDTO loginDTO = new LoginDTO();
                TempData["TemplateType"] = _configuration["EmailTemplateSettings:ForgotPassword"];
                //SetTemplateType("ForgotPassword");
                TempData.Keep("TemplateType");
                // Render the partial view as a string.
                string partialHtml = RenderPartialViewToString("_ForgotPassword", loginDTO);

                responseModel.IsSuccess = true;
                responseModel.Data = partialHtml;
                responseModel.TemplateType = _configuration["EmailTemplateSettings:ForgotPassword"];
                return Json(responseModel);
            }
            catch (Exception)
            {
                responseModel.Message = MessageConstants.TechnicalIssue;
                return Json(responseModel);
            }
        }

        public async Task<IActionResult> OTPVerification(LoginDTO loginDto)
        {
            var responseModel = new ApiResponseModel<string> { IsSuccess = false };

            try
            {
                string partialHtml = RenderPartialViewToString("_OTPVerification", loginDto);
                responseModel.IsSuccess = true;
                responseModel.Data = partialHtml;
                return Json(responseModel);
            }
            catch (Exception)
            {
                responseModel.Message = MessageConstants.TechnicalIssue;
                return Json(responseModel);
            }
        }

        #endregion

        #region Logout

        //public IActionResult Logout()
        //{
        //    // Rohit Tiwari :- Note if need to clear spedific cookie Define cookie key value. 
        //    foreach (var cookie in HttpContext.Request.Cookies.Keys)
        //    {
        //        ////Note:- Below LINE commented By "Harshida(11-02-25)"
        //        ////        Basically it remove all Cookies Which But I want to kept "UserLoginUsername" 
        //        //HttpContext.Response.Cookies.Delete(cookie); 

        //        if (cookie != "UserLoginUsername") // Preserve Remember Me cookie
        //        {
        //            HttpContext.Response.Cookies.Delete(cookie);
        //        }
        //    }
        //    SessionHelper.ClearSession(HttpContext);

        //    // Clear client-side cache
        //    Response.Headers.Append("Cache-Control", "no-store");
        //    Response.Headers.Append("Pragma", "no-cache");

        //    return Json(new { success = true, redirectUrl = Url.Action("LoginPage", "Account"), from = 2 });
        //}
        public async Task<IActionResult> Logout()
        {
            #region            
            HttpContext.Request.Cookies.TryGetValue("CLMSLogin", out var clmsLoginValue);

            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            int userId = int.TryParse(sessionData?.UserId, out var parsedUserId) ? parsedUserId : 0;
            bool shouldClearSession = true;
            if (clmsLoginValue == "1")
            {
                string userTranHisUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.PutLoginHistoryUrl;
                LoginHistory loginHistory = new LoginHistory
                {
                    UserId = userId,
                    LogOutTime = DateTime.Now,
                    IsLoggedOut = true,
                    IpAddress = GetClientIpAddress()
                };
                var result = await _userServiceHelper.PutSingleAsync(userTranHisUrl, loginHistory);
                shouldClearSession = result?.IsSuccess ?? false;
            }
            #endregion
            if (shouldClearSession)
            {
                // 🧹 Clear all cookies except Remember Me
                foreach (var cookie in HttpContext.Request.Cookies.Keys)
                {
                    if (cookie != "UserLoginUsername" && cookie != "CLMSLogin")
                    {
                        HttpContext.Response.Cookies.Delete(cookie);
                    }                  
                } 
            }
            HttpContext.Session.Clear();
           // HttpContext.Response.Cookies.Delete("CLMSLogin");
            HttpContext.Response.Cookies.Delete(".AspNet.Session");
            SessionHelper.ClearSession(HttpContext);

            Response.Headers.Append("Cache-Control", "no-store");
            Response.Headers.Append("Pragma", "no-cache");
            if (clmsLoginValue == "1"){
                // return RedirectToAction("Index", "");
                HttpContext.Response.Cookies.Delete("CLMSLogin");
                return Json(new { success = true, redirectUrl = Url.Action("Index", "CLMSLandingPage"), from = 1 });
                //return Json(new { success = true, redirectUrl = "/CLMSLandingPage/Index", from = 1 });
            }
            else {
                return Json(new { success = true, redirectUrl = Url.Action("LoginPage", "Account"), from = 2 });
            }
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
        #endregion

        #region Create Image for User Initial [Added By Harshida 11-02-'25]
        public IActionResult GenerateProfileImage(string username, int width , int height )
        {
            if (string.IsNullOrEmpty(username))
            {
                return Ok("Username is required.");
            }
            using (Bitmap bitmap = CommonFunctions.GenerateProfileImage(username, width, height))
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin); // Ensure the stream is readable
                    return File(stream.ToArray(), "image/png");
                }
            }
        }
        #endregion

        public IActionResult Access_Denied()
        {
            return View("Access_Denied");
        }
    }
}

