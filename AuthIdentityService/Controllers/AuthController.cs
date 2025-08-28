using UserService.BAL.Requests;
using Microsoft.AspNetCore.Mvc;
using UserService.DAL.Interface;
using AuthIdentityService.Helpers;
using Payroll.Common.CommonRequest;
using Payroll.Common.ApplicationModel;
using Payroll.Common.ApplicationConstant;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _repository;

    public AuthController(IConfiguration config, IUserRepository repository)
    {
        _configuration = config;
        _repository = repository;
    }

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] LoginRequest model)
    {
        var apiResponse = new ApiResponseModel<UserRequest> { IsSuccess = false };

        if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
        {
            apiResponse.Message = MessageConstants.InvalidLoginRequest;
            return BadRequest(apiResponse);
        }

        var userDetails = await _repository.GetByIdAsync(DbConstants.GetUserByIdForAuth, model.Username);
        #region Added By Harshida 23-12-'24
        //Task for getting UserAdditionalDetails :- Start

        var (companyDetails, locationDetails, roleDetails) = await _repository.GetUserAdditionalDetailsAsync(DbConstants.GetUserAdditionalDetailsByIdForAuth, userDetails.UserId);

        if (userDetails != null)
        {
            userDetails.CompanyDetails = companyDetails?.ToList() ?? new List<UserCompanyDetails>();
            userDetails.LocationDetails = locationDetails?.ToList() ?? new List<UserLocationDetails>();
            userDetails.RoleDetails = roleDetails?.ToList() ?? new List<UserRoleDetails>();

            if (userDetails.CompanyDetails.Count == 0 || userDetails.LocationDetails.Count == 0 || userDetails.RoleDetails.Count == 0)
            {
                apiResponse.Message = "One or more user additional details are missing.";
                apiResponse.IsSuccess = false;
                return BadRequest(apiResponse);
            }
        }
        //Task for getting UserAdditionalDetails :- End        
        #endregion
        if (userDetails == null)
        {
            apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
            return NotFound(apiResponse);
        }

        if (userDetails.LockAccount)
        {
            int lockDurationMinutes = _configuration.GetValue<int>("ApplicationUserSettings:UserAccountLockTime");
            if ((userDetails.UserCurrentDateTime - Convert.ToDateTime(userDetails.LockTime)).TotalMinutes > lockDurationMinutes)
            {
                userDetails.LockAccount = false;
                userDetails.MaxAttempts = _configuration.GetValue<int>("Authentication:Attempts");
                await _repository.UpdateLoginActivityAsync(DbConstants.UpdateUserLoginActivity, userDetails);
            }
            else
            {
                apiResponse.Message = MessageConstants.AccountInactiveTemporary;
                return Unauthorized(apiResponse);
            }
        }

        var hashPassword = Payroll.Common.Helpers.GenerateHashKeyHelper.HashKey(model.Password);
        if (hashPassword != userDetails.Password)
        {
            userDetails.MaxAttempts--;
            userDetails.LockAccount = userDetails.MaxAttempts <= 0;

            await _repository.UpdateLoginActivityAsync(DbConstants.UpdateUserLoginActivity, userDetails);
            apiResponse.Message = userDetails.LockAccount? MessageConstants.AccountInactiveTemporary: userDetails.MaxAttempts == 1
                                  ? MessageConstants.PasswordInfoMessage: MessageConstants.InvalidLoginRequest;

            return Unauthorized(apiResponse);   
        }

        userDetails.MaxAttempts = _configuration.GetValue<int>("Authentication:Attempts");
        await _repository.UpdateLoginActivityAsync(DbConstants.UpdateUserLoginActivity, userDetails);

        var token = JwtTokenGeneratorHelper.GetToken(userDetails.Email,new List<string> { userDetails.RoleName },_configuration);

        if (userDetails.NextPasswordChangeDate.HasValue) 
        {
            DateTime reminderStartDate = userDetails.NextPasswordChangeDate.Value.AddDays(-userDetails.PasswordExpiryReminderDays);
            DateTime currentDateTime = userDetails.UserCurrentDateTime;

            if (reminderStartDate <= userDetails.NextPasswordChangeDate)
            {
                apiResponse.IsSuccess = true;
                apiResponse.Message = $"Please change your password before the date {userDetails.NextPasswordChangeDate:yyyy-MM-dd}";

                if (userDetails.NextPasswordChangeDate == currentDateTime)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Message = "Today is the last day to change your password. Please change it.";
                    return Ok(apiResponse);
                }
                else if (userDetails.NextPasswordChangeDate < currentDateTime)
                {
                    userDetails.LockAccount = true;
                    await _repository.UpdateLoginActivityAsync(DbConstants.UpdateUserLoginActivity, userDetails);
                    apiResponse.Message = MessageConstants.AccountBlockedMessage;
                    return BadRequest(apiResponse);
                }
            }
        }
      
        apiResponse.Token = token;
        apiResponse.Result = userDetails;
        apiResponse.IsSuccess = true;
        return Ok(apiResponse);
    }

}
