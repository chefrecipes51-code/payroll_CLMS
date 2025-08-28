using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using System.Net;
using UserService.BAL.Models;
using UserService.DAL.Interface;

namespace UserService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MappingUserCompanyApiController : ControllerBase
    {
        #region CTOR And Private Variable
        private readonly IMappingUserCompanyRepository _repository;
        public MappingUserCompanyApiController(IMappingUserCompanyRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #endregion
        #region Mapping User Company Endpoint Handlers (CRUD)
        #region Mapping User Company Fetch All And By ID 
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- This API retrieves all AllUserCompanyMaster details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 11-Oct-2024
        ///  Last Modified  :- 11-Oct-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with  AllUserCompanyMaster details or an appropriate message</returns>
        [HttpGet("getallusercompanymaster")]
        public async Task<IActionResult> GetAllUserCompanyMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<UserCompanyMapModel>>();
            // Fetching data from the repository by executing the stored procedure
            var UserCompanyMaster = await _repository.GetAllAsync(DbConstants.GetMapUserCompanyMaster);
            // Check if data exists
            if (UserCompanyMaster != null && UserCompanyMaster.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = UserCompanyMaster;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            else
            {
                // Handle the case where no data is returned
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
            }
        }
        /// <summary>
        /// Developer Name: Harshida Parmar
        /// Message Detail: API to retrieve  AllUserCompanyMaster details based on the provided UserMapCompany_Id. 
        /// This method fetches data from the repository and returns the tbl_map_usercompany detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 11-Oct-2024
        /// Change Date: 11-Oct-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the tbl_map_usercompany to retrieve</param>
        /// <returns>Returns an API response with  AllUserCompanyMaster details or an error message.</returns>
        [HttpGet("getallusercompanymasterbyid/{id}")]
        public async Task<IActionResult> GetAllUserCompanyMasterById(int id)
        {
            ApiResponseModel<UserCompanyMapModel> apiResponse = new ApiResponseModel<UserCompanyMapModel>();
            var UserCompanyMaster = await _repository.GetByIdAsync(DbConstants.GetMapUserCompanyMasterById, new { UserMapCompany_Id = id });
            if (UserCompanyMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = UserCompanyMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        #endregion
        #region Mapping User Company Add
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- This API handles the addition of  AllUserCompanyMaster details based on the provided organization data.
        ///  Created Date   :- 11-Oct-2024
        ///  Change Date    :- 11-Oct-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="UserCompanyMapModel"> AllUserCompanyMaster detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>       
        [HttpPost("postusercompanymaster")]
        public async Task<IActionResult> PostUserCompanyMaster([FromBody] UserCompanyMapModel mappingusercompany)
        {
            ApiResponseModel<UserCompanyMapModel> apiResponse = new ApiResponseModel<UserCompanyMapModel>();
            if (mappingusercompany == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditMapUserCompanyMaster, mappingusercompany);
            if (mappingusercompany.MessageType == 1)
            {
                apiResponse.IsSuccess = true;
                apiResponse.Message = mappingusercompany.StatusMessage;
                apiResponse.MessageType = mappingusercompany.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.Created;
                return StatusCode((int)HttpStatusCode.Created, apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = mappingusercompany.StatusMessage;
                apiResponse.MessageType = mappingusercompany.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
        }
        #endregion
        #region Mapping User Company Delete
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- Updates the delete column based on the provided UserCompanyMaster and ID. 
        ///                    If the ID does not match or UserCompanyMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 11-Oct-2024
        ///  Last Updated   :- 11-Oct-2024
        ///  Change Details :- Initial implementation.
        /// </summary>
        /// <param name="id">The ID of the UserCompanyMaster to update.</param>
        /// <param name="mappingusercompany">The User Company Master detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deleteusercompanymaster/{id}")]
        public async Task<IActionResult> DeleteUserCompanyMaster(int id, [FromBody] UserCompanyMapModel mappingusercompany)
        {
            ApiResponseModel<UserCompanyMapModel> apiResponse = new ApiResponseModel<UserCompanyMapModel>();
            // Check if the provided mappingUserCompanyDetail is null or the id doesn't match the UserMapCompany_Id
            if (mappingusercompany == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            // Retrieve the existing record based on the id
            var existingMappingUserCompanyDetail = await _repository.GetByIdAsync(DbConstants.GetMapUserCompanyMasterById, new { UserMapCompany_Id = id });
            if (existingMappingUserCompanyDetail == null || existingMappingUserCompanyDetail.IsDeleted == true)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            // Call the UpdateAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteMapUserCompanyMaster, mappingusercompany);
            if (mappingusercompany.MessageType == 1)
            {
                apiResponse.IsSuccess = true;
                apiResponse.Message = mappingusercompany.StatusMessage;
                apiResponse.MessageType = mappingusercompany.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                return StatusCode((int)HttpStatusCode.OK, apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = mappingusercompany.StatusMessage;
                apiResponse.MessageType = mappingusercompany.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
        }
        #endregion
        #region Mapping User Company Update
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- Updates the  User Company detail based on the provided UserMapCompany_Id and ID. 
        ///                    If the ID does not match or tbl_map_usercompany is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 11-Oct-2024
        ///  Last Updated   :- 11-Oct-2024
        ///  Change Details :- Initial implementation.
        /// </summary>
        /// <param name="id">The ID of the  tbl_map_usercompany to update.</param>
        /// <param name="usercompanymaster">The  usercompanymaster detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
       
        [HttpPut("updateusercompanymaster/{id}")]
        public async Task<IActionResult> PutUserCompanyMaster(int id, [FromBody] UserCompanyMapModel mappingusercompany)
        {
            
            ApiResponseModel<UserCompanyMapModel> apiResponse = new ApiResponseModel<UserCompanyMapModel>();
            if (mappingusercompany == null || id != mappingusercompany.UserMapCompany_Id)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            try
            {
                var existingUser = await _repository.GetByIdAsync(DbConstants.GetMapUserCompanyMasterById, new { UserMapCompany_Id = id });
                if (existingUser == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                    return NotFound(apiResponse);
                }
                //mappingusercompany.IsActive=existingUser.IsActive;
                await _repository.UpdateAsync(DbConstants.AddEditMapUserCompanyMaster, mappingusercompany);
                if (mappingusercompany.MessageType == 1)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Message = mappingusercompany.StatusMessage;
                    apiResponse.MessageType = mappingusercompany.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.Created;
                    return StatusCode((int)HttpStatusCode.Created, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = mappingusercompany.StatusMessage;
                    apiResponse.MessageType = mappingusercompany.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return BadRequest(apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion
        #endregion
    }
}
