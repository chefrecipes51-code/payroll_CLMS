/****************************************************************************************************
 *  This controller handles CRUD operations for WageRateMaster entries.                                 
 *  It includes APIs to retrieve, create, update, and delete WageRateMaster                             
 *  records using the repository pattern and stored procedures.                                    
 *                                                                                                  
 *  Methods:                                                                                       
 *  - GetAllWageRateMaster : Retrieves all WageRateMaster records.                                         
 *  - GetWageRateMasterById: Retrieves a specific WageRateMaster record by ID.                              
 *  - PostWageRateMaster   : Adds a new WageRateMaster record.                                             
 *  - PutWageRateMaster    : Updates an existing WageRateMaster record.                                     
 *  - DeleteWageRateMaster : Soft-deletes an WageRateMaster record.                                         
 *                                                                                                  
 *  Author: Harshida Parmar                                                                    
 *  Date  : 17-Sep-2024                                                                            
 *                                                                                                  
 ****************************************************************************************************/
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.APIKeyManagement.Interface;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using PayrollMasterService.DAL.Service;
using System.Data.SqlClient;
using System.Data;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class WageRateMasterApiController : ControllerBase
    {
        #region Constructor 
        /// <summary>
        /// Initializes a new instance of the <see cref="WageRateMasterApiController"/> class.
        /// </summary>
        /// <param name="repository">The service repository for managing wage rate masters.</param>

        private readonly IWageRateMasterRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        private readonly IConfiguration _configuration; // Inject IConfiguration
        public WageRateMasterApiController(IWageRateMasterRepository repository, IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            // Create database connection
            IDbConnection dbConnection = new SqlConnection(connectionString);
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            IApiKeyService apiKeyService = new ApiKeyService(dbConnection);
            _apiKeyValidatorHelper = new ApiKeyValidatorHelper(apiKeyService);
        }
        #endregion
        #region Wage Rate Master Endpoint Handlers (CRUD)
        #region WageRateMaster Fetch All
        /// <summary>
        /// Retrieves all wage rate master records from the database.
        /// </summary>
        [HttpGet("getallwageratemaster")]
        public async Task<IActionResult> GetAllWageRateMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<WageRateMaster>>();
            try
            {
                // Fetching data from the repository by executing the stored procedure
                var wageRateDetails = await _repository.GetAllAsync(DbConstants.GetWageRateMaster);

                // Check if data exists
                if (wageRateDetails != null && wageRateDetails.Any())
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = wageRateDetails;
                    apiResponse.Message = ApiResponseMessageConstant.FetchAllRecords;
                    apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                }
                else
                {
                    // Handle the case where no data is returned
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = ApiResponseStatusConstant.NoContent;
                }

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message ?? ApiResponseMessageConstant.TechnicalIssue;
                apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion
        #region WageRateMaster Fetch By ID
        /// <summary>
        /// Retrieves a specific wage rate master record by its ID.
        /// </summary>
        /// <param name="id">The ID of the wage rate master record to retrieve.</param>
        [HttpGet("getwageratemasterbyid/{id}")]
        public async Task<IActionResult> GetWageRateMasterById(int id)
        {
            ApiResponseModel<WageRateMaster> apiResponse = new ApiResponseModel<WageRateMaster>();
            try
            {
                var user = await _repository.GetByIdAsync(DbConstants.GetWageRateMasterById, new { WageRate_Id = id });
                if (user == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                    return NotFound(apiResponse);
                }

                apiResponse.IsSuccess = true;
                apiResponse.Result = user;
                apiResponse.Message = ApiResponseMessageConstant.GetRecord;
                apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion
        #region WageRateMaster Add
        /// <summary>
        /// Handles HTTP POST requests to add a wage rate master record.
        /// </summary>
        /// <param name="wageRateDetail">
        /// The wage rate master details sent in the request body as JSON.
        /// </param>
        /// <returns>
        /// An IActionResult indicating the outcome of the operation. It could return a success
        /// status like 200 OK with the created wage rate data, or an error response
        /// if the input is invalid or the operation fails.
        /// </returns>
        [HttpPost("postwageratemaster")]
        public async Task<IActionResult> PostWageRateMaster([FromBody] WageRateMaster wageRateDetail)
        {
            ApiResponseModel<WageRateMaster> apiResponse = new ApiResponseModel<WageRateMaster>();
            if (wageRateDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditWageRateMaster, wageRateDetail);
            apiResponse.IsSuccess = wageRateDetail.MessageType == 1;
            apiResponse.Message = wageRateDetail.StatusMessage;
            apiResponse.MessageType = wageRateDetail.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
        #region WageRateMaster Update
        /// <summary>
        /// Handles HTTP PUT requests to update an existing wage rate master record by ID.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the wage rate master record that needs to be updated.
        /// </param>
        /// <returns>
        /// An IActionResult indicating the outcome of the operation. It could return a success
        /// status like 200 OK with the created wage rate data, or an error response
        /// if the input is invalid or the operation fails.
        /// </returns>
        [HttpPut("updatewageratemaster/{id}")]
        public async Task<IActionResult> PutWageRateMaster(int id, [FromBody] WageRateMaster wageRateDetail)
        {
            ApiResponseModel<WageRateMaster> apiResponse = new ApiResponseModel<WageRateMaster>();
            // Check if the provided wageRateDetail is null or the id doesn't match the Wage_Id
            if (wageRateDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            try
            {
                // Retrieve the existing record based on the id
                var existingWageRate = await _repository.GetByIdAsync(DbConstants.GetWageRateMasterById, new { WageRate_Id = id });

                if (existingWageRate == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                    return NotFound(apiResponse);
                }
                wageRateDetail.CreatedBy = existingWageRate.CreatedBy;
                wageRateDetail.WageRate_Id = existingWageRate.WageRate_Id;
                await _repository.UpdateAsync(DbConstants.AddEditWageRateMaster, wageRateDetail);
                var statusMessage = wageRateDetail.StatusMessage;
                if (wageRateDetail.MessageType == 1)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Message = ApiResponseMessageConstant.UpdateSuccessfully;
                    apiResponse.StatusCode = ApiResponseStatusConstant.Created;
                    return StatusCode((int)HttpStatusCode.Created, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = statusMessage;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return BadRequest(apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion
        #region WageRateMaster Delete
        /// <summary>
        /// Asynchronously deletes a wage rate master entry from the database by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the wage rate master entry to be deleted.</param>
        /// <param name="wageRateDetail">The details of the wage rate master to be deleted, provided in the request body.</param>
        [HttpDelete("deletewageratemaster/{id}")]
        public async Task<IActionResult> DeleteWageRateMaster(int id, [FromBody] WageRateMaster wageRateDetail)
        {
            ApiResponseModel<WageRateMaster> apiResponse = new ApiResponseModel<WageRateMaster>();
            // Check if the provided wageRateDetail is null or the id doesn't match the Wage_Id
            if (id <= 0 || wageRateDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            wageRateDetail.WageRate_Id = id;
            await _repository.DeleteAsync(DbConstants.DeleteWageRateMaster, wageRateDetail);
            apiResponse.IsSuccess = wageRateDetail.MessageType == 1;
            apiResponse.Message = wageRateDetail.StatusMessage;
            apiResponse.MessageType = wageRateDetail.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
        #region WageRateMaster Fetch All FOR TESTING ONLY (Validate API KEY)
        /// <summary>
        /// Retrieves all wage rate master records from the database.
        /// </summary>
        [HttpGet("getallwageratemasterusingapikey")]
        public async Task<IActionResult> GetAllWageRateMasterUsingApiKey([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<WageRateMaster>>();
            try
            {
                var isValid = _apiKeyValidatorHelper.Validate(apiKey);
                if (!isValid)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = "Invalid API Key.";
                    apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                    return Unauthorized(apiResponse);
                }

                // Fetching data from the repository by executing the stored procedure
                var wageRateDetails = await _repository.GetAllAsync(DbConstants.GetWageRateMaster);

                // Check if data exists
                if (wageRateDetails != null && wageRateDetails.Any())
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = wageRateDetails;
                    apiResponse.Message = ApiResponseMessageConstant.FetchAllRecords;
                    apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                }
                else
                {
                    // Handle the case where no data is returned
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = ApiResponseStatusConstant.NoContent;
                }

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message ?? ApiResponseMessageConstant.TechnicalIssue;
                apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion
        #endregion
    }
}
