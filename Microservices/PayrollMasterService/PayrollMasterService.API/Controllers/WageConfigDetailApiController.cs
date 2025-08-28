/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-413                                                                  *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for WageConfigDetails entries.                          *
 *  It includes APIs to retrieve, create, update, and delete WageConfigDetails                      *
 *  records using the repository pattern and stored procedures and added a Caching Properties.      *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllWageConfigDetails : Retrieves all WageConfigDetails records.                            *
 *  - GetWageConfigDetailById: Retrieves a specific WageConfigDetails record by ID.                *
 *  - PostWageConfigDetails   : Adds a new WageConfigDetails record.                                *
 *  - PutWageConfigDetails    : Updates an existing WageConfigDetails record.                       *
 *  - DeleteWageConfigDetails : Soft-deletes an WageConfigDetails record.                           *
 *                                                                                                  *
 *  Author: Chirag Gurjar                                                                           *
 *  Date  : 28-jan-2025                                                                             *
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
    public class WageConfigDetailApiController : ControllerBase
    {
        private readonly IWageConfigDetailRepository _repository;
        private readonly ICachingServiceRepository _cachingService;
        private const string CacheKey = "WageConfigDetailCache"; // Unique key for caching
        public WageConfigDetailApiController(IWageConfigDetailRepository repository, ICachingServiceRepository cachingService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cachingService = cachingService;
        }
        #region Wage Config Detail Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail    :- This API retrieves all wage Config details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 28-jan-2025
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with wage config details or an appropriate message</returns>
        [HttpGet("getallwageconfigdetail")]
        public async Task<IActionResult> GetAllWageconfigDetail()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<WageConfigDetail>>();
            // Fetching data from the repository by executing the stored procedure
            var wageConfigDetails = _cachingService.GetOrCreate(CacheKey, entry =>
            {
                // Create the item for the cache (repository call)
                // Since this is async, use Task.Run to wrap the async call in a synchronous context for caching
                return Task.Run(async () => await _repository.GetAllAsync(DbConstants.GetWageConfigDetails)).Result;
            }, TimeSpan.FromHours(1)); // Cache expiration time 

            // Check if data exists
            if (wageConfigDetails != null && wageConfigDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = wageConfigDetails;
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
        /// Developer Name: Chirag Gurjar
        /// Message Detail: API to retrieve wage Config details based on the provided wage Config ID. 
        /// This method fetches data from the repository and returns the wage Config detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 28-jan-2025
        /// Change Date: 
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the wage Config to retrieve</param>
        /// <returns>Returns an API response with wage Config details or an error message.</returns>
        [HttpGet("getwageconfigdetailbyid/{id}")]
        public async Task<IActionResult> GetWageConfigDetailById(int id)
        {
            ApiResponseModel<WageConfigDetail> apiResponse = new ApiResponseModel<WageConfigDetail>();
            // Attempt to retrieve from cache or fetch and create cache entry
            var wageConfigDetails = _cachingService.GetOrCreate(CacheKey, cacheEntry =>
            {
                // Fetch from the repository if not in cache
                return Task.Run(async () => await _repository.GetByIdAsync(DbConstants.GetWageConfigDetailById, new { WageConfig_Dtl_Id = id })).Result;
            }, TimeSpan.FromHours(1)); // Cache expiration set to 10 minutes
            if (wageConfigDetails == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = wageConfigDetails;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail :- This API handles the addition of wage Config details based on the provided organization data.
        ///  Created Date   :- 28-jan-2025
        ///  Change Date    :- 
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="wageConfigDetail">Wage Config detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postwageconfigdetail")]
        public async Task<IActionResult> PostWageConfigDetail([FromBody] WageConfigDetail wageConfigDetail)
        {
            ApiResponseModel<WageConfigDetail> apiResponse = new ApiResponseModel<WageConfigDetail>();
            if (wageConfigDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddEditWageConfigDetails, wageConfigDetail);
            apiResponse.IsSuccess = wageConfigDetail.MessageType == 1;
            apiResponse.Message = wageConfigDetail.StatusMessage;
            apiResponse.MessageType = wageConfigDetail.MessageType;
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
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail    :- Updates the wage Config detail based on the provided wageConfigDetail and ID. 
        ///                    If the ID does not match or wageConfigDetail is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 28-jan-2025
        ///  Last Updated   :- 
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the Wage Config to update.</param>
        /// <param name="wageConfigDetail">The wage Config detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatewageconfigdetail")]
        public async Task<IActionResult> PutWageConfigDetail( [FromBody] WageConfigDetail wageConfigDetail)
        {
            ApiResponseModel<WageConfigDetail> apiResponse = new ApiResponseModel<WageConfigDetail>();
            // Check if the provided wageConfigDetail is null or the id doesn't match the Wage_Id
            if (wageConfigDetail.WageConfig_Dtl_Id <= 0 || wageConfigDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

           // wageConfigDetail.Wage_Id = id;
            // Call the UpdateWageConfigAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditWageConfigDetails, wageConfigDetail);
            apiResponse.IsSuccess = wageConfigDetail.MessageType == 1;
            apiResponse.Message = wageConfigDetail.StatusMessage;
            apiResponse.MessageType = wageConfigDetail.MessageType;
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
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail    :- Updates the delete column based on the provided wageConfigDetail and ID. 
        ///                    If the ID does not match or wageConfigDetail is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 28-jan-2025
        ///  Last Updated   :- 
        ///  Change Details :- 
        /// </summary>
        /// <param name="id">The ID of the earning deduction to update.</param>
        /// <param name="wageConfigDetail">The Wage Config detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletewageconfigdetail/{id}")]
        public async Task<IActionResult> DeleteWageConfigDetail(int id, [FromBody] WageConfigDetail wageConfigDetail)
        {
            ApiResponseModel<WageConfigDetail> apiResponse = new ApiResponseModel<WageConfigDetail>();
            // Check if the provided wageConfigDetail is null or the id doesn't match the Wage_Id
            if (id <= 0 || wageConfigDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            wageConfigDetail.WageConfig_Dtl_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteWageConfigDetail, wageConfigDetail);
            apiResponse.IsSuccess = wageConfigDetail.MessageType == 1;
            apiResponse.Message = wageConfigDetail.StatusMessage;
            apiResponse.MessageType = wageConfigDetail.MessageType;
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
