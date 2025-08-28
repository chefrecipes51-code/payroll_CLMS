/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-177                                                                  *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for EventMaster entries.                                *
 *  It includes APIs to retrieve, create, update, and delete EventMaster                            *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllEventMaster : Retrieves all EventMaster records.                                        *
 *  - GetEventMasterById: Retrieves a specific EventMaster record by ID.                            *
 *  - PostEventMaster   : Adds a new EventMaster record.                                            *
 *  - PutEventMaster    : Updates an existing EventMaster record.                                   *
 *  - DeleteEventMaster : Soft-deletes an EventMaster record.                                       *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 12-Nov-2024                                                                             *
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
    public class EventMasterApiController : ControllerBase
    {
        private readonly IEventMasterRepository _repository;
        public EventMasterApiController(IEventMasterRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #region Event Master Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all Event details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 12-Nov-2024
        ///  Last Modified  :- 12-Nov-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Event details or an appropriate message</returns>
        [HttpGet("getalleventmaster")]
        public async Task<IActionResult> GetAllEventMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<EventMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var eventMasters = await _repository.GetAllAsync(DbConstants.GetEventMaster);

            // Check if data exists
            if (eventMasters != null && eventMasters.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = eventMasters;
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
        /// Message Detail: API to retrieve Event details based on the provided  Event ID. 
        /// This method fetches data from the repository and returns the  event detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 12-Nov-2024
        /// Change Date: 12-Nov-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the  event to retrieve</param>
        /// <returns>Returns an API response with event details or an error message.</returns>
        [HttpGet("geteventmasterbyid/{id}")]
        public async Task<IActionResult> GetEventMasterById(int id)
        {
            ApiResponseModel<EventMaster> apiResponse = new ApiResponseModel<EventMaster>();
            var eventMaster = await _repository.GetByIdAsync(DbConstants.GetEventMasterById, new { Event_Id = id });
            if (eventMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = eventMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of event details based on the provided organization data.
        ///  Created Date   :- 12-Nov-2024
        ///  Change Date    :- 12-Nov-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="eventMaster"> event detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("posteventmaster")]
        public async Task<IActionResult> PostEventMaster([FromBody] EventMaster eventMaster)
        {
            ApiResponseModel<EventMaster> apiResponse = new ApiResponseModel<EventMaster>();
            if (eventMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddEditEventMaster, eventMaster);
            // Handle response based on MessageType
            apiResponse.IsSuccess = eventMaster.MessageType == 1;
            apiResponse.Message = eventMaster.StatusMessage;
            apiResponse.MessageType = eventMaster.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the event detail based on the provided eventmaster and ID. 
        ///                    If the ID does not match or EventMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 12-Nov-2024
        ///  Last Updated   :- 12-Nov-2024
        ///  Change Details :- Initial implementation.
        /// </summary>
        /// <param name="id">The ID of the  event to update.</param>
        /// <param name="eventMaster">The  event detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updateeventmaster/{id}")]
        public async Task<IActionResult> PutEventMaster(int id, [FromBody] EventMaster eventMaster)
        {
            ApiResponseModel<EventMaster> apiResponse = new ApiResponseModel<EventMaster>();
            // Check if the provided eventMaster is null or the id doesn't match the Event_Id
            if (eventMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            eventMaster.Event_Id = id;// Set the event ID
            await _repository.UpdateAsync(DbConstants.AddEditEventMaster, eventMaster);// Call the UpdateAsync method in the repository
            // Handle response based on MessageType
            apiResponse.IsSuccess = eventMaster.MessageType == 1;
            apiResponse.Message = eventMaster.StatusMessage;
            apiResponse.MessageType = eventMaster.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created: ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
            
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided EventMaster and ID. 
        ///                    If the ID does not match or EventMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 12-Nov-2024
        ///  Last Updated   :- 12-Nov-2024
        ///  Change Details :- Initial implementation.
        /// </summary>
        /// <param name="id">The ID of the  Event to update.</param>
        /// <param name="eventMaster">The  Event detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deleteeventmaster/{id}")]
        public async Task<IActionResult> DeleteEventMaster(int id, [FromBody] EventMaster eventMaster)
        {
            ApiResponseModel<EventMaster> apiResponse = new ApiResponseModel<EventMaster>();
            // Check if the provided event is null or the id doesn't match the Event_Id
            if (eventMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            eventMaster.Event_Id = id;
            await _repository.DeleteAsync(DbConstants.DeleteEventMaster, eventMaster); // Call the DeleteAsync method in the repository
            // Handle response based on MessageType
            apiResponse.IsSuccess = eventMaster.MessageType == 1;
            apiResponse.Message = eventMaster.StatusMessage;
            apiResponse.MessageType = eventMaster.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
    }
}
