/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-74                                                                   *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for LocationMaster entries.                             *
 *  It includes APIs to retrieve, create, update, and delete LocationMaster                         *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllLocationMaster : Retrieves all LocationMaster records.                                  *
 *  - GetLocationMasterById: Retrieves a specific LocationMaster record by ID.                      *
 *  - PostLocationMaster   : Adds a new LocationMaster record.                                      *
 *  - PutLocationMaster    : Updates an existing LocationMaster record.                             *
 *  - DeleteLocationMaster : Soft-deletes an LocationMaster record.                                 *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 24-Sep-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class LocationMasterApiController : ControllerBase
    {
        private readonly ILocationMasterRepository _repository;
        public LocationMasterApiController(ILocationMasterRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #region Location Master Crud APIs Functionality
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all  Location details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 24-Sep-2024
        ///  Last Modified  :- 24-Sep-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with  Location details or an appropriate message</returns>
        [HttpGet("getalllocationmaster")]
        public async Task<IActionResult> GetAllLocationMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<LocationMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var LocationMasters = await _repository.GetAllAsync(DbConstants.GetLocationMaster);
            // Check if data exists
            if (LocationMasters != null && LocationMasters.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = LocationMasters;
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
        /// Message Detail: API to retrieve  Location details based on the provided  Location ID. 
        /// This method fetches data from the repository and returns the  Location detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 24-Sep-2024
        /// Change Date: 24-Sep-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the  location to retrieve</param>
        /// <returns>Returns an API response with  Location details or an error message.</returns>
        [HttpGet("getlocationmasterbyid/{id}")]
        public async Task<IActionResult> GetLocationMasterById(int id)
        {
            ApiResponseModel<LocationMaster> apiResponse = new ApiResponseModel<LocationMaster>();
            var LocationMaster = await _repository.GetByIdAsync(DbConstants.GetLocationMasterById, new { Location_Id = id });
            if (LocationMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = LocationMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of  Location details based on the provided organization data.
        ///  Created Date   :- 24-Sep-2024
        ///  Change Date    :- 24-Sep-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="LocationMaster"> Location detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postlocationmaster")]
        public async Task<IActionResult> PostLocationMaster([FromBody] LocationMaster locationMaster)
        {
            ApiResponseModel<LocationMaster> apiResponse = new ApiResponseModel<LocationMaster>();
            if (locationMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditLocationMaster, locationMaster);
            apiResponse.IsSuccess = locationMaster.MessageType == 1;
            apiResponse.Message = locationMaster.StatusMessage;
            apiResponse.MessageType = locationMaster.MessageType;
            apiResponse.StatusCode = locationMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return locationMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the  Location detail based on the provided LocationMaster and ID. 
        ///                    If the ID does not match or LocationMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 24-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the  Location to update.</param>
        /// <param name="LocationMaster">The  Location detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatelocationmaster/{id}")]
        public async Task<IActionResult> PutLocationMaster(int id, [FromBody] LocationMaster locationMaster)
        {
            ApiResponseModel<LocationMaster> apiResponse = new ApiResponseModel<LocationMaster>();
            // Check if the provided locationMaster is null or the id doesn't match the Location_Id
            if (id <= 0 || locationMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            locationMaster.Location_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditLocationMaster, locationMaster);
            apiResponse.IsSuccess = locationMaster.MessageType == 1;
            apiResponse.Message = locationMaster.StatusMessage;
            apiResponse.MessageType = locationMaster.MessageType;
            apiResponse.StatusCode = locationMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return locationMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided LocationMaster and ID. 
        ///                    If the ID does not match or LocationMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 24-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the  Location to update.</param>
        /// <param name="LocationMaster">The  Location detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletelocationmaster/{id}")]
        public async Task<IActionResult> DeleteLocationMaster(int id, [FromBody] LocationMaster locationMaster)
        {
            ApiResponseModel<LocationMaster> apiResponse = new ApiResponseModel<LocationMaster>();
            // Check if the provided Location is null or the id doesn't match the Location_Id
            if (id <= 0 || locationMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            locationMaster.Location_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteLocationMaster, locationMaster);
            apiResponse.IsSuccess = locationMaster.MessageType == 1;
            apiResponse.Message = locationMaster.StatusMessage;
            apiResponse.MessageType = locationMaster.MessageType;
            apiResponse.StatusCode = locationMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return locationMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
    }
}
