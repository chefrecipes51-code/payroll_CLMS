/****************************************************************************************************
 *  Jira Task Ticket :  Payroll-594                                                                 *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for Approval setup.                                     *
 *  It includes APIs to retrieve, create, update, and delete Approval Setup                         *
 *  Author: Chirag Gurjar                                                                           *
 *  Date  : 18-Mar-2025                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ApprovalSetUpApiController : ControllerBase
    {
        private readonly IApprovalSetUpRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        public ApprovalSetUpApiController(IApprovalSetUpRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }

        /// <summary>
        /// Developer Name :- Chirag Gurjar
        /// Message Detail :- API to add Approval Setup
        /// Created Date   :- 18-Mar-2025 
        /// Change Date    :- 
        /// Change Detail  :- 
        /// </summary>
        [HttpPost("postapprovalsetup")]
        public async Task<IActionResult> PostApprovalSetUpMapping([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] ApprovalConfigCommon approvalConfigCommon)
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

            // Validate the incoming request
            if (approvalConfigCommon == null || !approvalConfigCommon.Levels.Any())
            {
                return BadRequest("Invalid request: Missing required Event Auth Mapping details.");
            }

            try
            {
                var result = await _repository.AddApprovalConfigAsync(DbConstants.AddEditApprovalConfig, approvalConfigCommon);
                if (result != null)
                {
                    return Ok(new { message = "Approval Configuration Successfully added.", data = result });
                }

                return BadRequest("Failed to add Approval Configuration.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while processing your request.");
            }
        }

        /// <summary>
        /// Developer Name :- Chirag Gurjar
        /// Message Detail :- API to Get all Event Auth transaction Setup data by primary key id
        /// Created Date   :- 18-Mar-2025 
        /// Change Date    :- 
        /// Change Detail  :- 
        /// </summary>
        /// <param name="id">The Service ID of the approval setup to retrieve</param>
        /// <returns>Returns an API response with Approval SetUp details or an error message.</returns>  
        [HttpGet("getapprovalsetupbyserviceid/{id}")]
        public async Task<IActionResult> GetApprovalSetUpByServiceId(int id)
        {
            ApiResponseModel<IEnumerable<ApprovalSetUpFilter>> apiResponse = new ApiResponseModel<IEnumerable<ApprovalSetUpFilter>>();

            var ApprovalSetUpFilter = await _repository.GetByFilterAttributesAsync(DbConstants.GetApprovalSetUp
                                                                    , new { ServiceID = id });
            if (ApprovalSetUpFilter == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = ApprovalSetUpFilter;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        [HttpGet("getapprovalconfig")]
        public async Task<IActionResult> GetApprovalConfig([FromQuery] int configId)
        {
            ApiResponseModel<ApprovalConfigCommon> apiResponse = new ApiResponseModel<ApprovalConfigCommon>();
            try
            {
                var (approvalConfig, approvalLevel, approvalDetail) = await _repository.GetApprovalConfigDetailsAsync(DbConstants.GetApprovalConfig, configId);
                ApprovalConfigCommon ac = new ApprovalConfigCommon();
                ac.Config = approvalConfig;
                ac.Levels = approvalLevel.ToList();
                ac.Details = approvalDetail.ToList();

                var v = ac;

                if (approvalConfig != null)
                {


                    if (approvalLevel.ToList().Count == 0 || approvalDetail.ToList().Count == 0)
                    {
                        apiResponse.Message = "One or more user additional details are missing.";
                        apiResponse.IsSuccess = false;
                        return Ok(apiResponse);
                    }
                }

                if (approvalConfig == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = ac;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;

                    return Ok(apiResponse);

                    // return StatusCode((int)HttpStatusCode.OK, apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        [HttpGet("getapprovalconfiggrid")]
        public async Task<IActionResult> GetApprovalConfigGrid([FromQuery] int? companyId, [FromQuery] int? locationId, [FromQuery] int? serviceId)
        {
            ApiResponseModel<IEnumerable<ApprovalConfigGrid>> apiResponse = new ApiResponseModel<IEnumerable<ApprovalConfigGrid>>();
            try
            {
                var approvalConfig = await _repository.GetApprovalConfigGridAsync(DbConstants.GetApprovalConfigGrid, companyId, locationId, serviceId);

                if (approvalConfig != null)
                {


                    if (approvalConfig.ToList().Count == 0)
                    {
                        apiResponse.Message = "One or more user additional details are missing.";
                        apiResponse.IsSuccess = false;
                        return Ok(apiResponse);
                    }
                }

                if (approvalConfig == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = approvalConfig;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;

                    return Ok(apiResponse);

                    // return StatusCode((int)HttpStatusCode.OK, apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpDelete("deleteapprovalmatrix/{id}")]
        public async Task<IActionResult> DeleteApprovalMatrix(int id, [FromBody] ApprovalConfigGrid approvalconfig)
        {
            ApiResponseModel<ApprovalConfigGrid> apiResponse = new ApiResponseModel<ApprovalConfigGrid>();
            // Check if the provided Location is null or the id doesn't match the Location_Id
            if (id <= 0 || approvalconfig == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            approvalconfig.ConfigID = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteApprovalMatrix, approvalconfig);
            apiResponse.IsSuccess = approvalconfig.MessageType == 1;
            apiResponse.Message = approvalconfig.StatusMessage;
            apiResponse.MessageType = approvalconfig.MessageType;
            apiResponse.StatusCode = approvalconfig.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return approvalconfig.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        /// Created By:- Harshida Parmar 
        /// Create Date:- 29-05-25
        /// Retrieves approval requests assigned to a specific approver based on the provided filters.
        /// </summary>
        /// <param name="approverId">The ID of the approver for whom the approvals are fetched (required).</param>
        /// <param name="correspondanceId">The correspondence ID associated with the approval request (required).</param>
        /// <param name="requestDate">Optional request date to filter the approvals.</param>

        [HttpGet("get-approvals")]
        public async Task<IActionResult> GetApprovals(
            [FromHeader(Name = "X-API-KEY")] string apiKey,
            [FromQuery] int approverId,
            [FromQuery] int correspondanceId,
            [FromQuery] DateTime? requestDate)
        {
            var apiResponse = new ApiResponseModel<ApprovalListViewModel>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            try
            {
                var parameters = new
                {
                    Approver_ID = approverId,
                    Correspondance_ID = correspondanceId,
                    RequestDate = requestDate
                };

                var result = await _repository.GetApprovalWithSummaryAsync(DbConstants.GetAllApproval, parameters);

                if (result.ApprovalList == null || !result.ApprovalList.Any())
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = "No approvals found.";
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return StatusCode((int)HttpStatusCode.NotFound, apiResponse);
                }

                apiResponse.IsSuccess = true;
                apiResponse.Result = result;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            catch (Exception)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "A technical issue occurred.";
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        [HttpPut("update-approval/{id}")]
        public async Task<IActionResult> UpdateApproval([FromHeader(Name = "X-API-KEY")] string apiKey,int id,[FromBody] ApprovalDetailRequest model)
        {
            var apiResponse = new ApiResponseModel<ApprovalDetailRequest>();

            #region Validate Key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            #endregion

            if (id <= 0 || model == null || id != model.Approval_ID)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid request.";
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            var result = await _repository.UpdateApprovalAsync(DbConstants.UpdateApprovalStatus, model);

            apiResponse.IsSuccess = result.MessageType == 1;
            apiResponse.Message = result.StatusMessage;
            apiResponse.MessageType = result.MessageType;
            apiResponse.StatusCode = result.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;

            return apiResponse.IsSuccess
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

    }
}