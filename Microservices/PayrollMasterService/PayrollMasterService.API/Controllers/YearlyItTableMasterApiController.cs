/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-125                                                                  *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for YearlyItTableMaster entries.                        *
 *  It includes APIs to retrieve, create, update, and delete YearlyItTableMaster                    *
 *  records using the repository pattern and stored procedures and added a Caching Properties.      *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllYearlyItTableMaster : Retrieves all YearlyItTableMaster records.                        *
 *  - GetYearlyItTableMasterById: Retrieves a specific YearlyItTableMaster record by ID.            *
 *  - PostYearlyItTableMaster   : Adds a new YearlyItTableMaster record.                            *
 *  - PutYearlyItTableMaster    : Updates an existing YearlyItTableMaster record.                   *
 *  - DeleteYearlyItTableMaster : Soft-deletes an YearlyItTableMaster record.                       *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 21-Oct-2024                                                                             *
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
    public class YearlyItTableMasterApiController : ControllerBase
    {
        private readonly IYearlyItTableMasterRepository _repository;
        public YearlyItTableMasterApiController(IYearlyItTableMasterRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #region YearlyItTable Master Crud APIs Functionality
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all YearlyItTable details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 21-Oct-2024
        ///  Last Modified  :- 21-Oct-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Area details or an appropriate message</returns>
        [HttpGet("getallyearlyittable")]
        public async Task<IActionResult> GetAllYearlyItTable()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<YearlyItTableMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var yearlyITTableMaster = await _repository.GetAllAsync(DbConstants.GetYearlyITTable);

            // Check if data exists
            if (yearlyITTableMaster != null && yearlyITTableMaster.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = yearlyITTableMaster;
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
        /// Message Detail: API to retrieve Area details based on the provided  Area ID. 
        /// This method fetches data from the repository and returns the  area detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 21-Oct-2024
        /// Change Date: 21-Oct-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the  area to retrieve</param>
        /// <returns>Returns an API response with area details or an error message.</returns>
        [HttpGet("getyearlyittablebyid/{id}")]
        public async Task<IActionResult> GetYearlyItTableById(int id)
        {
            ApiResponseModel<YearlyItTableMaster> apiResponse = new ApiResponseModel<YearlyItTableMaster>();
            var yearlyITTableMaster = await _repository.GetByIdAsync(DbConstants.GetYearlyITTableById, new { YearlyItTable_Id = id });
            if (yearlyITTableMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = yearlyITTableMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of area details based on the provided organization data.
        ///  Created Date   :- 21-Oct-2024
        ///  Change Date    :- 21-Oct-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="YearlyITTableMaster"> area detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postyearlyittable")]
        public async Task<IActionResult> PostYearlyItTable([FromBody] YearlyItTableMaster yearlyITTableMaster)
        {
            ApiResponseModel<YearlyItTableMaster> apiResponse = new ApiResponseModel<YearlyItTableMaster>();
            if (yearlyITTableMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditYearlyITTable, yearlyITTableMaster);
            apiResponse.IsSuccess = yearlyITTableMaster.MessageType == 1;
            apiResponse.Message = yearlyITTableMaster.StatusMessage;
            apiResponse.MessageType = yearlyITTableMaster.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the area detail based on the provided YearlyITTableMaster and ID. 
        ///                    If the ID does not match or YearlyITTableMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 21-Oct-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the  area to update.</param>
        /// <param name="YearlyITTableMaster">The  area detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updateyearlyittable/{id}")]
        public async Task<IActionResult> PutYearlyItTable(int id, [FromBody] YearlyItTableMaster yearlyITTableMaster)
        {
            ApiResponseModel<YearlyItTableMaster> apiResponse = new ApiResponseModel<YearlyItTableMaster>();
            // Check if the provided YearlyITTableMaster is null or the id doesn't match the YearlyItTable_Id
            if (id <= 0 || yearlyITTableMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            yearlyITTableMaster.YearlyItTable_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditYearlyITTable, yearlyITTableMaster);
            apiResponse.IsSuccess = yearlyITTableMaster.MessageType == 1;
            apiResponse.Message = yearlyITTableMaster.StatusMessage;
            apiResponse.MessageType = yearlyITTableMaster.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided YearlyITTableMaster and ID. 
        ///                    If the ID does not match or YearlyITTableMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 21-Oct-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the  Area to update.</param>
        /// <param name="YearlyITTableMaster">The  Area detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deleteyearlyittable/{id}")]
        public async Task<IActionResult> DeleteYearlyItTable(int id, [FromBody] YearlyItTableMaster yearlyITTableMaster)
        {
            ApiResponseModel<YearlyItTableMaster> apiResponse = new ApiResponseModel<YearlyItTableMaster>();
            // Check if the provided area is null or the id doesn't match the YearlyItTable_Id
            if (id <= 0 || yearlyITTableMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            yearlyITTableMaster.YearlyItTable_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteYearlyITTable, yearlyITTableMaster);
            apiResponse.IsSuccess = yearlyITTableMaster.MessageType == 1;
            apiResponse.Message = yearlyITTableMaster.StatusMessage;
            apiResponse.MessageType = yearlyITTableMaster.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
    }
}
