/****************************************************************************************************                                                              *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for FormulaMaster entries.                            *
 *  It includes APIs to retrieve, create, update, and delete FormulaMaster                        *
 *  records using the repository pattern and stored procedures and added a Caching Properties.      *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllFormulaMaster : Retrieves all FormulaMaster records.                                *
 *  - GetFormulaMasterById: Retrieves a specific FormulaMaster record by ID.                    *
 *  - PostFormulaMaster   : Adds a new FormulaMaster record.                                    *
 *  - PutFormulaMaster    : Updates an existing FormulaMaster record.                           *
 *  - DeleteFormulaMaster : Soft-deletes an FormulaMaster record.                               *
 *                                                                                                  *
 *  Author: Foram Patel                                                                          *
 *  Date  : 12-Feb-2025                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/

using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
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
    public class FormulaMasterApiController : ControllerBase
       {
        private readonly IFormulaMasterRepository _repository;
        private readonly ICachingServiceRepository _cachingService;
        private const string CacheKey = "FormulaMasterCache"; // Unique key for caching
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        public FormulaMasterApiController(IFormulaMasterRepository repository, ICachingServiceRepository cachingService, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cachingService = cachingService;
            _apiKeyValidatorHelper = apiKeyValidator;
        }
        #region Formula Master Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Foram Patel
        ///  Message detail    :- This API retrieves all Formula details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 12-Feb-2025
        /// Change Date: 16-04-25
        /// Change Detail:  "Add API KEY"
        /// </summary>
        /// <returns>A JSON response with Formula details or an appropriate message</returns>
        [HttpGet("getallformulamaster")]
        public async Task<IActionResult> GetAllFormulaMaster([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            
            var apiResponse = new ApiResponseModel<IEnumerable<FormulaMaster>>();
            #region Validate KEY
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            #endregion
            // Fetching data from the repository by executing the stored procedure
            var forumlaMasters = await _repository.GetAllAsync(DbConstants.GetFormulaMaster);

            // Check if data exists
            if (forumlaMasters != null && forumlaMasters.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = forumlaMasters;
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
        /// Developer Name: Foram Patel
        /// Message Detail: API to retrieve Formula details based on the provided Formula ID. 
        /// This method fetches data from the repository and returns the Formula detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 12-Feb-2025
        /// Change Date: 16-04-25
        /// Change Detail:  "Add API KEY"
        /// </summary>
        /// <param name="id">The ID of the Formula to retrieve</param>
        /// <returns>Returns an API response with Formula details or an error message.</returns>
        [HttpGet("getformulamasterbyid/{id}")]
        public async Task<IActionResult> GetFormulaMasterById([FromHeader(Name = "X-API-KEY")] string apiKey, int id)
        {
            ApiResponseModel<FormulaMaster> apiResponse = new ApiResponseModel<FormulaMaster>();
            #region Validate KEY
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            #endregion
            var formulaMaster = await _repository.GetByIdAsync(DbConstants.GetFormulaMasterById, new { Formula_Id = id });
            if (formulaMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = formulaMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Foram Patel
        ///  Message detail :- This API handles the addition of Formula details based on the provided organization data.
        ///  Created Date   :- 12-Feb-2024
        /// Change Date:- 16-04-25
        /// Change Detail:-  "Add API KEY"
        /// </summary>
        /// <param name="FormulaDetail">Formula detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        /// 
        [HttpPost("postformulamaster")]
        public async Task<IActionResult> PostFormulaMaster([FromHeader(Name = "X-API-KEY")] string apiKey,[FromBody] FormulaMaster FormulaDetail)
        {
            ApiResponseModel<FormulaMaster> apiResponse = new ApiResponseModel<FormulaMaster>();
            #region Validate KEY
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            #endregion
            if (FormulaDetail == null )
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditFormulaMaster, FormulaDetail); 
            apiResponse.Result = FormulaDetail;
            apiResponse.IsSuccess = FormulaDetail.MessageType == 1;
            apiResponse.Message = FormulaDetail.StatusMessage;
            apiResponse.MessageType = FormulaDetail.MessageType;
            apiResponse.StatusCode = FormulaDetail.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return FormulaDetail.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Foram Patel
        ///  Message detail    :- Updates the Formula detail based on the provided FormulaDetail and ID. 
        ///                    If the ID does not match or FormulaDetail is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 12-Feb-2025
        /// Change Date:- 16-04-25
        /// Change Detail:-  "Add API KEY"
        /// </summary>
        /// <param name="id">The ID of the Formula to update.</param>
        /// <param name="FormulaDetail">The Formula detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updateformulamaster/{id}")]
        public async Task<IActionResult> PutFormulaMaster([FromHeader(Name = "X-API-KEY")] string apiKey,int id, [FromBody] FormulaMaster FormulaDetail)
        {
            ApiResponseModel<FormulaMaster> apiResponse = new ApiResponseModel<FormulaMaster>();
            #region Validate KEY
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            #endregion
            // Check if the provided FormulaDetail is null or the id doesn't match the Formula_Id
            if (id <= 0 || FormulaDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            FormulaDetail.Formula_Id = id;
            // Call the UpdateFormulaAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditFormulaMaster, FormulaDetail);
            apiResponse.IsSuccess = FormulaDetail.MessageType == 1;
            apiResponse.Message = FormulaDetail.StatusMessage;
            apiResponse.MessageType = FormulaDetail.MessageType;
            apiResponse.StatusCode = FormulaDetail.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return FormulaDetail.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);

            //apiResponse.IsSuccess = FormulaDetail.MessageType == 1;
            //apiResponse.Message = FormulaDetail.StatusMessage;
            //apiResponse.MessageType = FormulaDetail.MessageType;
            //if (apiResponse.IsSuccess)
            //{
            //    // Remove cache if operation is successful
            //    _cachingService.Remove(CacheKey);
            //    apiResponse.StatusCode = ApiResponseStatusConstant.Created;
            //}
            //else
            //{
            //    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
            //}            // Return response based on success or failure
            //return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Foram Patel
        ///  Message detail    :- Updates the delete column based on the provided FormulaDetail and ID. 
        ///                    If the ID does not match or FormulaDetail is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 12-Feb-2025
        /// Change Date:-       16-04-25
        /// Change Detail:-  "Add API KEY"
        /// </summary>
        /// <param name="id">The ID of the earning deduction to update.</param>
        /// <param name="FormulaDetail">The Formula detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deleteformulamaster/{id}")]
        public async Task<IActionResult> DeleteFormulaMaster([FromHeader(Name = "X-API-KEY")] string apiKey,int id, [FromBody] FormulaMaster FormulaDetail)
        {
            ApiResponseModel<FormulaMaster> apiResponse = new ApiResponseModel<FormulaMaster>();
            #region Validate KEY
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            #endregion
            // Check if the provided FormulaDetail is null or the id doesn't match the Formula_Id
            if (id <= 0 || FormulaDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            FormulaDetail.Formula_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteFormulaMaster, FormulaDetail);
            apiResponse.IsSuccess = FormulaDetail.MessageType == 1;
            apiResponse.Message = FormulaDetail.StatusMessage;
            apiResponse.MessageType = FormulaDetail.MessageType;
            apiResponse.StatusCode = FormulaDetail.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return FormulaDetail.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- Get List of Formula
        ///  Created Date   :- 12-Feb-2025

        [HttpGet("getformulasuggestions")]
        public async Task<IActionResult> GetFormulaSuggestions(string searchParam)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<FormulaMaster>>();
            if (string.IsNullOrEmpty(searchParam))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Search parameter is required.";
                apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                return BadRequest(apiResponse);
            }
            var param = new { Searchparam = searchParam };
            var suggestions = await _repository.GetFormulaSuggestionsAsync(DbConstants.FormulaSuggestionsMaster, param);
            if (suggestions != null && suggestions.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = suggestions;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
            }
        }


        #endregion
    }
}
