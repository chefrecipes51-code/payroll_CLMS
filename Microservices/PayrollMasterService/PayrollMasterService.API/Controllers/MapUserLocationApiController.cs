/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-194                                                                  *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for MapUserLocation entries.                            *
 *  It includes APIs to retrieve, create, update, and delete MapUserLocation using bulk insertion.  *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllMapUserLocation : Retrieves all MapUserLocation records.                                *
 *  - GetMapUserLocationById: Retrieves a specific MapUserLocation record by ID.                    *
 *  - PostMapUserLocation   : Adds a new MapUserLocation record.                                    *
 *  - PutMapUserLocation    : Updates an existing MapUserLocation record.                           *
 *  - DeleteMapUserLocation : Soft-deletes an MapUserLocation record.                               *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 06-Nov-2024                                                                             *
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
    public class MapUserLocationApiController : ControllerBase
    {
        private readonly IMapUserLocationRepository _repository;
        public MapUserLocationApiController(IMapUserLocationRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #region Map User Location Master Crud APIs Functionality
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all Map User Location details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 06-Nov-2024
        ///  Last Modified  :- 20-Nov-2024
        ///  Modification   :- Added CompanyName and UserName fields based on the database schema.
        /// </summary>
        /// <returns>A JSON response with Map User Location details or an appropriate message</returns>
        [HttpGet("getallmapuserlocation")]
        public async Task<IActionResult> GetAllMapUserLocation()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<MapUserLocation>>();
            // Fetching data from the repository by executing the stored procedure
            var mapUserLocation = await _repository.GetAllAsync(DbConstants.GetMapUserLocation);

            // Check if data exists
            if (mapUserLocation != null && mapUserLocation.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = mapUserLocation;
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
        /// Message Detail: API to retrieve Map User Location details based on the provided  UserMapLocation_Id. 
        /// This method fetches data from the repository and returns the Map User Location detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 06-Nov-2024
        /// Change Date:  06-Nov-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the  map user location to retrieve</param>
        /// <returns>Returns an API response with map user location details or an error message.</returns>
        [HttpGet("getmapuserlocationbyid/{id}")]
        public async Task<IActionResult> GetMapUserLocationById(int id)
        {
            ApiResponseModel<MapUserLocation> apiResponse = new ApiResponseModel<MapUserLocation>();
            var mapUserLocation = await _repository.GetByIdAsync(DbConstants.GetMapUserLocationById, new { UserMapLocation_Id = id });
            if (mapUserLocation == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = mapUserLocation;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of map user location details based on the provided organization data.
        ///  Created Date   :- 06-Nov-2024
        ///  Change Date    :- 20-Nov-2024
        ///  Change detail  :- Added a bulk insert functionality using a User-Defined Table (UDT).
        /// </summary>
        /// <param name="mapUserLocation"> map user location detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postmapuserlocation")]
        public async Task<IActionResult> PostMapUserLocation([FromBody] MapUserLocation mapUserLocation)
        {
            var apiResponse = new ApiResponseModel<MapUserLocation>();
            if (mapUserLocation == null || mapUserLocation.UserMapLocations == null || !mapUserLocation.UserMapLocations.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditMapUserLocation, mapUserLocation);
            apiResponse.IsSuccess = mapUserLocation.MessageType == 1;
            apiResponse.Message = mapUserLocation.StatusMessage;
            apiResponse.MessageType = mapUserLocation.MessageType;
            apiResponse.StatusCode = mapUserLocation.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return mapUserLocation.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the map user location detail based on the provided map user location and ID. 
        ///                    If the ID does not match or MapUserLocation is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 06-Nov-2024
        ///  Last Updated   :- 20-Nov-2024
        ///  Change Details :- Added a bulk insert functionality using a User-Defined Table (UDT).
        /// </summary>
        /// <param name="id">The ID of the map user location to update.</param>
        /// <param name="mapUserLocation">The map user location detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatemapuserlocation/{id}")]
        public async Task<IActionResult> PutMapUserLocationMaster(int id, [FromBody] MapUserLocation mapUserLocation)
        {
            ApiResponseModel<MapUserLocation> apiResponse = new ApiResponseModel<MapUserLocation>();
            if (mapUserLocation == null || id <= 0 || mapUserLocation.UserMapLocations == null || !mapUserLocation.UserMapLocations.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            // Call the UpdateAsync method in the repository
            mapUserLocation.UserMapLocations[0].UserMapLocation_Id = id;
            await _repository.UpdateAsync(DbConstants.AddEditMapUserLocation, mapUserLocation);
            apiResponse.IsSuccess = mapUserLocation.MessageType == 1;
            apiResponse.Message = mapUserLocation.StatusMessage;
            apiResponse.MessageType = mapUserLocation.MessageType;
            apiResponse.StatusCode = mapUserLocation.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return mapUserLocation.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided MapUserLocation and ID. 
        ///                    If the ID does not match or MapUserLocation is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 06-Nov-2024
        ///  Last Updated   :- 06-Nov-2024
        ///  Change Details :- Initial implementation.
        /// </summary>
        /// <param name="id">The ID of the map user location to update.</param>
        /// <param name="mapUserLocation">The map user location detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletemapuserlocation/{id}")]
        public async Task<IActionResult> DeleteMapUserLocation(int id, [FromBody] MapUserLocation mapUserLocation)
        {
            ApiResponseModel<MapUserLocation> apiResponse = new ApiResponseModel<MapUserLocation>();
            // Check if the provided mapUserLocation is null or the id doesn't match the UserMapLocation_Id
            if (mapUserLocation == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            mapUserLocation.UserMapLocation_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteMapUserLocation, mapUserLocation);
            apiResponse.IsSuccess = mapUserLocation.MessageType == 1;
            apiResponse.Message = mapUserLocation.StatusMessage;
            apiResponse.MessageType = mapUserLocation.MessageType;
            apiResponse.StatusCode = mapUserLocation.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return mapUserLocation.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
    }
}
