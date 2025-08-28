using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Interface;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.Helpers;
using System.Collections.Concurrent;
using UserService.BAL.Models;
using UserService.DAL.Interface;

namespace UserService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ApiKeyController : ControllerBase
    {

        private static readonly ConcurrentDictionary<string, ApiKey> ApiKeys = new();
        private readonly IApiKeyService _apiKeyService;
        private readonly IConfiguration _configuration;
        public ApiKeyController(IApiKeyService apiKeyService, IConfiguration configuration)
        {
            _apiKeyService = apiKeyService;
            _configuration = configuration;
        }
        /// <summary>
        /// Generates a new API key for the specified user.
        /// Developed by: [Abhishek Yadav]
        /// Date: [13-01-2025]
        /// </summary>
        /// <param name="rquest">Request object containing user ID, validity duration, and max usage.</param>
        /// <returns>API key and its expiry date.</returns>

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] ApiRquestForm rquest)
        {           
            string decryptedUserName = SingleEncryptionHelper.Decrypt(rquest.userId);
            string decryptedPassword = SingleEncryptionHelper.Decrypt(rquest.Password);

            // Validate user credentials using the service
            //bool isValidUser = await _apiKeyService.ValidateUserCredentialsAsync(rquest.userId, rquest.Password);
            bool isValidUser = await _apiKeyService.ValidateUserCredentialsAsync(decryptedUserName, decryptedPassword);

            if (!isValidUser) 
            {
                return BadRequest("Invalid User Details");
            }
            // Generate API key
            var ApiKeyValue = await _apiKeyService.GenerateApiKeyAsync(decryptedUserName, TimeSpan.FromMinutes(rquest.validityMinutes), rquest.maxUsage);

            return Ok(new { ApiKey = ApiKeyValue, ExpiryDate = DateTime.Now.AddHours(1) });
        }
        /// <summary>
        /// Logs in a user by validating their credentials and generates an API key upon success.
        /// Developed by: [Abhishek Yadav]
        /// Date: [13-01-2025]
        /// </summary>
        /// <param name="loginRequest">Request object containing user ID and password.</param>
        /// <returns>Generated API key and its expiry date.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApiRquestForm loginRequest)
        {
            // Validate user credentials using the service
            bool isValidUser = await _apiKeyService.ValidateUserCredentialsAsync(loginRequest.userId, loginRequest.Password);

            if (!isValidUser)
                return Unauthorized("Invalid username or password.");

            // Generate API key if validation succeeds
            var apiKeyValue = await _apiKeyService.GenerateApiKeyAsync(loginRequest.userId, TimeSpan.FromHours(1), 100);

            return Ok(new { ApiKey = apiKeyValue, ExpiryDate = DateTime.Now.AddHours(1) });
        }
        /// <summary>
        /// Demonstrates a restricted endpoint accessible only with a valid API key.
        /// Date: [16]
        /// </summary>
        /// <param name="apiKey">The API key provided in the header.</param>
        /// <returns>Access result.</returns>

        [HttpPost("restricted")]
        public IActionResult RestrictedEndpoint([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey) || !ApiKeys.TryGetValue(apiKey, out var key))
            {
                return Unauthorized("API Key is invalid.");
            }

            if (key.ExpiryDate < DateTime.Now)
            {
                return Unauthorized("API Key has expired.");
            }

            if (key.UsageCount >= key.MaxUsageLimit)
            {
                return Unauthorized("API Key usage limit exceeded.");
            }

            // Increment usage count
            key.UsageCount++;

            return Ok("You have accessed a restricted endpoint.");
        }

        #region First Time Encrypt the User Id and Password for Generate Key
        /// As Generate Key we have to pass Common User Id and Password which we need to 
        /// put it in appsetting.json. (These User ID and Name is different than "tbl_mst_user")
        /// But before that we have to make it encrypt 
        /// REMEMBER WE ARE NOT MAKING ANY API CALL FOR THESE 
        /// If in future User details change run these API call and pass, UserId and Password 
        /// And statically Store the Encrypted Details in appsetting.json file 
        [HttpPost("EncryptGenerateKeyUser")]
        public IActionResult EncryptGenerateKeyUser(string UserId, string UserPassword)
        {
            if (string.IsNullOrEmpty(UserId))
                return BadRequest("Invalid User ID");

            string encryptedUserId = SingleEncryptionHelper.Encrypt(UserId);
            string encryptedUserPassword = SingleEncryptionHelper.Encrypt(UserPassword);
            return Ok(encryptedUserId + "Hello" + encryptedUserPassword); // USING "HELLO" we can differentiat the UserID and Password
        }
        #endregion

    }
}
