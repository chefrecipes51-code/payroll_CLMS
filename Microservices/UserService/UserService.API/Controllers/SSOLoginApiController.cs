/****************************************************************************************************
 *                                                                                                  *
 *  Author: Harshida Parmar                                                                         *
 *  Date  : 21-March-25                                                                             *
 *  Jira Task :                                                                                     *
 *  Description: API consume by CLMS software for SSO                                               *
 *  ****************************************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using System.Net;
using System.Net.NetworkInformation;
using UserService.BAL.Models;
using UserService.BAL.Requests;
using Payroll.Common.APIKeyManagement.Common;
using UserService.DAL.Interface;
using Payroll.Common.APIKeyManagement.Interface;

namespace UserService.API.Controllers
{
    [Route("api/")]
    //[ApiController]
    public class SSOLoginApiController : ControllerBase
    {
        #region CTOR
        private readonly ISSOLoginService _ssoLoginService;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;
        public SSOLoginApiController(ISSOLoginService ssoLoginService, IUserRepository repository, ApiKeyValidatorHelper apiKeyValidator, IConfiguration configuration)
        {
            _ssoLoginService = ssoLoginService;
            _apiKeyValidatorHelper = apiKeyValidator;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _configuration = configuration;
        }


        //public SSOLoginApiController()
        //{

        //}

        #endregion
        #region SSO Login 
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Created Date   :- 21-March-'25
        ///  Message detail :- Pass Header Key and LoginRequest details and validate the user in PMS Database     
        ///  Last Updated   :- 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("PostCLMSCredentialWithKey")]
        public async Task<IActionResult> PostCLMSCredentialWithKey(
                                [FromHeader(Name = "X-API-KEY")] string apiKey,
                                [FromBody] CLMSLoginRequest request)
        {
            ApiResponseModel<CLMSLoginResponse> apiResponse = new();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            if (request == null || string.IsNullOrWhiteSpace(request.UserName))
            {

                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            /* var userDetails = await _repository.GetByIdAuthAsync(DbConstants.GetUserByIdForAuth, request.UserName);*/ //Cuurent Check with EMAIL
                                                                                                                         // var hashPassword = Payroll.Common.Helpers.GenerateHashKeyHelper.HashKey(loginDto.Password);
            bool.TryParse(request.UserStatus, out bool parsedStatus);
            // if (hashPassword != userAuthResult.Result.Password){}
            LoginHistoryRequestModel obj = new LoginHistoryRequestModel {
                UserId = request.UserId,
                UserName = request.UserName,
                DbStatus= parsedStatus,

            };
            //var userDetails = await _repository.GetUserLoginStatusAsync(DbConstants.GetUserByIdForAuth, obj);
            var userDetails = await _repository.GetUserLoginStatusAsync(DbConstants.SelectUserTransactionHistory, obj);
            //if (userDetails != null)
            if (userDetails==1)
            {
                // var loginUrl = _configuration["ApiSettings:LoginUrl"];
                //var redirectUrl = "https://localhost:7093/SSOHandleLogin/LoginFromCLMS";
                var redirectUrl = _configuration["ApiSettingsForRedirect:LoginReDirectUrl"];
                string htmlForm = $@"
                                        <html>
                                        <body onload='document.forms[0].submit()'>
                                            <form action='{redirectUrl}' method='post'>
                                                <input type='hidden' name='UserId' value='{request.UserId}' />
                                                <input type='hidden' name='UserName' value='{request.UserName}' />
                                                <input type='hidden' name='UserStatus' value='{request.UserStatus}' />
                                            </form>
                                        </body>
                                        </html>";
                return Content(htmlForm, "text/html");
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
        }


        [HttpPost("PostCLMSCredential")]
        public IActionResult PostCLMSCredential()
        {
            var redirectUrl = "https://localhost:7093/SSOHandleLogin/LoginFromCLMS";
            string htmlForm = $@"
                                <html>
                                <body onload='document.forms[0].submit()'>
                                    <form action='{redirectUrl}' method='post'></form>
                                </body>
                                </html>";
            return Content(htmlForm, "text/html");
        }
        #endregion
    }
}
