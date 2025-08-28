using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.ReportModel;
using PayrollTransactionService.DAL.Interface;
using System.Net;

namespace PayrollTransactionService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PendingApprovalRequestApiController : ControllerBase
    {
        private readonly IPendingApprovalRequestRepository _pendingApprovalRequestRepository;

        public PendingApprovalRequestApiController(IPendingApprovalRequestRepository pendingApprovalRequestRepository)
        {
            _pendingApprovalRequestRepository = pendingApprovalRequestRepository;
        }

        /// <summary>
        /// Retrieves a list of unapproved service requests for a specified company and user.
        /// </summary>
        /// <param name="filterModel">The filter model containing CompanyId and UserId.</param>
        /// <returns>A JSON response with the unapproved requests or a no content status if no records are found.</returns>
        [HttpPost("getUnapprovedRequests")]
        public async Task<IActionResult> GetUnapprovedRequests([FromBody] PendingApprovalRejectionFilter filterModel)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<PendingApprovalRequest>>();

            try
            {
                // Check if the filter model is valid
                if (filterModel == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.NullData;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    return BadRequest(apiResponse);
                }

                var unapprovedRequests = await _pendingApprovalRequestRepository.GetServiceApprovalRejectionsAsync(
                     DbConstants.GetPendingApprovalRequest, filterModel);

                if (unapprovedRequests != null && unapprovedRequests.Any())
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = unapprovedRequests;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = "No unapproved requests found for the specified criteria.";
                    apiResponse.StatusCode = (int)HttpStatusCode.NoContent;
                    return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
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

    }
}
