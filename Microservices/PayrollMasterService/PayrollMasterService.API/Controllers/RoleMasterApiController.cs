/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-137, Alteration -> PAYROLL-173                                       *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for RoleMaster entries.                                 *
 *  It includes APIs to retrieve, create, update, and delete RoleMaster                             *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllRoleMaster : Retrieves all RoleMaster records.                                          *
 *  - GetRoleMasterById: Retrieves a specific RoleMaster record by ID.                              *
 *  - PostRoleMaster   : Adds a new RoleMaster record.                                              *
 *  - PutRoleMaster    : Updates an existing RoleMaster record.                                     *
 *  - DeleteRoleMaster : Soft-deletes an RoleMaster record.                                         *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 24-Oct-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class RoleMasterApiController : ControllerBase
    {
        private readonly IRoleMasterRepository _repository;
        public RoleMasterApiController(IRoleMasterRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #region Role Master Crud APIs Functionality
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all Role details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 24-Oct-2024
        ///  Last Modified  :- 24-Oct-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Role details or an appropriate message</returns>
        [HttpGet("getallrolemaster")]
        public async Task<IActionResult> GetAllRoleMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<RoleMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var roleMasters = await _repository.GetAllAsync(DbConstants.GetRoleMaster);

            // Check if data exists
            if (roleMasters != null && roleMasters.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = roleMasters;
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
        /// Developer Name :- Priyanshi Jain
        /// Message Detail :- API to retrieve Role details based on the provided  Role ID. 
        ///                   This method fetches data from the repository and returns the role detail if found, 
        ///                   otherwise returns a not found response.
        /// Created Date   :- 24-Oct-2024
        /// Change Date    :- 24-Oct-2024
        /// Change Detail  :- No changes yet
        /// </summary>
        /// <param name="id">The ID of the role to retrieve</param>
        /// <returns>Returns an API response with role details or an error message.</returns>
        [HttpGet("getrolemasterbyid/{id}")]
        public async Task<IActionResult> GetRoleMasterById(int id)
        {
            ApiResponseModel<RoleMaster> apiResponse = new ApiResponseModel<RoleMaster>();
            var roleMaster = await _repository.GetByIdAsync(DbConstants.GetRoleMasterById, new { Role_Id = id });
            if (roleMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = roleMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of role details based on the provided organization data.
        ///  Created Date   :- 24-Oct-2024
        ///  Change Date    :- 24-Oct-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="roleMaster"> role detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postrolemaster")]
        public async Task<IActionResult> PostRoleMaster([FromBody] RoleMaster roleMaster)
        {
            ApiResponseModel<RoleMaster> apiResponse = new ApiResponseModel<RoleMaster>();
            if (roleMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditRoleMaster, roleMaster);
            apiResponse.IsSuccess = roleMaster.MessageType == 1;
            apiResponse.Message = roleMaster.StatusMessage;
            apiResponse.MessageType = roleMaster.MessageType;
            apiResponse.StatusCode = roleMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return roleMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- Updates the role detail based on the provided roleMaster and ID. 
        ///                    If the ID does not match or roleMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 24-Oct-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the role to update.</param>
        /// <param name="roleMaster">The role detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updaterolemaster/{id}")]
        public async Task<IActionResult> PutRoleMaster(int id, [FromBody] RoleMaster roleMaster)
        {
            ApiResponseModel<RoleMaster> apiResponse = new ApiResponseModel<RoleMaster>();
            // Check if the provided roleMaster is null or the id doesn't match the Role_Id
            if (roleMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            roleMaster.Role_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditRoleMaster, roleMaster);
            apiResponse.IsSuccess = roleMaster.MessageType == 1;
            apiResponse.Message = roleMaster.StatusMessage;
            apiResponse.MessageType = roleMaster.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- Updates the delete column based on the provided roleMaster and ID. 
        ///                    If the ID does not match or roleMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 24-Oct-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the  Role to update.</param>
        /// <param name="roleMaster">The Role detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deleterolemaster/{id}")]
        public async Task<IActionResult> DeleteRoleMaster(int id, [FromBody] RoleMaster roleMaster)
        {
            ApiResponseModel<RoleMaster> apiResponse = new ApiResponseModel<RoleMaster>();
            // Check if the provided Role is null or the id doesn't match the Role_Id
            if (roleMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            roleMaster.Role_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteRoleMaster, roleMaster);
            apiResponse.IsSuccess = roleMaster.MessageType == 1;
            apiResponse.Message = roleMaster.StatusMessage;
            apiResponse.MessageType = roleMaster.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
    }
}
