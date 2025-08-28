/****************************************************************************************************
 *  Jira Task Ticket : 328                                                                          *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for UserSettingsApiController entries.                  *
 *  It includes APIs to retrieve, create, update, and delete UserSettings                           *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - PutUserRoleLocation    : Updates an existing User Default Role And Location record.           *
 *                                                                                                  *
 *  Author: Harshida Parmar                                                                         *
 *  Date  : 27-12-'24 '                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using System.Net;
using UserService.BAL.Requests;
using UserService.DAL.Interface;

namespace UserService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserSettingsApiController : ControllerBase
    {
        private readonly IUserSettingsRepository _repository;
        private readonly IUserRepository _repositoryUser;
        public UserSettingsApiController(IUserSettingsRepository repository, IUserRepository repositoryUser)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _repositoryUser = repositoryUser;
        }

        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- Updates UserRoleLocation based on the provided details and ID.
        ///  Created Date   :- 27-12-'24
        ///  Change Date    :- 
        ///  Change detail  :-
        /// </summary>
        /// <param name="id">The ID of the UserRoleLocation to update.</param>
        /// <param name="UserRoleLocation">The UserRoleLocation details to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updateuserrolelocation/{id}")]
        public async Task<IActionResult> PutUserRoleLocation(int id, [FromBody] RoleOrLocationRequest userRoleLocation)
        {
            ApiResponseModel<RoleOrLocationRequest> apiResponse = new ApiResponseModel<RoleOrLocationRequest>();
            // Check if the provided userRoleLocation is null or the id doesn't match the UserId
            if (userRoleLocation == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            var result = await _repository.UpdateUserRoleLocationAsync(DbConstants.UpdateUserDefaultRoleLocation, userRoleLocation);// Call the UpdateUserRoleLocationAsync method in the repository
            if (result.MessageType == 1)
            {
                var (companyDetails, locationDetails, roleDetails) = await _repository.GetUserAdditionalDetailsAsync(DbConstants.GetUserAdditionalDetailsByIdForAuth, userRoleLocation.UserId);
                userRoleLocation.CompanyDetails = companyDetails?.ToList() ?? new List<UserCompanyDetails>();
                userRoleLocation.LocationDetails = locationDetails?.ToList() ?? new List<UserLocationDetails>();
                userRoleLocation.RoleDetails = roleDetails?.ToList() ?? new List<UserRoleDetails>();
            }
            apiResponse.IsSuccess = true;
            apiResponse.Message = ApiResponseMessageConstant.UpdateSuccessfully;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            apiResponse.Data = userRoleLocation; // Return the updated userRoleLocation

            return Ok(apiResponse); // Return the response
         }

        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- Check Email address already exist.
        ///  Created Date   :- 22-01-'25
        ///  Change Date    :- 
        ///  Change detail  :-
        /// </summary> 
        [HttpGet("checkemailexist")]
        public async Task<IActionResult> CheckEmailExist(string email)
        {
            ApiResponseModel<string> apiResponse = new ApiResponseModel<string>();

            // Validate the email
            if (string.IsNullOrEmpty(email))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // Call the CheckUserExistAsync method in the repository
            var (applicationMessage, applicationMessageType) = await _repository.CheckUserExistAsync(DbConstants.CheckEmailExist, email);

            // Set the response based on the result of the email check
            if (applicationMessageType == 1)
            {
                // Email does not exist or no issue
                apiResponse.IsSuccess = true;
                apiResponse.Message = ApiResponseMessageConstant.NoIssues;
                apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            }
            else if (applicationMessageType == 2)
            {
                // Email already exists
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.EmailalreadyConflict;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
            }

            // Set additional response properties if needed
            apiResponse.Data = applicationMessage;

            // Return the response
            return Ok(apiResponse);
        }

        /// <summary>
        /// Developer Name :- Harshida Parmar 
        /// Message Details:- BAsed on UserId,CompanyID and userMapLocation Fetch the Location And Role Details
        /// Created Date:- 07-03-25
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        /// <param name="userMapLocationId"></param>
        /// <returns></returns>
        [HttpGet("getuserrolelocation")]
        public async Task<IActionResult> GetUserRoleLocation(int userId, int? companyId = null, int? userMapLocationId = null)
        {
            ApiResponseModel<UserCompanyRoleLocation> apiResponse = new ApiResponseModel<UserCompanyRoleLocation>();

            if (userId <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid User ID.";
                apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return BadRequest(apiResponse);
            }

            try
            {
                var userDetails = await _repository.GetUserRoleLocationAsync(
                    DbConstants.GetUserUserCompanyRoleLocation, userId, companyId, userMapLocationId);

                if (userDetails == null || (!userDetails.RoleDetails.Any()))
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return NotFound(apiResponse);
                }

                apiResponse.IsSuccess = true;
                apiResponse.Result = userDetails;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

    }
}
