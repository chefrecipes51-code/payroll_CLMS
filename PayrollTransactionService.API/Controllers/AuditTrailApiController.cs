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
    public class AuditTrailApiController : ControllerBase
    {
        #region Constructor 
        private readonly IAuditTrailRepository _repository;
        public AuditTrailApiController(IAuditTrailRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #endregion
        #region AuditTrail CRUD APIs Functionality
        /// <summary>
        /// Retrieves audit trail records for a specified company within a date range.
        /// </summary>
        /// <param name="companyId">The ID of the company to filter the audit trail records.</param>
        /// <param name="dateFrom">Start date for the audit trail records.</param>
        /// <param name="dateTo">End date for the audit trail records.</param>
        /// <returns>A JSON response with the audit trail records or a no content status if no records found.</returns>
        [HttpPost("getAuditTrails")]
        public async Task<IActionResult> GetAuditTrailsByDateRange([FromBody] AuditTrail requestModel)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<AuditTrail>>();

            try
            {
                // Check if the request model is valid
                if (requestModel == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.NullData;
                    apiResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    return BadRequest(apiResponse);
                }

                // Call the repository to get audit trails based on the model
                var auditTrails = await _repository.GetAuditTrailsByDateRangeAsync(
                    DbConstants.GetAuditTrailByCompanyIDAndDate, requestModel);

                if (auditTrails != null && auditTrails.Any())
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = auditTrails;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = "No audit trails found for the specified criteria.";
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



        /// <summary>
        /// This API retrieves all AuditTrail records from the database.
        /// </summary>
        /// <returns>A JSON response with AuditTrail details or an appropriate message.</returns>
        [HttpGet("getallaudittrails")]
        public async Task<IActionResult> GetAllAuditTrails()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<AuditTrail>>();
            var auditTrails = await _repository.GetAllAsync(DbConstants.GetAllAuditTrails);

            if (auditTrails != null && auditTrails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = auditTrails;
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

        /// <summary>
        /// API to retrieve AuditTrail details by AuditTrail ID.
        /// </summary>
        /// <param name="id">The ID of the AuditTrail to retrieve.</param>
        /// <returns>Returns an API response with AuditTrail details or an error message.</returns>
        [HttpGet("getaudittrailbyid/{id}")]
        public async Task<IActionResult> GetAuditTrailById(int id)
        {
            var apiResponse = new ApiResponseModel<AuditTrail>();
            var auditTrail = await _repository.GetByIdAsync(DbConstants.GetAuditTrailById, new { AuditTrail_Id = id });

            if (auditTrail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = auditTrail;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        /// This API adds an AuditTrail record.
        /// </summary>
        /// <param name="auditTrail">AuditTrail details to be added.</param>
        /// <returns>A JSON response with the result of the operation.</returns>
        [HttpPost("postaudittrail")]
        public async Task<IActionResult> PostAuditTrail([FromBody] AuditTrail auditTrail)
        {
            var apiResponse = new ApiResponseModel<AuditTrail>();

            if (auditTrail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddAuditTrail, auditTrail);

            if (auditTrail.MessageType == 1)
            {
                apiResponse.IsSuccess = true;
                apiResponse.Message = auditTrail.StatusMessage;
                apiResponse.MessageType = auditTrail.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.Created;
                return StatusCode((int)HttpStatusCode.Created, apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = auditTrail.StatusMessage;
                apiResponse.MessageType = auditTrail.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
        }

        /// <summary>
        /// Updates an existing AuditTrail record.
        /// </summary>
        /// <param name="id">The ID of the AuditTrail to update.</param>
        /// <param name="auditTrail">Updated AuditTrail details.</param>
        /// <returns>A JSON response with the result of the update operation.</returns>
        [HttpPut("updateaudittrail/{id}")]
        public async Task<IActionResult> UpdateAuditTrail(int id, [FromBody] AuditTrail auditTrail)
        {
            var apiResponse = new ApiResponseModel<AuditTrail>();

            if (auditTrail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            var existingAuditTrail = await _repository.GetByIdAsync(DbConstants.GetAuditTrailById, new { AuditTrail_Id = id });

            if (existingAuditTrail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            await _repository.UpdateAsync(DbConstants.UpdateAuditTrail, auditTrail);

            if (auditTrail.MessageType == 1)
            {
                apiResponse.IsSuccess = true;
                apiResponse.Message = auditTrail.StatusMessage;
                apiResponse.MessageType = auditTrail.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                return Ok(apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = auditTrail.StatusMessage;
                apiResponse.MessageType = auditTrail.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
        }

        /// <summary>
        /// Deletes an AuditTrail record.
        /// </summary>
        /// <param name="id">The ID of the AuditTrail to delete.</param>
        /// <returns>A JSON response with the result of the delete operation.</returns>
        [HttpDelete("deleteaudittrail/{id}")]
        public async Task<IActionResult> DeleteAuditTrail(int id)
        {
            var apiResponse = new ApiResponseModel<AuditTrail>();

            var existingAuditTrail = await _repository.GetByIdAsync(DbConstants.GetAuditTrailById, new { AuditTrail_Id = id });

            if (existingAuditTrail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            await _repository.DeleteAsync(DbConstants.DeleteAuditTrail, new { AuditTrail_Id = id });

            apiResponse.IsSuccess = true;
            apiResponse.Message = ApiResponseMessageConstant.DeleteSuccessfully;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

    #endregion
    }
}
