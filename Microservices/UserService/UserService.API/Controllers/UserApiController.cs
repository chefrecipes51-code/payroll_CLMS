using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using System.Net;
using Payroll.Common.CommonDto;
using UserService.DAL.Interface;
using UserService.BAL.Requests;
using Payroll.Common.Helpers;
using UserService.BAL.Models;
using System.Collections.Generic;


namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/UserApi/")]
    public class UserApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repository;

        public UserApiController(IConfiguration config, IUserRepository repository)
        {
            _configuration = config;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #region User Core Functionality

        [HttpGet("getusers")]
        public async Task<IActionResult> GetAllUser()
        {
            ApiResponseModel<IEnumerable<UserRequest>> apiResponse = new ApiResponseModel<IEnumerable<UserRequest>>();
            try
            {
                var users = await _repository.GetAllAsync(DbConstants.GetUsers);
                if (users != null)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = users;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = MessageConstants.UserNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return NotFound(apiResponse);
                }
            }
            catch
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpGet("getuserslist")]
        public async Task<IActionResult> GetUsersList()
        {
            ApiResponseModel<IEnumerable<UserListModel>> apiResponse = new ApiResponseModel<IEnumerable<UserListModel>>();
            try
            {
                var users = await _repository.GetUsersListAsync(DbConstants.GetUsersList);
                if (users != null)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = users;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = MessageConstants.UserNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return NotFound(apiResponse);
                }
            }
            catch
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpGet("getlocationwiseuserslist")]
        public async Task<IActionResult> GetLocationwiseUsersList(int? Company_Id, int? Location_Id)
        {
            ApiResponseModel<IEnumerable<UserListModel>> apiResponse = new ApiResponseModel<IEnumerable<UserListModel>>();
            try
            {

                var users = await _repository.GetLocationwiseUsersListAsync(DbConstants.GetLocationwiseUsersList, Company_Id, Location_Id);
                if (users != null)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = users;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = MessageConstants.UserNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return NotFound(apiResponse);
                }
            }
            catch
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
            try
            {
                var user = await _repository.GetByIdAsync(DbConstants.GetUserById, id);
                if (user == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = user;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return StatusCode((int)HttpStatusCode.OK, apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        //chirag gurjar payroll-359 5jan2025 
        //added GetByIdAuth as moved auth module here
        //[HttpGet("getbyidauth/{id}")]
        //public async Task<IActionResult> GetByIdAuth(string id)
        //{
        //    ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
        //    try
        //    {
        //        var userDetails = await _repository.GetByIdAuthAsync(DbConstants.GetUserByIdForAuth, id);

        //        if (userDetails != null)
        //        {
        //            var (companyDetails, locationDetails, roleDetails) = await _repository.GetUserAdditionalDetailsAsync(DbConstants.GetUserAdditionalDetailsByIdForAuth, userDetails.UserId);

        //            userDetails.CompanyDetails = companyDetails?.ToList() ?? new List<UserCompanyDetails>();
        //            userDetails.LocationDetails = locationDetails?.ToList() ?? new List<UserLocationDetails>();
        //            userDetails.RoleDetails = roleDetails?.ToList() ?? new List<UserRoleDetails>();

        //            if (userDetails.CompanyDetails.Count == 0 || userDetails.LocationDetails.Count == 0 || userDetails.RoleDetails.Count == 0)
        //            {
        //                apiResponse.Message = "One or more user additional details are missing.";
        //                apiResponse.IsSuccess = false;
        //                return Ok(apiResponse);
        //            }
        //        }

        //        if (userDetails == null)
        //        {
        //            apiResponse.IsSuccess = false;
        //            apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
        //            apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
        //            return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
        //        }
        //        else
        //        {
        //            apiResponse.IsSuccess = true;
        //            apiResponse.Result = userDetails;
        //            apiResponse.StatusCode = (int)HttpStatusCode.OK;
        //            return StatusCode((int)HttpStatusCode.OK, apiResponse);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResponse.IsSuccess = false;
        //        apiResponse.Message = MessageConstants.TechnicalIssue;
        //        apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
        //        return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
        //    }
        //}

        [HttpGet("getbyidauth")]
        public async Task<IActionResult> GetByIdAuth([FromQuery] string id, [FromQuery] string isClmsUser)
        {
            ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
            try
            {
                var userDetails = await _repository.GetByIdAuthAsync(DbConstants.GetUserByIdForAuth, id, isClmsUser);//isClmsUser Added 03-04-25 

                if (userDetails != null)
                {
                    var (companyDetails, locationDetails, roleDetails) = await _repository.GetUserAdditionalDetailsAsync(DbConstants.GetUserAdditionalDetailsByIdForAuth, userDetails.UserId);

                    userDetails.CompanyDetails = companyDetails?.ToList() ?? new List<UserCompanyDetails>();
                    userDetails.LocationDetails = locationDetails?.ToList() ?? new List<UserLocationDetails>();
                    userDetails.RoleDetails = roleDetails?.ToList() ?? new List<UserRoleDetails>();

                    if (userDetails.CompanyDetails.Count == 0 || userDetails.LocationDetails.Count == 0 || userDetails.RoleDetails.Count == 0)
                    {
                        apiResponse.Message = "One or more user additional details are missing.";
                        apiResponse.IsSuccess = false;
                        return Ok(apiResponse);
                    }
                }

                if (userDetails == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = userDetails;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return StatusCode((int)HttpStatusCode.OK, apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        [HttpPost("adduser")]
        public async Task<IActionResult> PostUserDetails([FromBody] UserRequest model)
        {
            ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
            if (model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return StatusCode((int)HttpStatusCode.BadRequest, apiResponse);
            }

            try
            {
                var randomeCode = GenerateRandomCodeHelper.GenerateCode();
                model.ActivationCode = GenerateHashKeyHelper.HashKey(randomeCode);
                var baseUrl = _configuration.GetValue<string>("ApiSettings:WebApplicationBaseUrl");
                //model.ActivationLink = $"https:/{baseUrl}/User/VerifyResetPasswordLink?encryptedCode={model.ActivationCode}";
                model.ActivationLink = $"{baseUrl}/User/VerifyResetPasswordLink?encryptedCode={model.ActivationCode}";
                var result = await _repository.AddAsync(DbConstants.AddEditUser, model);
                if (result != null && !string.IsNullOrEmpty(result.StatusMessage))
                {

                    model.UserId = result.UserId; //Assign UserID to Model (Added By Harshida 13-01-'25)

                    SendEmailModel sendEmail = new SendEmailModel();
                    sendEmail.Email = model.Email;
                    sendEmail.TemplateType = MessageConstants.AccountActivationTemplateType;
                    await _repository.SendEmailAsync(DbConstants.SendEmail, sendEmail);
                }
                apiResponse.IsSuccess = true;
                apiResponse.Result = model;
                apiResponse.Message = model.StatusMessage;
                apiResponse.StatusCode = (int)HttpStatusCode.Created;
                return StatusCode((int)HttpStatusCode.Created, apiResponse);
            }
            catch
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpPut("edituser")]
        public async Task<IActionResult> PutUserDetails([FromBody] UserRequest model)
        {
            ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
            if (model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return StatusCode((int)HttpStatusCode.BadRequest, apiResponse);
            }

            try
            {
                //var UserId = Convert.ToInt32(model.UserId);
                var existingUser = await _repository.GetByIdAsync(DbConstants.GetUserById, model.UserId);
                if (existingUser == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
                }

                var result = await _repository.UpdateAsync(DbConstants.AddEditUser, model);
                apiResponse.IsSuccess = true;
                apiResponse.Message = result.ApplicationMessage;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return StatusCode((int)HttpStatusCode.OK, apiResponse);
            }
            catch
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpPut("deactivateuser")]
        public async Task<IActionResult> DeactivateUserDetails([FromBody] DeactivateUser model)
        {
            var apiResponse = new ApiResponseModel<DeactivateUser>();

            if (model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return StatusCode(apiResponse.StatusCode, apiResponse);
            }

            try
            {
                var result = await _repository.UpdateDeactiveUserStatusAsync(DbConstants.UpdateDeactivateUsers, model);

                if (result == null || result.MessageType == 3) // 3 = Error type
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = result?.ApplicationMessage ?? ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode(apiResponse.StatusCode, apiResponse);
                }

                apiResponse.IsSuccess = true;
                apiResponse.Message = result.ApplicationMessage;
                apiResponse.Data = result;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;

                return StatusCode(apiResponse.StatusCode, apiResponse);
            }
            catch (Exception ex)
            {
                // Optionally log the error
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode(apiResponse.StatusCode, apiResponse);
            }
        }


        // Added by Abhishek 18-02-25 Task:484
        [HttpPost("updateuserpwd")]
        public async Task<IActionResult> PutchangeUserPwdDetails([FromBody] UserRequest model)
        {
            ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
            if (model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return StatusCode((int)HttpStatusCode.BadRequest, apiResponse);
            }

            try
            {
                //var UserId = Convert.ToInt32(model.UserId);
                var existingUser = await _repository.GetByIdAsync(DbConstants.GetUserById, model.UserId);
                if (existingUser == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
                }

                var result = await _repository.ChangeUserPasswordAsync(DbConstants.ChangeUserPassword, model);
                apiResponse.Message = result.ApplicationMessage;

                if (result.MessageType != 1)
                {
                    apiResponse.IsSuccess = false;
                    return StatusCode((int)HttpStatusCode.OK, apiResponse);
                }
                apiResponse.IsSuccess = true;
                return StatusCode((int)HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #region CODE NOT USED 
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] UserRequest model)
        {
            ApiResponseModel<bool> apiResponse = new ApiResponseModel<bool>();
            try
            {
                // Attempt to delete the user using the repository
                model.UpdatedBy = 1; // comes from session userid as updateby.
                var result = await _repository.DeleteAsync(DbConstants.DeleteUserDetail, model);

                if (result == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Message = result.ApplicationMessage;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return StatusCode((int)HttpStatusCode.OK, apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion

        [HttpGet("getusermapdetailsbyid/{User_Id}")]
        public async Task<IActionResult> GetUserMapDetailsById(string User_Id)
        {
            ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
            try
            {
                var user = await _repository.GetUserMapDetailsByIdAsync(DbConstants.GetUserMapDetailsById, User_Id);
                if (user == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = user;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return StatusCode((int)HttpStatusCode.OK, apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpGet("geteditusermapdetailsbyid/{User_Id}")]
        public async Task<IActionResult> GetEditUserMapDetailsById(int User_Id)
        {
            ApiResponseModel<UserMapDetailModel> apiResponse = new ApiResponseModel<UserMapDetailModel>();
            try
            {
                var user = await _repository.GetEditUserMapDetailsByIdAsync(DbConstants.GetEditUserMapDetailsById, User_Id);
                if (user == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = user;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return StatusCode((int)HttpStatusCode.OK, apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpGet("getedituserlocationwiserole/")]
        public async Task<IActionResult> GetEditUserLocationWiseRole(int userId, int companyId, int? correspondanceId)
        {
            var apiResponse = new ApiResponseModel<object>(); // Use object to handle multiple result sets

            try
            {
                var (locationWiseRoles, roleMenuHeaders) = await _repository.GetEditUserLocationWiseRole(
                    DbConstants.GetEditUserLocationWiseRole, userId, companyId, correspondanceId
                );

                if (!locationWiseRoles.Any() && !roleMenuHeaders.Any())
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
                }

                apiResponse.IsSuccess = true;
                apiResponse.Result = new
                {
                    LocationWiseRoles = locationWiseRoles,
                    RoleMenuHeaders = roleMenuHeaders
                };
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return StatusCode((int)HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of UserRoleStatusMaster details based on the provided organization data.
        ///  Created Date   :- 17-Feb-2025
        ///  Change Date    :- 17-Feb-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="updateUserRoleStatus"> UserRoleStatusMaster detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPut("updateuserrolestatusmaster")]
        public async Task<IActionResult> UpdateUserRoleStatusMaster([FromBody] UpdateUserRoleStatusModel updateUserRoleStatus)
        {
            ApiResponseModel<UpdateUserRoleStatusModel> apiResponse = new ApiResponseModel<UpdateUserRoleStatusModel>();
            if (updateUserRoleStatus == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.UpdateUserRoleStatusAsync(DbConstants.UpdateUserRoleStatus, updateUserRoleStatus);
            apiResponse.IsSuccess = updateUserRoleStatus.MessageType == 1;
            apiResponse.Message = updateUserRoleStatus.StatusMessage;
            apiResponse.MessageType = updateUserRoleStatus.MessageType;
            apiResponse.StatusCode = updateUserRoleStatus.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return updateUserRoleStatus.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);

        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of UserLocationStatusMaster details based on the provided organization data.
        ///  Created Date   :- 17-Feb-2025
        ///  Change Date    :- 17-Feb-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="updateUserRoleStatus"> UserLocationStatusMaster detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPut("updateuserlocationstatusmaster")]
        public async Task<IActionResult> UpdateUserLocationStatusMaster([FromBody] UpdateUserLocationStatusModel updateUserLocationStatus)
        {
            ApiResponseModel<UpdateUserLocationStatusModel> apiResponse = new ApiResponseModel<UpdateUserLocationStatusModel>();
            if (updateUserLocationStatus == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.UpdateUserLocationStatusAsync(DbConstants.UpdateUserLocationStatus, updateUserLocationStatus);
            apiResponse.IsSuccess = updateUserLocationStatus.MessageType == 1;
            apiResponse.Message = updateUserLocationStatus.StatusMessage;
            apiResponse.MessageType = updateUserLocationStatus.MessageType;
            apiResponse.StatusCode = updateUserLocationStatus.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return updateUserLocationStatus.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);

        }


        #region Added By Harshida   
        /// <summary>
        /// Developer Name:-Harshida Parmar
        /// </summary>
        /// <param name="loginHistory"></param>
        /// <returns></returns>
        [HttpPost("postloginhistory")]
        public async Task<IActionResult> PostLoginHistory([FromBody] LoginHistory loginHistory)
        {
            ApiResponseModel<LoginHistory> apiResponse = new ApiResponseModel<LoginHistory>();

            if (loginHistory == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = 400;
                return BadRequest(apiResponse);
            }
            var result = await _repository.AddLoginHistoryAsync(DbConstants.AddUserTransactionHistory, loginHistory);

            apiResponse.IsSuccess = result.MessageType == 1;
            apiResponse.Message = result.StatusMessage;
            apiResponse.MessageType = result.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? 201 : 400;
            apiResponse.Result = result;

            return apiResponse.IsSuccess
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : BadRequest(apiResponse);
        }

        /// <summary>
        /// Developer Name:-Harshida Parmar
        /// </summary>
        /// <param name="loginHistory"></param>
        /// <returns></returns>
        [HttpPut("putloginhistory")]
        public async Task<IActionResult> PutLoginHistory([FromBody] LoginHistory loginHistory)
        {
            ApiResponseModel<LoginHistory> apiResponse = new ApiResponseModel<LoginHistory>();

            if (loginHistory == null || loginHistory.UserId <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = 400;
                return BadRequest(apiResponse);
            }
            var result = await _repository.UpdateLoginHistoryAsync(DbConstants.AddUserTransactionHistory, loginHistory);
            apiResponse.IsSuccess = result.MessageType == 1;
            apiResponse.Message = result.StatusMessage;
            apiResponse.MessageType = result.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? 200 : 400;
            apiResponse.Result = result;

            return apiResponse.IsSuccess
                ? Ok(apiResponse)
                : BadRequest(apiResponse);
        }

        [HttpPost("check-login-status")]
        public async Task<IActionResult> CheckLoginStatus([FromBody] LoginHistoryRequestModel requestModel)
        {
            ApiResponseModel<int> apiResponse = new ApiResponseModel<int>();

            if (requestModel == null || requestModel.UserId <= 0 || string.IsNullOrWhiteSpace(requestModel.UserName))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = 400;
                return BadRequest(apiResponse);
            }

            try
            {
                int userExistCount = await _repository.GetUserLoginStatusAsync(DbConstants.SelectUserTransactionHistory, requestModel);

                apiResponse.IsSuccess = true;
                apiResponse.StatusCode = 200;
                apiResponse.Result = userExistCount;
                apiResponse.Message = "User login history checked successfully.";
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.StatusCode = 500;
                apiResponse.Message = $"An error occurred: {ex.Message}";
            }

            return apiResponse.IsSuccess
                ? Ok(apiResponse)
                : StatusCode(apiResponse.StatusCode, apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- Delete/ Update the User IsDelete True or false.
        ///  Created Date   :- 01-01-2025
        ///  Last Updated   :-
        ///  Change Details :-
        /// </summary>      
        [HttpPut("deleteUserById/{id}")]
        public async Task<IActionResult> DeleteUserById(int id, [FromBody] UserRequest userInfo)
        {
            ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
            // Check if the provided User is null or the id doesn't match the User_Id
            if (id <= 0 || userInfo == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }
            // Call the DeleteAsync method in the repository
            await _repository.DeleteUserByIDAsync(DbConstants.DeleteUserDetail, userInfo);
            apiResponse.IsSuccess = userInfo.MessageType == 1;
            apiResponse.Message = userInfo.StatusMessage;
            apiResponse.MessageType = userInfo.MessageType;
            apiResponse.StatusCode = userInfo.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return userInfo.MessageType == 1 ? StatusCode((int)HttpStatusCode.OK, apiResponse) : Ok(apiResponse);
        }
        #endregion
        #endregion

        #region Email Notification

        [HttpPost("sendemail")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailModel model)
        {
            ApiResponseModel<object> apiResponse = new ApiResponseModel<object>();
            try
            {
                if (model == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.InvalidRequest;
                }
                else
                {
                    ResponseModel response = await _repository.SendEmailAsync(DbConstants.SendEmail, model);
                    if (response.ApplicationMessageType == 1)
                    {
                        apiResponse.IsSuccess = true;
                        apiResponse.Message = response.ApplicationMessage;
                        apiResponse.StatusCode = (int)HttpStatusCode.OK;
                        return Ok(apiResponse);
                    }
                    else
                    {
                        apiResponse.IsSuccess = false;
                        apiResponse.Message = response.ApplicationMessage;
                        apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Ok(apiResponse);
                    }
                }
                return StatusCode((int)HttpStatusCode.BadRequest, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpPost("verifyotp")]
        public async Task<IActionResult> VerifyOTP([FromBody] SendEmailModel model)
        {
            ApiResponseModel<object> apiResponse = new ApiResponseModel<object>();
            if (model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return Ok(apiResponse);
            }

            try
            {
                var data = await _repository.VerifyOTPAsync(DbConstants.CheckOTPIsValid, model);
                if (data.OTPStatus == 3 || data.OTPStatus == 2)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = data.ApplicationMessage;
                    return StatusCode((int)HttpStatusCode.Unauthorized, apiResponse);
                }
                apiResponse.IsSuccess = true;
                apiResponse.Result = data;
                apiResponse.Message = data.ApplicationMessage;
                return StatusCode((int)HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        #endregion

        #region User Update Password

        [HttpPost("updateuserpassword")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] SendEmailModel model)
        {
            ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
            if (model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return Ok(apiResponse);
            }

            try
            {
                var result = await _repository.UpdateUserPasswordAsync(DbConstants.UpdateUserPassword, model);
                apiResponse.Message = result.ApplicationMessage;

                if (result.IsSuccessType == 0 || result.IsSuccessType == 2)
                {
                    apiResponse.IsSuccess = false;
                    return StatusCode((int)HttpStatusCode.BadRequest, apiResponse);
                }
                apiResponse.IsSuccess = true;
                return StatusCode((int)HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpGet("verifyresetpasswordlink")]
        public async Task<IActionResult> VerifyResetPasswordLink([FromQuery] string encryptedCode)
        {
            ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
            try
            {
                // encryptedCode -> ActivationCode
                // Rohit Tiwari Note :- I am using the GetByIdAsync method as a common approach to check the activation code provided by the user, where IsLinkActivate is set to false.
                var userDetails = await _repository.GetByIdAsync(DbConstants.GetUserById, encryptedCode); // encryptedCode is pass to Id parameter.
                if (userDetails != null)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = userDetails;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.StatusCode = (int)HttpStatusCode.NoContent;
                    return Ok(apiResponse);
                }
            }
            catch
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        #endregion

        #region User account status managment

        [HttpPost("updateuseraccountstatus")]
        public async Task<IActionResult> UpdateUserAccountStatus([FromBody] UserRequest model)
        {
            ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
            if (model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return Ok(apiResponse);
            }

            try
            {
                var result = await _repository.UpdateUserAccountStatus(DbConstants.UpdateUserAccountStatus, model);
                apiResponse.Message = result.ApplicationMessage;

                if (result.MessageType != 1)
                {
                    apiResponse.IsSuccess = false;
                    return StatusCode((int)HttpStatusCode.BadRequest, apiResponse);
                }
                apiResponse.IsSuccess = true;
                return StatusCode((int)HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpPost("updateuseractivedeactivestatus")]
        public async Task<IActionResult> UpdateUserActiveDeactiveStatus([FromBody] int serviceApproveRejectId)
        {
            ApiResponseModel<UserRequest> apiResponse = new ApiResponseModel<UserRequest>();
            if (serviceApproveRejectId <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return Ok(apiResponse);
            }

            try
            {
                var result = await _repository.UpdateUserActiveDeactiveStatus(DbConstants.UpdateUserActiveDeactiveStatus, serviceApproveRejectId);
                apiResponse.Message = result.ApplicationMessage;

                if (result.MessageType != 1)
                {
                    apiResponse.IsSuccess = false;
                    return StatusCode((int)HttpStatusCode.BadRequest, apiResponse);
                }
                apiResponse.IsSuccess = true;
                return StatusCode((int)HttpStatusCode.OK, apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        #endregion

        #region Added By Harshida(17-01-25) [Add New User Role Menu Details] 
        [HttpPost("AddOrUpdateUserRoleMenu")]
        public async Task<IActionResult> AddOrUpdateUserRoleMenu([FromBody] AddUpdateUserRoleMenuRequest request)
        {
            ApiResponseModel<AddUpdateUserRoleMenuRequest> apiResponse = new ApiResponseModel<AddUpdateUserRoleMenuRequest>();

            if (request == null || request.UserRoleMenuDetails == null || request.UserRoleMenuDetails.Count == 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return StatusCode((int)HttpStatusCode.BadRequest, apiResponse);
            }
            var result = await _repository.AddOrUpdateUserRoleMenuAsync(DbConstants.AddUserUserRoleBasedMenu, request);
            apiResponse.IsSuccess = true;
            apiResponse.Result = request;
            apiResponse.Message = "Record Created Successfully";
            apiResponse.StatusCode = (int)HttpStatusCode.Created;
            return StatusCode((int)HttpStatusCode.Created, apiResponse);
        }
        #endregion
    }
}
