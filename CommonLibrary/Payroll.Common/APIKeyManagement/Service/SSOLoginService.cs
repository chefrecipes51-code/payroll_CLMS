/****************************************************************************************************
 *                                                                                                  *
 *  Author: Harshida Parmar                                                                         *
 *  Date  : 21-MARCH-'25                                                                            *
 *  Jira Task : 650                                                                                    *
 ****************************************************************************************************/

using Payroll.Common.APIKeyManagement.Interface;
using Payroll.Common.APIKeyManagement.Requests;

namespace Payroll.Common.APIKeyManagement.Service
{
    public class SSOLoginService : ISSOLoginService
    {
        private readonly IConfiguration _configuration;

        public SSOLoginService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<CLMSLoginResponse> ValidateCLMSUserAsync(CLMSLoginRequest request)
        {
            // Simulate database check - replace with Dapper or EF DB check
            if (request.UserId == 123 && request.UserName == "harshida.p" && request.UserStatus == "Active")
            {
                // Generate token (simplified example)
                var token = Guid.NewGuid().ToString(); // Replace with JWT

                return new CLMSLoginResponse
                {
                    IsSuccess = true,
                    Message = "Login successful",
                    Token = token,
                    RedirectUrl = $"/CLMSLogin/LoginSSO?token={token}"
                };
            }

            return new CLMSLoginResponse
            {
                IsSuccess = false,
                Message = "Invalid user credentials"
            };
        }
    }
}
