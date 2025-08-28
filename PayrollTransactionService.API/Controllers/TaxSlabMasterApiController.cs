using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.DAL.Interface;
using System.Net;

namespace PayrollTransactionService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class TaxSlabMasterApiController : ControllerBase
    {
        private readonly ITaxSlabMasterRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        public TaxSlabMasterApiController(ITaxSlabMasterRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }

        #region Tax Slab Master Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all pay component details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 08-April-2025
        ///  Last Modified  :- 08-April-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with pay component details or an appropriate message</returns>
        [HttpGet("getalltaxslabmaster/{id}")]
        public async Task<IActionResult> GetAllTaxSlabMaster([FromHeader(Name = "X-API-KEY")] string apiKey,int id)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<TaxSlabMaster>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Fetching data from the repository by executing the stored procedure
            var taxSlabMaster = await _repository.GetAllTaxRegimeByCompanyAsync(DbConstants.GetTaxSlabTable, new { Company_ID = id});

            // Check if data exists
            if (taxSlabMaster != null && taxSlabMaster.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = taxSlabMaster;
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
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all pay component details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 08-April-2025
        ///  Last Modified  :- 08-April-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with pay component details or an appropriate message</returns>
        [HttpGet("getalltaxregimemaster")]
        public async Task<IActionResult> GetAllTaxRegimeMaster([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<TaxRegimeMaster>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Fetching data from the repository by executing the stored procedure
            var taxregimeMaster = await _repository.GetAllTaxRegimeAsync(DbConstants.GetTaxRegimeTable);

            // Check if data exists
            if (taxregimeMaster != null && taxregimeMaster.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = taxregimeMaster;
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
        /// Message Detail: API to retrieve pay component details based on the provided pay component ID. 
        /// This method fetches data from the repository and returns the pay component detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 09-APril-2025
        /// Change Date:  09-APril-2025
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the pay component to retrieve</param>
        /// <returns>Returns an API response with pay component details or an error message.</returns>
        [HttpGet("gettaxslabmasterbyid/{id}")]
        public async Task<IActionResult> GetTaxSlabMasterById([FromHeader(Name = "X-API-KEY")] string apiKey, int id, int companyId)
        {
            ApiResponseModel<TaxSlabMaster> apiResponse = new ApiResponseModel<TaxSlabMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            var taxSlabMaster = await _repository.GetByIdAsync(DbConstants.GetTaxSlabTableById, new { YearlyItTableDetail_Id = id, Company_ID = companyId });
            if (taxSlabMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = taxSlabMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of pay component details based on the provided organization data.
        ///  Created Date   :- 09-APril-2025
        ///  Change Date    :- 09-APril-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="taxSlabMaster">pay component detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("posttaxslabmaster")]
        public async Task<IActionResult> PostTaxSlabMaster([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] TaxSlabMaster taxSlabMaster)
        {

            ApiResponseModel<TaxSlabMaster> apiResponse = new ApiResponseModel<TaxSlabMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            if (taxSlabMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddEditTaxSlabTable, taxSlabMaster);
            apiResponse.IsSuccess = taxSlabMaster.MessageType == 1;
            apiResponse.Message = taxSlabMaster.StatusMessage;
            apiResponse.MessageType = taxSlabMaster.MessageType;
            apiResponse.StatusCode = taxSlabMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return taxSlabMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the pay component detail based on the provided taxSlabMaster and ID. 
        ///                    If the ID does not match or taxSlabMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 09-APril-2025
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the pay component to update.</param>
        /// <param name="taxSlabMaster">The pay component detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatetaxslabmaster/{id}")]
        public async Task<IActionResult> PutTaxSlabMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] TaxSlabMaster taxSlabMaster)
        {
            ApiResponseModel<TaxSlabMaster> apiResponse = new ApiResponseModel<TaxSlabMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Check if the provided taxSlabMaster is null or the id doesn't match the EarningDeduction_Id
            if (id <= 0 || taxSlabMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            taxSlabMaster.YearlyItTableDetail_Id = id;
            // Call the UpdateEarningDeductionAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditTaxSlabTable, taxSlabMaster);
            apiResponse.IsSuccess = taxSlabMaster.MessageType == 1;
            apiResponse.Message = taxSlabMaster.StatusMessage;
            apiResponse.MessageType = taxSlabMaster.MessageType;
            apiResponse.StatusCode = taxSlabMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return taxSlabMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided taxSlabMaster and ID. 
        ///                    If the ID does not match or taxSlabMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 09-APril-2025
        ///  Last Updated   :- 09-APril-2025
        /// </summary>
        /// <param name="id">The ID of the pay component to update.</param>
        /// <param name="taxSlabMaster">The pay component detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletetaxslabMaster/{id}")]
        public async Task<IActionResult> DeletetaxSlabMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] TaxSlabMaster taxSlabMaster)
        {
            ApiResponseModel<TaxSlabMaster> apiResponse = new ApiResponseModel<TaxSlabMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Check if the provided taxSlabMaster is null or the id doesn't match the EarningDeduction_Id
            if (id <= 0 || taxSlabMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            taxSlabMaster.YearlyItTableDetail_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteTaxSlabTable, taxSlabMaster);
            apiResponse.IsSuccess = taxSlabMaster.MessageType == 1;
            apiResponse.Message = taxSlabMaster.StatusMessage;
            apiResponse.MessageType = taxSlabMaster.MessageType;
            apiResponse.StatusCode = taxSlabMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return taxSlabMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
    }
}
