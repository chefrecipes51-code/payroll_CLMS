using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.ReportModel;
using PayrollTransactionService.DAL.Interface;
using System.Net;

namespace PayrollTransactionService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class PayrollTransactionStagingApiController : ControllerBase
    {
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        private readonly IPayrollTransactionStagingRepository _repository;

        public PayrollTransactionStagingApiController(IPayrollTransactionStagingRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator ?? throw new ArgumentNullException(nameof(apiKeyValidator));
        }
        [HttpPost("save-payroll-staging")]
        public async Task<IActionResult> SavePayrollStagingData(
      [FromHeader(Name = "X-API-KEY")] string apiKey,
      [FromBody] SavePayrollStagingRequestModel request)
        {
            var apiResponse = new ApiResponseModel<PayrollStgData>();

            // Step 1: Validate API Key
            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // Step 2: Input validation
            if (request == null || request.PayrollData == null || !request.PayrollData.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // Step 3: Call repository method
            var result = await _repository.SavePayrollStagingDataAsync(
                        DbConstants.AddPayrollTranStgData,
                        request
                    );


            // Step 4: Prepare response
            apiResponse.Data = result;
            apiResponse.Message = result.StatusMessage;
            apiResponse.MessageType = result.MessageType;
            apiResponse.IsSuccess = result.MessageType == 1;
            apiResponse.StatusCode = result.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            return result.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        [HttpPost("postpayrolltransactionstaging")]
        public async Task<IActionResult> PostPayrollTransactionStaging(
            [FromHeader(Name = "X-API-KEY")] string apiKey,
            [FromBody] PayrollTranStgDataRequest model)
        {
            var apiResponse = new ApiResponseModel<PayrollTranStgDataRequest>();

            // Step 1: Validate API key
            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // Step 2: Validate incoming data
            if (model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // Step 3: Repository call
            await _repository.AddAsync(DbConstants.AddPayrollTransactionStagingData, model); // use your actual SP constant

            // Step 4: Response mapping
            apiResponse.IsSuccess = model.MessageType == 1;
            apiResponse.Message = model.StatusMessage;
            apiResponse.MessageType = model.MessageType;
            apiResponse.Data = model;
            apiResponse.StatusCode = model.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            // Step 5: Return appropriate response
            return model.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : BadRequest(apiResponse);
        }
    }

}
