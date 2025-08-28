/****************************************************************************************************
 *  Jira Task Ticket :PAYROLL-105                                                                   *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for MapDepartmentLocation entries.                      *
 *  It includes APIs to retrieve, create, update, and delete MapDepartmentLocation                  *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllMapDepartmentLocation : Retrieves all MapDepartmentLocation records.                    *
 *  - GetMapDepartmentLocationById: Retrieves a specific MapDepartmentLocation record by ID.        *
 *  - PostMapDepartmentLocation   : Adds a new MapDepartmentLocation record.                        *
 *  - PutMapDepartmentLocation    : Updates an existing MapDepartmentLocation record.               *
 *  - DeleteMapDepartmentLocation : Soft-deletes an MapDepartmentLocation record.                   *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 09-Oct-2024                                                                             *
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
    public class MapDepartmentLocationApiController : ControllerBase
    {
        private readonly IMapDepartmentLocationRepository _repository;
        public MapDepartmentLocationApiController(IMapDepartmentLocationRepository repository)
        {
            _repository = repository;
        }
        #region Map Department Location Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all Map Department Location details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 09-Oct-2024
        ///  Last Modified  :- 09-Oct-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Map Department Location details or an appropriate message</returns>
        [HttpGet("getallmapdepartmentlocation")]
        public async Task<IActionResult> GetAllMapDepartmentLocation()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<MapDepartmentLocation>>();
            // Fetching data from the repository by executing the stored procedure
            var mapDepartmentLocation = await _repository.GetAllAsync(DbConstants.GetMapDepartmentLocation);
            // Check if data exists
            if (mapDepartmentLocation != null && mapDepartmentLocation.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = mapDepartmentLocation;
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
        /// Message Detail: API to retrieve Map Department Location details based on the provided  Department Location ID. 
        /// This method fetches data from the repository and returns the  Map Department Location detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 09-Oct-2024
        /// Change Date: 09-Oct-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the Map Department Location to retrieve</param>
        /// <returns>Returns an API response with Map Department Location details or an error message.</returns>
        [HttpGet("getmapdepartmentlocationbyid/{id}")]
        public async Task<IActionResult> GetMapDepartmentLocationById(int id)
        {
            ApiResponseModel<MapDepartmentLocation> apiResponse = new ApiResponseModel<MapDepartmentLocation>();
            var mapDepartmentLocation = await _repository.GetByIdAsync(DbConstants.GetMapDepartmentLocationById, new { Department_Location_Id = id });
            if (mapDepartmentLocation == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = mapDepartmentLocation;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of Map Department Location details based on the provided organization data.
        ///  Created Date   :- 09-Oct-2024
        ///  Change Date    :- 09-Oct-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="mapDepartmentLocation"> Map Department Location detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postmapdepartmentlocation")]
        public async Task<IActionResult> PostMapDepartmentLocation([FromBody] MapDepartmentLocation mapDepartmentLocation)
        {
            ApiResponseModel<MapDepartmentLocation> apiResponse = new ApiResponseModel<MapDepartmentLocation>();
            if (mapDepartmentLocation == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditMapDepartmentLocation, mapDepartmentLocation);
            apiResponse.IsSuccess = mapDepartmentLocation.MessageType == 1;
            apiResponse.Message = mapDepartmentLocation.StatusMessage;
            apiResponse.MessageType = mapDepartmentLocation.MessageType;
            apiResponse.StatusCode = mapDepartmentLocation.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return mapDepartmentLocation.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the Map Department Location detail based on the provided mapDepartmentLocation and ID. 
        ///                    If the ID does not match or mapDepartmentLocation is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 09-Oct-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the  Map Department Location to update.</param>
        /// <param name="mapDepartmentLocation">The  Map Department Location detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatemapDepartmentLocation/{id}")]
        public async Task<IActionResult> PutMapDepartmentLocation(int id, [FromBody] MapDepartmentLocation mapDepartmentLocation)
        {
            ApiResponseModel<DepartmentMaster> apiResponse = new ApiResponseModel<DepartmentMaster>();
            // Check if the provided mapDepartmentLocation is null or the id doesn't match the Department_Location_Id
            if (id <= 0 || mapDepartmentLocation == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }
            mapDepartmentLocation.Department_Location_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditMapDepartmentLocation, mapDepartmentLocation);
            apiResponse.IsSuccess = mapDepartmentLocation.MessageType == 1;
            apiResponse.Message = mapDepartmentLocation.StatusMessage;
            apiResponse.MessageType = mapDepartmentLocation.MessageType;
            apiResponse.StatusCode = mapDepartmentLocation.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return mapDepartmentLocation.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided Map Department Location and ID. 
        ///                    If the ID does not match or Map Department Location  is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 09-Oct-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the  Map Department Location to update.</param>
        /// <param name="mapDepartmentLocation">The  Map Department Location detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletemapDepartmentLocation/{id}")]
        public async Task<IActionResult> DeleteMapDepartmentLocation(int id, [FromBody] MapDepartmentLocation mapDepartmentLocation)
        {
            ApiResponseModel<MapDepartmentLocation> apiResponse = new ApiResponseModel<MapDepartmentLocation>();
            // Check if the provided Department is null or the id doesn't match the Department_Id
            if (id <= 0 || mapDepartmentLocation == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }
            mapDepartmentLocation.Department_Location_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteMapDepartmentLocation, mapDepartmentLocation);
            apiResponse.IsSuccess = mapDepartmentLocation.MessageType == 1;
            apiResponse.Message = mapDepartmentLocation.StatusMessage;
            apiResponse.MessageType = mapDepartmentLocation.MessageType;
            apiResponse.StatusCode = mapDepartmentLocation.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return mapDepartmentLocation.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }
        #endregion

        #region Floor Master APIs Functionality

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all Floor details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 05-Jan-2025
        ///  Last Modified  :- 05-Jan-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Map Department Location details or an appropriate message</returns>
        [HttpGet("getallfloormaster")]
        public async Task<IActionResult> GetAllFloorMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<FloorMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var floorMaster = await _repository.GetAllFloorAsync(DbConstants.GetAllFloorMaster);
            // Check if data exists
            if (floorMaster != null && floorMaster.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = floorMaster;
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
        /// Message Detail: API to retrieve Map Department Location details based on the provided  Department Location ID. 
        /// This method fetches data from the repository and returns the  Map Department Location detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 09-Oct-2024
        /// Change Date: 09-Oct-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the Map Department Location to retrieve</param>
        /// <returns>Returns an API response with Map Department Location details or an error message.</returns>
        [HttpGet("getfloormasterbyid/{id}")]
        public async Task<IActionResult> GetFloorMasterById(int id)
        {
            ApiResponseModel<IEnumerable<FloorMaster>> apiResponse = new ApiResponseModel<IEnumerable<FloorMaster>>();

            //ApiResponseModel<AreaMaster> apiResponse = new ApiResponseModel<AreaMaster>();
            var floorMaster = await _repository.GetFloorByIdAsync(DbConstants.GetFloorMasterById, new { Floor_Id = id });
            if (floorMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = floorMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        #endregion
    }
}


