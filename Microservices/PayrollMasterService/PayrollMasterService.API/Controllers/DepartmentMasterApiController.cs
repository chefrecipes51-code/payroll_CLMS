/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-96,203                                                                  *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for DepartmentMaster entries.                           *
 *  It includes APIs to retrieve, create, update, and delete DepartmentMaster                       *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllDepartmentMaster : Retrieves all DepartmentMaster records.                              *
 *  - GetDepartmentMasterById: Retrieves a specific DepartmentMaster record by ID.                  *
 *  - PostDepartmentMaster   : Adds a new DepartmentMaster record.                                  *
 *  - PutDepartmentMaster    : Updates an existing DepartmentMaster record.                         *
 *  - DeleteDepartmentMaster : Soft-deletes an DepartmentMaster record.                             *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 03-Oct-2024                                                                             *
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
    public class DepartmentMasterApiController : ControllerBase
    {
        private readonly IDepartmentMasterRepository _repository;
        public DepartmentMasterApiController(IDepartmentMasterRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #region Department Master Crud APIs Functionality
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all Department details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 03-Oct-2024
        ///  Last Modified  :- 25-Nov-2024
        ///  Modification   :- Added DepartmentCode.
        /// </summary>
        /// <returns>A JSON response with Department details or an appropriate message</returns>
        [HttpGet("getalldepartmentmaster")]
        public async Task<IActionResult> GetAllDepartmentMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<DepartmentMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var departmentMasters = await _repository.GetAllAsync(DbConstants.GetDepartmentMaster);
            // Check if data exists
            if (departmentMasters != null && departmentMasters.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = departmentMasters;
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
        /// Message Detail: API to retrieve Department details based on the provided  Department ID. 
        /// This method fetches data from the repository and returns the  Department detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 03-Oct-2024
        /// Change Date:  25-Nov-2024
        /// Change Detail: Added DepartmentCode.
        /// </summary>
        /// <param name="id">The ID of the Department to retrieve</param>
        /// <returns>Returns an API response with Department details or an error message.</returns>
        [HttpGet("getdepartmentmasterbyid/{id}")]
        public async Task<IActionResult> GetDepartmentMasterById(int id)
        {
            ApiResponseModel<IEnumerable<DepartmentMaster>> apiResponse = new ApiResponseModel<IEnumerable<DepartmentMaster>>();
            var departmentMaster = await _repository.GetAllByIdAsync(DbConstants.GetDepartmentMasterById, new { Department_Id = id });
            if (departmentMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = departmentMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of Department details based on the provided organization data.
        ///  Created Date   :- 03-Oct-2024
        ///  Change Date    :- 25-Nov-2024
        ///  Change detail  :- Added DepartmentCode.
        /// </summary>
        /// <param name="DepartmentMaster"> Department detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postdepartmentmaster")]
        public async Task<IActionResult> PostDepartmentMaster([FromBody] DepartmentMaster departmentMaster)
        {
            ApiResponseModel<DepartmentMaster> apiResponse = new ApiResponseModel<DepartmentMaster>();
            if (departmentMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditDepartmentMaster, departmentMaster);
            apiResponse.IsSuccess = departmentMaster.MessageType == 1;
            apiResponse.Message = departmentMaster.StatusMessage;
            apiResponse.MessageType = departmentMaster.MessageType;
            apiResponse.StatusCode = departmentMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return departmentMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the Department detail based on the provided DepartmentMaster and ID. 
        ///                    If the ID does not match or DepartmentMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 03-Oct-2024
        ///  Last Updated   :- 21-Nov-2024,25-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket,Added DepartmentCode.
        /// </summary>
        /// <param name="id">The ID of the  Department to update.</param>
        /// <param name="DepartmentMaster">The  Department detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatedepartmentmaster/{id}")]
        public async Task<IActionResult> PutDepartmentMaster(int id, [FromBody] DepartmentMaster departmentMaster)
        {
            ApiResponseModel<DepartmentMaster> apiResponse = new ApiResponseModel<DepartmentMaster>();
            // Check if the provided DepartmentMaster is null or the id doesn't match the Department_Id
            if (id <= 0 || departmentMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }
            departmentMaster.Department_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditDepartmentMaster, departmentMaster);
            apiResponse.IsSuccess = departmentMaster.MessageType == 1;
            apiResponse.Message = departmentMaster.StatusMessage;
            apiResponse.MessageType = departmentMaster.MessageType;
            apiResponse.StatusCode = departmentMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return departmentMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided Department Master and ID. 
        ///                    If the ID does not match or Department Master  is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 03-Oct-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the  Department to update.</param>
        /// <param name="DepartmentMaster">The  Department detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletedepartmentmaster/{id}")]
        public async Task<IActionResult> DeleteDepartmentMaster(int id, [FromBody] DepartmentMaster departmentMaster)
        {
            ApiResponseModel<DepartmentMaster> apiResponse = new ApiResponseModel<DepartmentMaster>();
            // Check if the provided Department is null or the id doesn't match the Department_Id
            if (id <= 0 || departmentMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }
            departmentMaster.Department_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteDepartmentMaster, departmentMaster);
            apiResponse.IsSuccess = departmentMaster.MessageType == 1;
            apiResponse.Message = departmentMaster.StatusMessage;
            apiResponse.MessageType = departmentMaster.MessageType;
            apiResponse.StatusCode = departmentMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return departmentMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }
        #endregion
    }
}
