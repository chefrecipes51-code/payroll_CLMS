/****************************************************************************************************
 *  This controller handles CRUD operations for PayPeriod entries.                                 
 *  It includes APIs to retrieve, create, update, and delete PayPeriod                             
 *  records using the repository pattern and stored procedures.                                    
 *                                                                                                  
 *  Methods:                                                                                       
 *  - GetAllPayPeriod : Retrieves all PayPeriod records.                                         
 *  - GetPayPeriodMasterById: Retrieves a specific PayPeriod record by ID.                              
 *  - PostPayPeriodMaster   : Adds a new PayPeriod record.                                             
 *  - PutPayPeriodMaster    : Updates an existing PayPeriod record.                                     
 *  - DeletePayPeriodMaster : Soft-deletes an PayPeriod record.                                         
 *                                                                                                  
 *  Author: Harshida Parmar                                                                    
 *  Date  : 08-04-2025                                                                            
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
    public class PayMonthApiController : ControllerBase
    {
        private readonly IPayPeriodRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;

        public PayMonthApiController(IPayPeriodRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }
        #region Pay Monthly Crud APIs Functionality
        [HttpGet("getallpaymonthwithsdate")]
        public async Task<IActionResult> GetAllPayMonthWithSDate(
       [FromHeader(Name = "X-API-KEY")] string apiKey,
       [FromQuery] int id,
       [FromQuery] int createdBy,
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
                    CreatedBy = createdBy
                };

                var (payPeriods, errorMessage) = await _repository.GetPayPeriodsByCompanyIdAndSDateAsync(DbConstants.AddEditPayMonth, param);

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

        //[HttpGet("getallpaymonthwithsdate")]
        //public async Task<IActionResult> GetAllPayMonthWithSDate(
        //       [FromHeader(Name = "X-API-KEY")] string apiKey,
        //       [FromQuery] int id,
        //       [FromQuery] int createdBy,
        //       [FromQuery] DateTime? fGroupDate = null
        //)
        //{
        //    var apiResponse = new ApiResponseModel<IEnumerable<PeriodRequest>>();

        //    try
        //    {
        //        if (!_apiKeyValidatorHelper.Validate(apiKey))
        //        {
        //            apiResponse.IsSuccess = false;
        //            apiResponse.Message = "Invalid API Key.";
        //            apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
        //            return Unauthorized(apiResponse);
        //        }

        //        var param = new
        //        {
        //            CompanyId = id,
        //            FromDate = fGroupDate,
        //            IsGet = 1,
        //            CreatedBy= createdBy
        //        };

        //        var payPeriods = await _repository.GetPayPeriodsByCompanyIdAndSDateAsync(DbConstants.AddEditPayMonth, param);

        //        if (payPeriods == null || !payPeriods.Any())
        //        {
        //            apiResponse.IsSuccess = false;
        //            apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
        //            apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
        //            return NotFound(apiResponse);
        //        }
        //        apiResponse.IsSuccess = true;
        //        apiResponse.Result = payPeriods;
        //        apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
        //        return Ok(apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResponse.IsSuccess = false;
        //        apiResponse.Message = ex.Message;
        //        apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
        //        return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
        //    }
        //}


        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- This API retrieves all Pay Month details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 08-04-25
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Pay Month details or an appropriate message</returns>

        //public async Task<IActionResult> GetAllPayMonth([FromHeader(Name = "X-API-KEY")] string apiKey,int id)
        [HttpGet("getallpaymonth")]
        public async Task<IActionResult> GetAllPayMonth(
                                    [FromHeader(Name = "X-API-KEY")] string apiKey,
                                    [FromQuery] int id,
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
                var payMonthDetails = await _repository.GetPayPeriodsByCompanyIdAsync(DbConstants.GetPayMonthMaster, new { Company_Id = id, FGroupDate= fGroupDate });
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
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- This API handles the addition of Pay Month .
        ///  Created Date   :- 08-04-2025
        ///  Change Date    :- 
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="payMonthDetails"> pay Month detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postpaymaster")]
        public async Task<IActionResult> PostPayMonth([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] PeriodRequest payMonthDetails)
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

                if (payMonthDetails == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.NullData;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return Ok(apiResponse);
                }
                await _repository.AddAsync(DbConstants.AddEditPayMonth, payMonthDetails);
                if (payMonthDetails.MessageType == 1)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Message = payMonthDetails.StatusMessage;
                    apiResponse.MessageType = payMonthDetails.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.Created;
                    return StatusCode((int)HttpStatusCode.Created, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = payMonthDetails.StatusMessage;
                    apiResponse.MessageType = payMonthDetails.MessageType;
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
