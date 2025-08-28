/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-168                                                                  *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for MapDepartmentRole entries.                          *
 *  It includes APIs to retrieve, create, update, and delete MapDepartmentRole                      *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllMapDepartmentRole : Retrieves all MapDepartmentRole records.                            *
 *  - GetMapDepartmentRoleById: Retrieves a specific MapDepartmentRole record by ID.                *
 *  - PostMapDepartmentRole   : Adds a new MapDepartmentRole record.                                *
 *  - PutMapDepartmentRole    : Updates an existing MapDepartmentRole record.                       *
 *  - DeleteMapDepartmentRole : Soft-deletes an MapDepartmentRole record.                           *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 08-Nov-2024                                                                             *
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
    public class MapDepartmentRoleApiController : ControllerBase
    {
        private readonly IMapDepartmentRoleRepository _repository;
        public MapDepartmentRoleApiController(IMapDepartmentRoleRepository repository)
        {
            _repository = repository;
        }
        #region Map Department Role Crud APIs Functionality
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all Map Department Role details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 08-Nov-2024
        ///  Last Modified  :- 08-Nov-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Map Department Role details or an appropriate message</returns>
        [HttpGet("getallmapdepartmentrole")]
        public async Task<IActionResult> GetAllMapDepartmentRole()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<MapDepartmentRole>>();
            // Fetching data from the repository by executing the stored procedure
            var mapDepartmentRole = await _repository.GetAllAsync(DbConstants.GetMapDepartmentRole);
            // Check if data exists
            if (mapDepartmentRole != null && mapDepartmentRole.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = mapDepartmentRole;
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
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve Map Department Role details based on the provided  Department Role ID. 
        /// This method fetches data from the repository and returns the  Map Department Role detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 08-Nov-2024
        /// Change Date:  08-Nov-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the Map Department Role to retrieve</param>
        /// <returns>Returns an API response with Map Department Role details or an error message.</returns>
        [HttpGet("getmapdepartmentrolebyid/{id}")]
        public async Task<IActionResult> GetMapDepartmentRoleById(int id)
        {
            ApiResponseModel<MapDepartmentRole> apiResponse = new ApiResponseModel<MapDepartmentRole>();
            var mapDepartmentRole = await _repository.GetByIdAsync(DbConstants.GetMapDepartmentRoleById, new { RoleDepartment_Id = id });
            if (mapDepartmentRole == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = mapDepartmentRole;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of Map Department Role details based on the provided organization data.
        ///  Created Date   :- 07-Nov-2024
        ///  Change Date    :- 07-Nov-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="mapDepartmentRole"> Map Department Role detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postmapdepartmentrole")]
        public async Task<IActionResult> PostMapDepartmentRole([FromBody] MapDepartmentRole mapDepartmentRole)
        {
            ApiResponseModel<MapDepartmentRole> apiResponse = new ApiResponseModel<MapDepartmentRole>();
            if (mapDepartmentRole == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditMapDepartmentRole, mapDepartmentRole);
            apiResponse.IsSuccess = mapDepartmentRole.MessageType == 1;
            apiResponse.Message = mapDepartmentRole.StatusMessage;
            apiResponse.MessageType = mapDepartmentRole.MessageType;
            apiResponse.StatusCode = mapDepartmentRole.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return mapDepartmentRole.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the Map Department Role detail based on the provided mapDepartmentRole and ID. 
        ///                    If the ID does not match or mapDepartmentRole is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 07-Nov-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the  Map Department Role to update.</param>
        /// <param name="mapDepartmentRole">The  Map Department Role detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatemapdepartmentrole/{id}")]
        public async Task<IActionResult> PutMapDepartmentRole(int id, [FromBody] MapDepartmentRole mapDepartmentRole)
        {
            ApiResponseModel<MapDepartmentRole> apiResponse = new ApiResponseModel<MapDepartmentRole>();
            // Check if the provided mapDepartmentRole is null or the id doesn't match the RoleDepartment_Id
            if (id <= 0 || mapDepartmentRole == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            mapDepartmentRole.RoleDepartment_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditMapDepartmentRole, mapDepartmentRole);
            apiResponse.IsSuccess = mapDepartmentRole.MessageType == 1;
            apiResponse.Message = mapDepartmentRole.StatusMessage;
            apiResponse.MessageType = mapDepartmentRole.MessageType;
            apiResponse.StatusCode = mapDepartmentRole.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return mapDepartmentRole.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided Map Department Role and ID. 
        ///                    If the ID does not match or Map Department Role  is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 08-Nov-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the Map Department Role to update.</param>
        /// <param name="mapDepartmentRole">The  Map Department Role detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletemapdepartmentrole/{id}")]
        public async Task<IActionResult> DeleteMapDepartmentRole(int id, [FromBody] MapDepartmentRole mapDepartmentRole)
        {
            ApiResponseModel<MapDepartmentRole> apiResponse = new ApiResponseModel<MapDepartmentRole>();
            // Check if the provided Map Department Role is null or the id doesn't match the RoleDepartment_Id
            if (id <= 0 || mapDepartmentRole == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            mapDepartmentRole.RoleDepartment_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteMapDepartmentRole, mapDepartmentRole);
            apiResponse.IsSuccess = mapDepartmentRole.MessageType == 1;
            apiResponse.Message = mapDepartmentRole.StatusMessage;
            apiResponse.MessageType = mapDepartmentRole.MessageType;
            apiResponse.StatusCode = mapDepartmentRole.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return mapDepartmentRole.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
    }
}
