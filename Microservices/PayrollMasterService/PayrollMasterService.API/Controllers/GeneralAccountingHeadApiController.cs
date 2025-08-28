/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-1020                                                                 *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for GeneralAccounting entries.                          *
 *  It includes APIs to retrieve, create, update, and delete GeneralAccounting                      *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllGeneralAccountingHead : Retrieves all GeneralAccountingHead records.                    *
 *  - GetGeneralAccountingHeadById: Retrieves a specific GeneralAccountingHeadrecord by ID.         *
 *  - PostGeneralAccountingHead   : Adds a new GeneralAccountingHead record.                        *
 *  - PutGeneralAccountingHead    : Updates an existing GeneralAccountingHead record.               *
 *  - DeleteGeneralAccountingHead : Soft-deletes an GeneralAccountingHead record.                   *
 *                                                                                                  *
 *  Author: HARSHIDA PARMAR                                                                         *
 *  Date  : 25-June-2025                                                                            *
 *                                                                                                  *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Requests;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class GeneralAccountingHeadApiController : ControllerBase
    {
        private readonly IGeneralAccountingHeadRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;

        public GeneralAccountingHeadApiController(IGeneralAccountingHeadRepository repository, ApiKeyValidatorHelper apiKeyValidatorHelper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidatorHelper;
        }

        #region General Accounting Head CRUD APIs

        [HttpGet("getallgeneralaccountinghead")]
        public async Task<IActionResult> GetAllAccountingHeads([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<AccountingHeadRequest>>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            var result = await _repository.GetAllAsync(DbConstants.GetGeneralAccountMaster);

            if (result != null && result.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = result;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }

            apiResponse.IsSuccess = false;
            apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
            return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
        }

        [HttpGet("getgeneralaccountingheadbyid/{id}")]
        public async Task<IActionResult> GetAccountingHeadById([FromHeader(Name = "X-API-KEY")] string apiKey, int id)
        {
            var apiResponse = new ApiResponseModel<AccountingHeadRequest>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            var result = await _repository.GetByIdAsync(DbConstants.GetGeneralAccountMasterById, new { Accounting_Head_Id = id });

            if (result == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = result;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        [HttpPost("postgeneralaccountinghead")]
        public async Task<IActionResult> PostAccountingHead([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] AccountingHeadRequest model)
        {
            var apiResponse = new ApiResponseModel<AccountingHeadRequest>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            if (model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddEditGeneralAccountMaster, model);

            apiResponse.IsSuccess = model.MessageType == 1;
            apiResponse.Result = model;
            apiResponse.Message = model.StatusMessage;
            apiResponse.MessageType = model.MessageType;
            apiResponse.StatusCode = model.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;

            return model.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        [HttpPut("updategeneralaccountinghead/{id}")]
        public async Task<IActionResult> PutAccountingHead([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] AccountingHeadRequest model)
        {
            var apiResponse = new ApiResponseModel<AccountingHeadRequest>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            if (id <= 0 || model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            model.Accounting_Head_Id = id;
            await _repository.UpdateAsync(DbConstants.AddEditGeneralAccountMaster, model);

            apiResponse.IsSuccess = model.MessageType == 1;
            apiResponse.Result = model;
            apiResponse.Message = model.StatusMessage;
            apiResponse.MessageType = model.MessageType;
            apiResponse.StatusCode = model.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;

            return model.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        [HttpDelete("deletegeneralaccountinghead/{id}")]
        public async Task<IActionResult> DeleteAccountingHead([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] AccountingHeadRequest model)
        {
            var apiResponse = new ApiResponseModel<AccountingHeadRequest>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            if (id <= 0 || model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            model.Accounting_Head_Id = id;
            await _repository.DeleteAsync(DbConstants.DeleteGeneralAccountMaster, model);

            apiResponse.IsSuccess = model.MessageType == 1;
            apiResponse.Message = model.StatusMessage;
            apiResponse.MessageType = model.MessageType;
            apiResponse.StatusCode = model.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return model.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }

        #endregion

        #region ACCOUNT TYPE 
        /// <summary>
        /// Developer Name: Harshida Parmar  
        /// Message Detail: API to get all active General Account Types
        /// Created Date: 25-Jun-2025
        /// </summary>
        /// <param name="isActive">Optional: Filter by active status</param>
        /// <returns>List of Account Types</returns>
        [HttpGet("getgeneralaccounttypes")]
        public async Task<IActionResult> GetGeneralAccountTypes([FromQuery] bool? isActive = true)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<AccountType>>();

            var result = await _repository.GetAllAccountTypesAsync(DbConstants.GetGeneralAccountType, new
            {
                AccountType_ID = (int?)null,
                IsActive = isActive
            });

            if (result != null && result.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = result;
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
