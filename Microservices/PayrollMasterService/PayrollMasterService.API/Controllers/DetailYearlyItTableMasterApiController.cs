/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-133                                                                  *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for DetailYearlyItTableMaster entries.                  *
 *  It includes APIs to retrieve, create, update, and delete DetailYearlyItTableMaster              *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllDetailYearlyItTableMaster : Retrieves all DetailYearlyItTableMaster records.            *
 *  - GetDetailYearlyItTableMasterById: Retrieves a specific DetailYearlyItTableMaster record by ID.*
 *  - PostDetailYearlyItTableMaster   : Adds a new DetailYearlyItTableMaster record.                *
 *  - PutDetailYearlyItTableMaster    : Updates an existing DetailYearlyItTableMaster record.       *
 *  - DeleteDetailYearlyItTableMaster : Soft-deletes an DetailYearlyItTableMaster record.           *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 22-Oct-2024                                                                             *
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
    public class DetailYearlyItTableMasterApiController : ControllerBase
    {
        private readonly IDetailYearlyItTableMasterRepository _repository;
        public DetailYearlyItTableMasterApiController(IDetailYearlyItTableMasterRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #region DetailYearlyItTable Master Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all DetailYearlyItTable details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 22-Oct-2024
        ///  Last Modified  :- 22-Oct-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with DetailYearlyItTable details or an appropriate message</returns>
        [HttpGet("getalldetailyearlyittable")]
        public async Task<IActionResult> GetAllDetailYearlyItTable()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<DetailYearlyItTableMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var detailYearlyITTableMaster = await _repository.GetAllAsync(DbConstants.GetDeatilYearlyITTable);

            // Check if data exists
            if (detailYearlyITTableMaster != null && detailYearlyITTableMaster.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = detailYearlyITTableMaster;
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
        /// Message Detail: API to retrieve DetailYearlyItTable details based on the provided  DetailYearlyItTable ID. 
        /// This method fetches data from the repository and returns the  DetailYearlyItTable detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 22-Oct-2024
        /// Change Date: 22-Oct-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the  DetailYearlyItTable to retrieve</param>
        /// <returns>Returns an API response with DetailYearlyItTable details or an error message.</returns>
        [HttpGet("getdetailyearlyittablebyid/{id}")]
        public async Task<IActionResult> GetDetailYearlyItTableById(int id)
        {
            ApiResponseModel<DetailYearlyItTableMaster> apiResponse = new ApiResponseModel<DetailYearlyItTableMaster>();
            var detailYearlyItTableMaster = await _repository.GetByIdAsync(DbConstants.GetDeatilYearlyITTableById, new { YearlyItTableDetail_Id = id });
            if (detailYearlyItTableMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = detailYearlyItTableMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of DetailYearlyItTable details based on the provided organization data.
        ///  Created Date   :- 22-Oct-2024
        ///  Change Date    :- 22-Oct-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="detailYearlyItTableMaster"> DetailYearlyItTable detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postdetailyearlyittable")]
        public async Task<IActionResult> PostDetailYearlyItTable([FromBody] DetailYearlyItTableMaster detailYearlyItTableMaster)
        {
            ApiResponseModel<DetailYearlyItTableMaster> apiResponse = new ApiResponseModel<DetailYearlyItTableMaster>();
            if (detailYearlyItTableMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditDeatilYearlyITTable, detailYearlyItTableMaster);
            apiResponse.IsSuccess = detailYearlyItTableMaster.MessageType == 1;
            apiResponse.Message = detailYearlyItTableMaster.StatusMessage;
            apiResponse.MessageType = detailYearlyItTableMaster.MessageType;
            apiResponse.StatusCode = detailYearlyItTableMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return detailYearlyItTableMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the DetailYearlyItTable detail based on the provided YearlyITTableMaster and ID. 
        ///                    If the ID does not match or YearlyITTableMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 22-Oct-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the  DetailYearlyItTable to update.</param>
        /// <param name="detailYearlyItTableMaster">The DetailYearlyItTable detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatedetailyearlyittable/{id}")]
        public async Task<IActionResult> PutDetailYearlyItTable(int id, [FromBody] DetailYearlyItTableMaster detailYearlyItTableMaster)
        {
            ApiResponseModel<DetailYearlyItTableMaster> apiResponse = new ApiResponseModel<DetailYearlyItTableMaster>();
            // Check if the provided YearlyITTableMaster is null or the id doesn't match the YearlyItTable_Id
            if (id <= 0 || detailYearlyItTableMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            detailYearlyItTableMaster.YearlyItTableDetail_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditDeatilYearlyITTable, detailYearlyItTableMaster);
            apiResponse.IsSuccess = detailYearlyItTableMaster.MessageType == 1;
            apiResponse.Message = detailYearlyItTableMaster.StatusMessage;
            apiResponse.MessageType = detailYearlyItTableMaster.MessageType;
            apiResponse.StatusCode = detailYearlyItTableMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return detailYearlyItTableMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided YearlyITTableMaster and ID. 
        ///                    If the ID does not match or YearlyITTableMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 24-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the  DetailYearlyItTable to update.</param>
        /// <param name="detailYearlyItTableMaster">The  DetailYearlyItTable detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletedetailyearlyittable/{id}")]
        public async Task<IActionResult> DeleteDetailYearlyItTable(int id, [FromBody] DetailYearlyItTableMaster detailYearlyItTableMaster)
        {
            ApiResponseModel<DetailYearlyItTableMaster> apiResponse = new ApiResponseModel<DetailYearlyItTableMaster>();
            // Check if the provided DetailYearlyItTable is null or the id doesn't match the YearlyItTableDetail_Id
            if (id <= 0 || detailYearlyItTableMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            detailYearlyItTableMaster.YearlyItTableDetail_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteDeatilYearlyITTable, detailYearlyItTableMaster);
            apiResponse.IsSuccess = detailYearlyItTableMaster.MessageType == 1;
            apiResponse.Message = detailYearlyItTableMaster.StatusMessage;
            apiResponse.MessageType = detailYearlyItTableMaster.MessageType;
            apiResponse.StatusCode = detailYearlyItTableMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return detailYearlyItTableMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
    }
}
