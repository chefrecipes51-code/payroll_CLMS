/****************************************************************************************************                                                              *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for ContractorValidation entries.                       *
 *  It includes APIs to retrieve, create, update, and delete ContractorValidation                          *
 *  records using the repository pattern and stored procedures and added a Caching Properties.      *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetContractorValidation : Retrieves Data based on Company ID.                                 *
 *  Author: Harshida Parmar                                                                         *
 *  Date  : 09-06-2025                                                                              *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.ReportModel;
using PayrollTransactionService.DAL.Interface;
using PayrollTransactionService.DAL.Service;
using System.Net;


namespace PayrollTransactionService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ContractorValidationApiController : ControllerBase
    {       
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        private readonly IContractorValidationRepository _repository;
        public ContractorValidationApiController(IContractorValidationRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {           
            _apiKeyValidatorHelper = apiKeyValidator;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        /// <summary>
        /// Created By :- HArshida PArmar
        /// Date:- 10-06-25
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpPost("getcontractorvalidation")]
        public async Task<IActionResult> GetContractorValidation(
    [FromHeader(Name = "X-API-KEY")] string apiKey,
    [FromBody] ContractorValidationRequestModel requestModel)
        {
            ApiResponseModel<IEnumerable<ContractorValidationRequest>> apiResponse = new();

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

            var result = await _repository.GetContractorValidationAsync(
                DbConstants.GetValidateMissingContractorData,
                requestModel.CompanyId,
                requestModel.LocationIds,
                //requestModel.ContractorIds,
                requestModel.WorkOrderIds
            );

            if (result == null || !result.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return Ok(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = result;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }


        /// <summary>
        /// Created By :- HArshida PArmar
        /// Date:- 10-06-25
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("getentityvalidation")]
        public async Task<IActionResult> GetEntityValidation(
             [FromHeader(Name = "X-API-KEY")] string apiKey,
             [FromBody] EntityValidationRequestModel requestModel)
        {
            ApiResponseModel<IEnumerable<EntityValidationRequest>> apiResponse = new();

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

            var result = await _repository.GetEntityValidationAsync(
                DbConstants.GetValidateMissingEntityData,
                requestModel.CompanyId,
                requestModel.ContractorIds
            );

            if (result == null || !result.Any())
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

        /// <summary>
        /// Created By :- HArshida PArmar
        /// Date:- 11-06-25
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPut("validate-contractors")]
        public async Task<IActionResult> UpdateValidateContractors(
                                        [FromHeader(Name = "X-API-KEY")] string apiKey,
                                        [FromBody] ValidateContractorRequest request)
        {
            var apiResponse = new ApiResponseModel<ValidateContractorRequest>();

            // 1. Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // 2. Validate input
            if (request == null || request.CompanyId <= 0 || request.ContractorIds == null || !request.ContractorIds.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // 3. Call repository method
            var updatedResult = await _repository.UpdateValidateContractorsAsync(DbConstants.UpdateValidateMissingContractor, request);

            // 4. Prepare response
            apiResponse.Data = updatedResult;
            apiResponse.IsSuccess = updatedResult.MessageType == 1;
            apiResponse.Message = updatedResult.StatusMessage;
            apiResponse.MessageType = updatedResult.MessageType;
            apiResponse.StatusCode = updatedResult.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            // 5. Return proper status code
            return updatedResult.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }


        #region Entity 
        /// <summary>
        /// Created By :- Harshida Parmar  
        /// Date:- 11-06-25  
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPut("validate-entities")]
        public async Task<IActionResult> UpdateValidateEntities(
            [FromHeader(Name = "X-API-KEY")] string apiKey,
            [FromBody] EntityUpdateRequest requestModel)
        {
            var apiResponse = new ApiResponseModel<EntityUpdateRequest>();

            // 1. Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // 2. Validate input
            if (requestModel == null || requestModel.CompanyId <= 0 || requestModel.EntityUpdateList == null || !requestModel.EntityUpdateList.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // 3. Call repository method
            var updatedResult = await _repository.UpdateValidateEntitiesAsync(DbConstants.UpdateMissingEntity, requestModel);

            // 4. Prepare response
            apiResponse.Data = updatedResult;
            apiResponse.IsSuccess = updatedResult.MessageType == 1;
            apiResponse.Message = updatedResult.StatusMessage;
            apiResponse.MessageType = updatedResult.MessageType;
            apiResponse.StatusCode = updatedResult.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            // 5. Return proper status code
            return updatedResult.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }


        #endregion

        #region Pay Calculation
        /// <summary>
        /// Created By :- Harshida Parmar
        /// Date :- 10-06-25
        /// Description :- Get Entity Pay Structure Validation Data
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("getentitypayvalidation")]
        public async Task<IActionResult> GetEntityPayValidation(
            [FromHeader(Name = "X-API-KEY")] string apiKey,
            [FromBody] EntityPayValidationRequestModel requestModel)
        {
            ApiResponseModel<IEnumerable<EntityPayValidationRequest>> apiResponse = new();

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

            var result = await _repository.GetEntityPayValidationAsync(
                DbConstants.GetValidateMissingPayData, 
                requestModel.CompanyId,
                requestModel.EntityIds);

            if (result == null || !result.Any())
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


        [HttpPut("validate-pay-structure")]
        public async Task<IActionResult> UpdateValidatePayCalculation(
            [FromHeader(Name = "X-API-KEY")] string apiKey,
            [FromBody] PayCalculationUpdateRequest request)
        {
            var apiResponse = new ApiResponseModel<PayCalculationUpdateRequest>();

            // 1. Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // 2. Validate input
            if (request == null || request.CompanyId <= 0 || request.EntityStructureUpdateList == null || !request.EntityStructureUpdateList.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // 3. Call repository method
            var updatedResult = await _repository.UpdateValidatePayCalcAsync(DbConstants.UpdateMissingPayCal, request);

            // 4. Prepare response
            apiResponse.Data = updatedResult;
            apiResponse.IsSuccess = updatedResult.MessageType == 1;
            apiResponse.Message = updatedResult.StatusMessage;
            apiResponse.MessageType = updatedResult.MessageType;
            apiResponse.StatusCode = updatedResult.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            // 5. Return proper status code
            return updatedResult.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        #endregion

        #region Compliance
        [HttpPost("getentitycompliance")]
        public async Task<IActionResult> GetEntityComplianceValidation(
        [FromHeader(Name = "X-API-KEY")] string apiKey,
        [FromBody] ComplianceValidationRequest requestModel)
            {
                var apiResponse = new ApiResponseModel<IEnumerable<EntityComplianceValidationRequest>>();

                #region Validate API Key
                var isValid = _apiKeyValidatorHelper.Validate(apiKey);
                if (!isValid)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = "Invalid API Key.";
                    apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                    return Unauthorized(apiResponse);
                }
                #endregion

                #region Input Validation
                if (requestModel == null || requestModel.CompanyId <= 0 || requestModel.EntityIds == null || !requestModel.EntityIds.Any())
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return BadRequest(apiResponse);
                }
                #endregion

                // Call the repository
                var result = await _repository.GetEntityComplianceValidationAsync(
                    DbConstants.GetValidateMissingComplianceData, 
                    (byte)requestModel.CompanyId,
                    requestModel.EntityIds
                );

                if (result == null || !result.Any())
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


        /// <summary>
        /// Created By :- Harshida Parmar
        /// Date:- 2006-25
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPut("validate-entity-compliance")]
        public async Task<IActionResult> UpdateEntityCompliance(
            [FromHeader(Name = "X-API-KEY")] string apiKey,
            [FromBody] EntityComplianceUpdateRequest requestModel)
        {
            var apiResponse = new ApiResponseModel<EntityComplianceUpdateRequest>();

            // 1. Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // 2. Validate input
            if (requestModel == null || requestModel.CompanyId <= 0 || requestModel.EntityComplianceUpdateList == null || !requestModel.EntityComplianceUpdateList.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // 3. Call repository method
            var updatedResult = await _repository.UpdateValidateComplianceAsync(DbConstants.UpdateValidateMissingComplianceData, requestModel);

            // 4. Prepare response
            apiResponse.Data = updatedResult;
            apiResponse.IsSuccess = updatedResult.MessageType == 1;
            apiResponse.Message = updatedResult.StatusMessage;
            apiResponse.MessageType = updatedResult.MessageType;
            apiResponse.StatusCode = updatedResult.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            // 5. Return proper status code
            return updatedResult.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        #endregion

        #region Attendance
        /// <summary>
        /// Created By :- Harshida Parmar
        /// Date :- 20-06-25
        /// Description :- Get Entity Attendance & Work Data
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("getentityattendance")]
        public async Task<IActionResult> GetEntityAttendanceData(
            [FromHeader(Name = "X-API-KEY")] string apiKey,
            [FromBody] ComplianceValidationRequest requestModel)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<EntityAttendanceRequest>>();

            #region Validate API Key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            #endregion

            #region Input Validation
            if (requestModel == null || requestModel.CompanyId <= 0 || requestModel.EntityIds == null || !requestModel.EntityIds.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            #endregion

            var result = await _repository.GetEntityAttendanceValidationAsync(
                DbConstants.GetValidateMissingAttendanceData,
                (byte)requestModel.CompanyId,
                requestModel.EntityIds,  
                requestModel.PayrollMonth, 
                requestModel.PayrollYear);

            if (result == null || !result.Any())
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

        /// <summary>
        /// Created By :- Harshida Parmar  
        /// Date:- 11-06-2025  
        /// Description :- Update Attendance Validation Data
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPut("validate-entity-attendance")]
        public async Task<IActionResult> UpdateEntityAttendance(
            [FromHeader(Name = "X-API-KEY")] string apiKey,
            [FromBody] EntityAttendanceUpdateRequest requestModel)
        {
            var apiResponse = new ApiResponseModel<EntityAttendanceUpdateRequest>();

            // 1. Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // 2. Validate input
            if (requestModel == null || requestModel.CompanyId <= 0 || requestModel.EntityAttendanceIds == null || !requestModel.EntityAttendanceIds.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // 3. Call repository method
            var updatedResult = await _repository.UpdateValidateAttendanceAsync(
                DbConstants.UpdateValidateMissingAttendanceData,
                requestModel);

            // 4. Prepare response
            apiResponse.Data = updatedResult;
            apiResponse.IsSuccess = updatedResult.MessageType == 1;
            apiResponse.Message = updatedResult.StatusMessage;
            apiResponse.MessageType = updatedResult.MessageType;
            apiResponse.StatusCode = updatedResult.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            // 5. Return appropriate response
            return updatedResult.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        #endregion
        [HttpGet("get-previous-period")]
        public async Task<IActionResult> GetPreviousMonthYearPeriod(
        [FromHeader(Name = "X-API-KEY")] string apiKey,
        [FromQuery] int companyId)
        {
            var apiResponse = new ApiResponseModel<CompanyPreviousMonthYearRequest>();

            // Validate API Key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // Input Validation
            if (companyId <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // Get Data from Repository
            var result = await _repository.GetPreviousMonthYearPeriodByCompanyIdAsync(DbConstants.GetPreviousMonthYearCompany, companyId);

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
    }
}
