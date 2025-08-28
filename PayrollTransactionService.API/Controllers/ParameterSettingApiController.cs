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
    public class ParameterSettingApiController : ControllerBase
    {
        #region CTOR
        private readonly IParameterRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        public ParameterSettingApiController(IParameterRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }
        #endregion

        #region Copy From One Company to Another Company 
        [HttpPost("copy-payroll-parameter")]
        public async Task<IActionResult> CopyPayrollParameter(
                [FromHeader(Name = "X-API-KEY")] string apiKey,
                [FromBody] CopySettingsRequest request)
        {
            var apiResponse = new ApiResponseModel<CopySettingsRequest>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            if (request == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.CopyPayrollParameterAsync(DbConstants.PostOneCompanyToAnotherCompany, request);

            apiResponse.IsSuccess = request.MessageType == 1;
            apiResponse.Message = request.StatusMessage;
            apiResponse.MessageType = request.MessageType;
            apiResponse.StatusCode = request.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;

            return request.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        #endregion

        #region PayrollGlobalParamRequest CRUD
        [HttpGet("get-all-entity-type")]
        public async Task<IActionResult> GetAllEntityTypes()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<EntityTypeRequest>>();

            // 1. Validate API Key
            //var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            //if (!isValid)
            //{
            //    apiResponse.IsSuccess = false;
            //    apiResponse.Message = "Invalid API Key.";
            //    apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
            //    return Unauthorized(apiResponse);
            //}

            // 2. Call repository method with stored procedure name
            var result = await _repository.GetAllEntityTypeRequestAsync(DbConstants.GetAllEntityType); 

            // 3. Build response
            if (result != null && result.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = result;
                apiResponse.Message = ApiResponseMessageConstant.GetRecord;
                apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
            }

            return Ok(apiResponse);
        }

        [HttpGet("get-global-payroll-parameters")]
        public async Task<IActionResult> GetGlobalPayrollParameters(
    [FromHeader(Name = "X-API-KEY")] string apiKey,
    int companyId)
        {
            var apiResponse = new ApiResponseModel<PayrollSettingsWrapper>();

            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            var parameters = new { Company_id = companyId };
            var data = await _repository.GetGlobalPayrollParametersAsync(DbConstants.GetPayrollGlobal, parameters);

            if (data != null &&
                (data.GlobalParams != null ||
                 data.Settings != null ||
                 data.Compliances != null ||
                 data.ThirdPartyParams != null))
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = data;
                apiResponse.Message = ApiResponseMessageConstant.GetRecord;
                apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                return Ok(apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return Ok(apiResponse);
            }
        }

        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- This API handles the Adds payroll global settings for a company..
        ///  Created Date   :- 13-05-2025
        ///  Change Date    :- 
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="apiKey">API key passed in the request header for validation.</param>
        /// <param name="request">The payroll global settings request model.</param>
        /// <returns>Returns success or failure response with status message.</returns>
        [HttpPost("postpayrollglobal")]
        public async Task<IActionResult> PostPayrollGlobal(
            [FromHeader(Name = "X-API-KEY")] string apiKey,
            [FromBody] PayrollGlobalParamRequest request)
        {
            ApiResponseModel<PayrollGlobalParamRequest> apiResponse = new ApiResponseModel<PayrollGlobalParamRequest>();

            // Validate API Key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Validate Model
            if (request == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            // Call Repository Method
            await _repository.AddGlobalParamAsync(DbConstants.AddEditPayrollGlobal, request);

            apiResponse.IsSuccess = request.MessageType == 1;
            apiResponse.Message = request.StatusMessage;
            apiResponse.MessageType = request.MessageType;
            apiResponse.StatusCode = request.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;

            return request.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        [HttpPut("updateglobalparam/{id}")]
        public async Task<IActionResult> PutGlobalParam(
                [FromHeader(Name = "X-API-KEY")] string apiKey,
                int id,
                [FromBody] PayrollGlobalParamRequest request)
        {
            var apiResponse = new ApiResponseModel<PayrollGlobalParamRequest>();

            // Validate API Key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // Validate input
            if (id <= 0 || request == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // Set the ID in the model
            request.Global_Param_ID = id;

            // Call the repository method
            await _repository.UpdateGlobalParamAsync(DbConstants.AddEditPayrollGlobal, request);

            // Prepare response
            apiResponse.IsSuccess = request.MessageType == 1;
            apiResponse.Message = request.StatusMessage;
            apiResponse.MessageType = request.MessageType;
            apiResponse.StatusCode = request.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            return request.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        #endregion

        #region PayrollSetting CRUD
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- This API handles the addition of PayrollSetting details based on the provided  data.
        ///  Created Date   :- 13-05-2025
        ///  Change Date    :- 
        ///  Change detail  :- Not yet modified.     
        /// </summary>
        /// <param name="apiKey">The API key provided in the request header for authentication.</param>
        /// <param name="request">The payroll setting data to be inserted.</param>
        /// </summary>
        [HttpPost("postpayrollsetting")]
        public async Task<IActionResult> PostPayrollSetting(
                        [FromHeader(Name = "X-API-KEY")] string apiKey,
                        [FromBody] PayrollSettingRequest request)
        {
            var apiResponse = new ApiResponseModel<PayrollSettingRequest>();

            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            if (request == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddPayrollSettingAsync(DbConstants.AddEditPayrollSetting, request);
            apiResponse.IsSuccess = request.MessageType == 1;
            apiResponse.Message = request.StatusMessage;
            apiResponse.MessageType = request.MessageType;
            apiResponse.StatusCode = request.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;

            return request.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        [HttpPut("updatepayrollsetting/{id}")]
        public async Task<IActionResult> PutPayrollSetting(
                        [FromHeader(Name = "X-API-KEY")] string apiKey,
                        int id,
                        [FromBody] PayrollSettingRequest request)
        {
            var apiResponse = new ApiResponseModel<PayrollSettingRequest>();

            // Validate API key
            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // Validate input
            if (id <= 0 || request == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // Set ID before update
            request.Payroll_Setin_ID = id;

            // Call the update method
            await _repository.UpdatePayrollSettingAsync(DbConstants.AddEditPayrollSetting, request);

            apiResponse.IsSuccess = request.MessageType == 1;
            apiResponse.Message = request.StatusMessage;
            apiResponse.MessageType = request.MessageType;
            apiResponse.StatusCode = request.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            return request.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        #endregion

        #region PayrollCompliance CRUD
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- This API handles the Adds Adds payroll compliance settings for a company.
        ///  Created Date   :- 13-05-2025
        ///  Change Date    :- 
        ///  Change detail  :- Not yet modified.
        /// .
        /// </summary>
        /// <param name="apiKey">API key for authorization.</param>
        /// <param name="request">PayrollComplianceRequest model.</param>
        /// <returns>Returns the result of the operation.</returns>
        [HttpPost("postpayrollcompliance")]
        public async Task<IActionResult> PostPayrollCompliance(
                        [FromHeader(Name = "X-API-KEY")] string apiKey,
                        [FromBody] PayrollComplianceRequest request)
        {
            var apiResponse = new ApiResponseModel<PayrollComplianceRequest>();

            // Validate API Key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // Validate Input
            if (request == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // Call repository method
            await _repository.AddPayrollComplianceAsync(DbConstants.AddEditPayrollCompliance, request);

            apiResponse.IsSuccess = request.MessageType == 1;
            apiResponse.Message = request.StatusMessage;
            apiResponse.MessageType = request.MessageType;
            apiResponse.StatusCode = request.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;

            return request.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Created Date   :-  13-05-2025
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        [HttpPut("updatepayrollcompliance/{id}")]
        public async Task<IActionResult> PutPayrollCompliance(
                        [FromHeader(Name = "X-API-KEY")] string apiKey,
                        int id,
                        [FromBody] PayrollComplianceRequest request)
        {
            var apiResponse = new ApiResponseModel<PayrollComplianceRequest>();
            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            if (id <= 0 || request == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            request.Prm_Comlliance_ID = id;
            await _repository.UpdatePayrollComplianceAsync(DbConstants.AddEditPayrollCompliance, request);
            apiResponse.IsSuccess = request.MessageType == 1;
            apiResponse.Message = request.StatusMessage;
            apiResponse.MessageType = request.MessageType;
            apiResponse.StatusCode = request.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            return request.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        #endregion

        #region ThirdPartyParameterRequest CRUD

        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Created Date   :-  13-05-2025
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        [HttpPost("postthirdpartyparameter")]
        public async Task<IActionResult> PostThirdPartyParameter(
            [FromHeader(Name = "X-API-KEY")] string apiKey,
            [FromBody] ThirdPartyParameterRequest request)
        {
            var apiResponse = new ApiResponseModel<ThirdPartyParameterRequest>();

            // Validate API Key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // Validate Input
            if (request == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // Call repository method
            await _repository.AddThirdPartyParameterAsync(DbConstants.AddEditThirdPartyData, request);

            // Set response
            apiResponse.IsSuccess = request.MessageType == 1;
            apiResponse.Message = request.StatusMessage;
            apiResponse.MessageType = request.MessageType;
            apiResponse.StatusCode = request.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            return request.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }


        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Created Date   :-  13-05-2025
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        [HttpPut("updatethirdpartyparameter/{id}")]
        public async Task<IActionResult> PutThirdPartyParameter(
                    [FromHeader(Name = "X-API-KEY")] string apiKey,
                    int id,
                    [FromBody] ThirdPartyParameterRequest request)
        {
            var apiResponse = new ApiResponseModel<ThirdPartyParameterRequest>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            if (id <= 0 || request == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            request.Clms_Param_ID = id;

            await _repository.UpdateThirdPartyParameterAsync(DbConstants.AddEditThirdPartyData, request);

            apiResponse.IsSuccess = request.MessageType == 1;
            apiResponse.Message = request.StatusMessage;
            apiResponse.MessageType = request.MessageType;
            apiResponse.StatusCode = request.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            return request.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        #endregion
    }
}