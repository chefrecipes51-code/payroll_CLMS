/****************************************************************************************************
 *  This controller handles CRUD operations for Attendance PayPeriod entries.                                 
 *  It includes APIs to retrieve, create, update, and delete attendance PayPeriod                             
 *  records using the repository pattern and stored procedures.                                    
 *                                                                                                  
 *  Methods:                                                                                       
 *  - GetAllAttendancePayPeriod : Retrieves all PayPeriod records contractor wise.                                         
 *  - GetAttendancePayPeriodMasterById: Retrieves a specific Attendance Contractor wise PayPeriod record by ID.                              
 *  - PostAttendancePayPeriodMaster   : Adds a new  Attendance PayPeriod record for contractor.                                             
 *  - PutAttendancePayPeriodMaster    : Updates an existing Attendance PayPeriod record.                                     
 *  - DeleteAttendancePayPeriodMaster : Soft-deletes an Attendance PayPeriod record.                                         
 *                                                                                                  
 *  Author: Abhishek Yadav                                                                    
 *  Date  : 26-05-2025                                                                          
 *                                                                                                  
 ****************************************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.APIKeyManagement.Interface;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.ReportModel;
using PayrollTransactionService.DAL.Interface;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net;

namespace PayrollTransactionService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class AttendancePayMonthApiController : ControllerBase
    {
        private readonly IAttendancePayPeriodRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;

        public AttendancePayMonthApiController(IAttendancePayPeriodRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }
        #region Pay Monthly Crud APIs Functionality
        [HttpGet("getallattendancepaymonthwithsdate")]
        public async Task<IActionResult> GetAllAttendancePayMonthWithSDate(
       [FromHeader(Name = "X-API-KEY")] string apiKey,
       [FromQuery] int id,
       [FromQuery] int createdBy,
       [FromQuery] string Contractor_Code,
       [FromQuery] string WorkOrder_Code,
       [FromQuery] DateTime? fGroupDate = null
)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<PeriodRequest>>();

            try
            {
                if (!_apiKeyValidatorHelper.Validate(apiKey))
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = "Invalid API Key.";
                    apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                    return Unauthorized(apiResponse);
                }

                var param = new
                {
                    CompanyId = id,
                    FromDate = fGroupDate,
                    IsGet = 1,
                    CreatedBy = createdBy,
                    Contractor_Code = Contractor_Code,
                    WorkOrder_Code = WorkOrder_Code, 
                };

                var (payPeriods, errorMessage) = await _repository.GetAttendancePayPeriodsByCompanyIdAndSDateAsync(DbConstants.AddEditAttendancePayMonth, param);

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    // This means SP returned a custom message instead of JSON
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = errorMessage;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return Ok(apiResponse);
                }

                if (payPeriods == null || !payPeriods.Any())
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                    return Ok(apiResponse);
                }

                apiResponse.IsSuccess = true;
                apiResponse.Result = payPeriods;
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
        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail    :- This API retrieves all Attenance Pay Month details Contractor wise from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 26-05-2025
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Attendance Pay Month details or an appropriate message</returns>

        //public async Task<IActionResult> GetAllAttendancePayMonth([FromHeader(Name = "X-API-KEY")] string apiKey,int id)
        [HttpGet("GetAllAttendancePayMonth")]
        public async Task<IActionResult> GetAllAttendancePayMonth(
                                    [FromHeader(Name = "X-API-KEY")] string apiKey,
                                    [FromQuery] int id,
                                    [FromQuery] string Contractor_Code,
                                    [FromQuery] string WorkOrder_Code,
                                    [FromQuery] string? fGroupDate = null)      
        {
			var apiResponse = new ApiResponseModel<IEnumerable<PeriodRequest>>();

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
                var payMonthDetails = await _repository.GetAttendancePayPeriodsByCompanyIdAsync(DbConstants.GetAttendancePayMonthMaster, new { Company_Id = id, FGroupDate= fGroupDate, Contractor_Code = Contractor_Code, WorkOrder_Code=WorkOrder_Code });
                if (payMonthDetails == null || !payMonthDetails.Any())
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                    return NotFound(apiResponse);
                }
                if (string.IsNullOrEmpty(fGroupDate))
                {
                    var filteredResult = payMonthDetails
                        .GroupBy(p => p.FYearDate)
                        .Select(g => g.First()) 
                        .ToList();

                    apiResponse.IsSuccess = true;
                    apiResponse.Result = filteredResult;
                }
                else
                {
                    // return full list as-is
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = payMonthDetails;
                }

                apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
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

        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API handles the addition of Attendance Pay Month .
        ///  Created Date   :- 26-05-2025
        ///  Change Date    :- 
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="AttendancepayMonthDetails"> pay Month detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postattendancepaymaster")]
        public async Task<IActionResult> PostAttendancePayMonth([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] PeriodRequest AttendancepayMonthDetails)
        {
            ApiResponseModel<PeriodRequest> apiResponse = new ApiResponseModel<PeriodRequest>();
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

                if (AttendancepayMonthDetails == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.NullData;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return Ok(apiResponse);
                }
                await _repository.AddAsync(DbConstants.AddEditAttendancePayMonth, AttendancepayMonthDetails);
                if (AttendancepayMonthDetails.MessageType == 1)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Message = AttendancepayMonthDetails.StatusMessage;
                    apiResponse.MessageType = AttendancepayMonthDetails.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.Created;
                    return StatusCode((int)HttpStatusCode.Created, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = AttendancepayMonthDetails.StatusMessage;
                    apiResponse.MessageType = AttendancepayMonthDetails.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return Ok(apiResponse);
                }
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
    }
}
