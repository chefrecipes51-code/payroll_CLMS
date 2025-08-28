/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-58,59                                                                *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for WageGradeMaster entries.                            *
 *  It includes APIs to retrieve, create, update, and delete WageGradeMaster                        *
 *  records using the repository pattern and stored procedures and added a Caching Properties.      *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllWageGradeMaster : Retrieves all WageGradeMaster records.                                *
 *  - GetWageGradeMasterById: Retrieves a specific WageGradeMaster record by ID.                    *
 *  - PostWageGradeMaster   : Adds a new WageGradeMaster record.                                    *
 *  - PutWageGradeMaster    : Updates an existing WageGradeMaster record.                           *
 *  - DeleteWageGradeMaster : Soft-deletes an WageGradeMaster record.                               *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 13-Sep-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.Repository.Interface;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class WageGradeMasterApiController : ControllerBase
    {
        private readonly IWageGradeMasterRepository _repository;
        private readonly ICachingServiceRepository _cachingService;
        private const string CacheKey = "WageGradeMasterCache"; // Unique key for caching
        public WageGradeMasterApiController(IWageGradeMasterRepository repository, ICachingServiceRepository cachingService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cachingService = cachingService;
        }
        #region Wage Grade Master Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all wage grade details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 13-Sep-2024
        ///  Last Modified  :- 13-Sep-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with wage grade details or an appropriate message</returns>
        [HttpGet("getallwagegrademaster")]
        public async Task<IActionResult> GetAllWageGradeMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<WageGradeMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var wageGradeDetails = _cachingService.GetOrCreate(CacheKey, entry =>
            {
                // Create the item for the cache (repository call)
                // Since this is async, use Task.Run to wrap the async call in a synchronous context for caching
                return Task.Run(async () => await _repository.GetAllAsync(DbConstants.GetWageGradeMaster)).Result;
            }, TimeSpan.FromHours(1)); // Cache expiration time 

            // Check if data exists
            if (wageGradeDetails != null && wageGradeDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = wageGradeDetails;
                //apiResponse.Message = ApiResponseMessageConstant.FetchAllRecords;
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
        /// Message Detail: API to retrieve wage grade details based on the provided wage grade ID. 
        /// This method fetches data from the repository and returns the wage grade detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 13-Sep-2024
        /// Change Date: 13-Sep-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the wage grade to retrieve</param>
        /// <returns>Returns an API response with wage grade details or an error message.</returns>
        [HttpGet("getwagegrademasterbyid/{id}")]
        public async Task<IActionResult> GetWageGradeMasterById(int id)
        {
            ApiResponseModel<WageGradeMaster> apiResponse = new ApiResponseModel<WageGradeMaster>();
            // Attempt to retrieve from cache or fetch and create cache entry
            var wageGradeMaster = _cachingService.GetOrCreate(CacheKey, cacheEntry =>
            {
                // Fetch from the repository if not in cache
                return Task.Run(async () => await _repository.GetByIdAsync(DbConstants.GetWageGradeMasterById, new { Wage_Id = id })).Result;
            }, TimeSpan.FromHours(1)); // Cache expiration set to 10 minutes
            if (wageGradeMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = wageGradeMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of wage grade details based on the provided organization data.
        ///  Created Date   :- 16-Sep-2024
        ///  Change Date    :- 16-Sep-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="wageGradeDetail">Wage grade detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postwagegrademaster")]
        public async Task<IActionResult> PostWageGradeMaster([FromBody] WageGradeMaster wageGradeDetail)
        {
            ApiResponseModel<WageGradeMaster> apiResponse = new ApiResponseModel<WageGradeMaster>();
            if (wageGradeDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddEditWageGradeMaster, wageGradeDetail);
            apiResponse.IsSuccess = wageGradeDetail.MessageType == 1;
            apiResponse.Message = wageGradeDetail.StatusMessage;
            apiResponse.MessageType = wageGradeDetail.MessageType;
            if (apiResponse.IsSuccess)
            {
                // Remove cache if operation is successful
                _cachingService.Remove(CacheKey);
                apiResponse.StatusCode = ApiResponseStatusConstant.Created;
            }
            else
            {
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
            }
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the wage grade detail based on the provided wageGradeDetail and ID. 
        ///                    If the ID does not match or wageGradeDetail is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 16-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the Wage Grade to update.</param>
        /// <param name="wageGradeDetail">The wage grade detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatewagegrademaster/{id}")]
        public async Task<IActionResult> PutWageGradeMaster(int id, [FromBody] WageGradeMaster wageGradeDetail)
        {
            ApiResponseModel<WageGradeMaster> apiResponse = new ApiResponseModel<WageGradeMaster>();
            // Check if the provided wageGradeDetail is null or the id doesn't match the Wage_Id
            if (id <= 0 || wageGradeDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            wageGradeDetail.Wage_Id = id;
            // Call the UpdateWageGradeAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditWageGradeMaster, wageGradeDetail);
            apiResponse.IsSuccess = wageGradeDetail.MessageType == 1;
            apiResponse.Message = wageGradeDetail.StatusMessage;
            apiResponse.MessageType = wageGradeDetail.MessageType;
            if (apiResponse.IsSuccess)
            {
                // Remove cache if operation is successful
                _cachingService.Remove(CacheKey);
                apiResponse.StatusCode = ApiResponseStatusConstant.Created;
            }
            else
            {
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
            }            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided wageGradeDetail and ID. 
        ///                    If the ID does not match or wageGradeDetail is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 19-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the earning deduction to update.</param>
        /// <param name="wageGradeDetail">The Wage Grade detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletewagegrademaster/{id}")]
        public async Task<IActionResult> DeleteWageGradeMaster(int id, [FromBody] WageGradeMaster wageGradeDetail)
        {
            ApiResponseModel<WageGradeMaster> apiResponse = new ApiResponseModel<WageGradeMaster>();
            // Check if the provided wageGradeDetail is null or the id doesn't match the Wage_Id
            if (id <= 0 || wageGradeDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            wageGradeDetail.Wage_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteWageGradeMaster, wageGradeDetail);
            apiResponse.IsSuccess = wageGradeDetail.MessageType == 1;
            apiResponse.Message = wageGradeDetail.StatusMessage;
            apiResponse.MessageType = wageGradeDetail.MessageType;
            if (apiResponse.IsSuccess)
            {
                // Remove cache if operation is successful
                _cachingService.Remove(CacheKey);
                apiResponse.StatusCode = ApiResponseStatusConstant.Created;
            }
            else
            {
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
            }            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        #endregion
    }
}
