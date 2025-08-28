using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;
using PayrollTransactionService.DAL.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;

namespace PayrollTransactionService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class PayrollProcessApiController : ControllerBase
    {
        private readonly IPayrollProcessRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        private readonly IServiceProvider _serviceProvider;
        public PayrollProcessApiController(IPayrollProcessRepository repository, ApiKeyValidatorHelper apiKeyValidator,  IServiceProvider serviceProvider)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
            _serviceProvider = serviceProvider;

        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all Contractor Payroll Validation details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 23-June-2025
        ///  Last Modified  :- 23-June-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Contractor Payroll Validation details or an appropriate message</returns>
        [HttpGet("getallcontractorpayrollvalidation")]
        public async Task<IActionResult> GetAllContractorPayrollValidation(
         int company_ID, string month_Yr, bool? isActive)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<ContractorPayrollValidation>>();

            var contractorDetails = await _repository.GetAllContractorPayrollValidationAsync(DbConstants.GetallContractorPayrollValidationDetails, new { Company_ID = company_ID, Month_Yr = month_Yr, IsActive = isActive });

            if (contractorDetails != null && contractorDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = contractorDetails;
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
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all Company Payroll Validation details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 23-June-2025
        ///  Last Modified  :- 23-June-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Comoany Payroll Validation details or an appropriate message</returns>
        [HttpGet("getallcompanypayrollvalidation")]
        public async Task<IActionResult> GetAllCompanyPayrollValidation(
         string month_Yr)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<CompanyPayrollValidation>>();
            //// Validate API key
            //var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            //if (!isValid)
            //{
            //    apiResponse.IsSuccess = false;
            //    apiResponse.Message = "Invalid API Key.";
            //    apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
            //    return Unauthorized(apiResponse);
            //}

            var contractorDetails = await _repository.GetAllCompanyPayrollValidationAsync(DbConstants.GetallCompanyPayrollValidationDetails, new { Month_Yr = month_Yr });

            if (contractorDetails != null && contractorDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = contractorDetails;
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
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all Company Location Payroll Validation details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 23-June-2025
        ///  Last Modified  :- 23-June-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Company Location Payroll Validation details or an appropriate message</returns>
        [HttpGet("getallcompanylocationpayrollvalidation")]
        public async Task<IActionResult> GetAllCompanyLocationPayrollValidation(
          string month_Yr, int company_ID, bool? isActive)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<CompanyLocationPayrollValidation>>();

            var companylocationDetails = await _repository.GetAllCompanyLocationPayrollValidationAsync(DbConstants.GetallCompanyLocationPayrollValidationDetails, new { Month_Yr = month_Yr, Company_ID = company_ID, IsActive = isActive });

            if (companylocationDetails != null && companylocationDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = companylocationDetails;
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
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all WorkOrder Payroll Validation details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 23-June-2025
        ///  Last Modified  :- 23-June-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with WorkOrder Payroll Validation details or an appropriate message</returns>
        [HttpGet("getallworkorderpayrollvalidation")]
        public async Task<IActionResult> GetAllWorkOrderPayrollValidation(
         string month_Yr, int company_ID, bool? isActive)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<WorkOrderPayrollValidation>>();

            var workorderDetails = await _repository.GetAllWorkOrderPayrollValidationAsync(DbConstants.GetallWorkOrderPayrollValidationDetails, new { Month_Yr = month_Yr, Company_ID = company_ID, IsActive = isActive });


            if (workorderDetails != null && workorderDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = workorderDetails;
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
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all Previous Month Year Period_ByCmpId Payroll Validation details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 23-June-2025
        ///  Last Modified  :- 23-June-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Previous Month Year Period_ByCmpId Payroll Validation details or an appropriate message</returns>
        [HttpGet("getallpreviousmonthyearperiodbycmpId")]
        public async Task<IActionResult> GetAllPreviousMonthYearPeriod_ByCmpId(
         int company_ID)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<PreviousMonthYearPeriod>>();

            var workorderDetails = await _repository.GetAllPreviousMonthYearPeriodAsync(DbConstants.GetallPreviousMonthYearPeriodDetails, new { Company_Id = company_ID });


            if (workorderDetails != null && workorderDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = workorderDetails;
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
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of payroll process filter details based on the provided organization data.
        ///  Created Date   :- 24-June-2025
        ///  Change Date    :- 24-June-2025
        ///  Change detail  :- Added a insert functionality.
        /// </summary>
        /// <param name="payrollTransDataForProcess"> payrollTransDataForProcess detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postpayrolltransdataforprocess")]
        public async Task<IActionResult> PostPayrollTransDataForProcess(
       [FromHeader(Name = "X-API-KEY")] string apiKey,
       [FromBody] PayrollTransDataForProcess payrollTransDataForProcess)
        {
            var apiResponse = new ApiResponseModel<PayrollTransDataForProcess>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            if (payrollTransDataForProcess == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddPayrollTransDataForProcessAsync(DbConstants.UpdatePayrollTranStgDataForProcessDetails, payrollTransDataForProcess);

            apiResponse.IsSuccess = payrollTransDataForProcess.MessageType == 1;
            apiResponse.Result = payrollTransDataForProcess;
            apiResponse.Message = payrollTransDataForProcess.StatusMessage;
            apiResponse.MessageType = payrollTransDataForProcess.MessageType;
            apiResponse.StatusCode = payrollTransDataForProcess.MessageType == 1
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            return payrollTransDataForProcess.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : BadRequest(apiResponse);
        }

        [HttpPost("poststartpayrollprocess")]
        public async Task<IActionResult> PostStartPayrollProcess(
     [FromHeader(Name = "X-API-KEY")] string apiKey,
     [FromBody] StartPayrollProcess startPayrollProcess)
        {
            
            var apiResponse = new ApiResponseModel<StartPayrollProcess>();

            // Validate API Key
            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // Validate input model
            if (startPayrollProcess == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            try
            {
                // Call repository method
                var result = await _repository.AddStartPayrollProcessAsync(
            DbConstants.UpdateStartPayrollProcessDetails,
            startPayrollProcess);
                apiResponse.Result = result;
                apiResponse.IsSuccess = startPayrollProcess.MessageType == 1;
                apiResponse.Message = startPayrollProcess.StatusMessage;
                apiResponse.MessageType = startPayrollProcess.MessageType;
                apiResponse.StatusCode = startPayrollProcess.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
                return startPayrollProcess.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);

            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "An error occurred while processing payroll.";
                apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
                apiResponse.MessageType = -1;
                apiResponse.Result = null;

                // Log the exception if logging is configured
                // _logger.LogError(ex, "Error in PostStartPayrollProcess");

                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }


        [HttpPost("postprocesspayrollemployees")]
        public async Task<IActionResult> PostProcessPayrollEmployees(
        [FromHeader(Name = "X-API-KEY")] string apiKey,
        [FromBody] PayrollProcessRequestModel startPayrollProcess)
        {
            var apiResponse = new ApiResponseModel<PayrollProcessResultModel>();

            // Validate API Key
            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // Validate input
            if (startPayrollProcess == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // Call the processing method
            var result = await _repository.ProcessPayrollEmployeesAsync(
                DbConstants.PostProcessPayrollEmployeesDetails, // Stored procedure name
                startPayrollProcess
            );

            // Build API response
            apiResponse.Result = result;
            apiResponse.IsSuccess = result.IsSuccess;
            apiResponse.Message = result.Message;
            apiResponse.StatusCode = result.IsSuccess
                ? ApiResponseStatusConstant.Created
                : ApiResponseStatusConstant.BadRequest;

            return result.IsSuccess
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all WorkOrder Payroll Validation details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 26-June-2025
        ///  Last Modified  :- 26-June-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with WorkOrder Payroll Validation details or an appropriate message</returns>
        [HttpGet("getallpayrollprocessusingsignalR")]
        public async Task<IActionResult> GetAllPayrollProcessusingSignalR(
         int cmp_Id, int month_Id, int year_Id)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<PayrollProcessusingSignalR>>();

            var processDetails = await _repository.GetAllPayrollProcessusingSignalRAsync(DbConstants.GetAllPayrollProcessusingSignalRDetails, new { Cmp_Id = cmp_Id, Month_Id = month_Id, Year_Id = year_Id });

            if (processDetails != null && processDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = processDetails;
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

        [HttpPost("getphasewisecomponentpayrollprocess")]
        public async Task<IActionResult> GetPhaseWiseComponentPayrollProcess(
             [FromHeader(Name = "X-API-KEY")] string apiKey,
        [FromBody] PhaseWiseComponentPayrollProcess model)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<dynamic>>();
            try
            { 
                // Validate API Key
                if (!_apiKeyValidatorHelper.Validate(apiKey))
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = "Invalid API Key.";
                    apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                    return Unauthorized(apiResponse);
                }

                var result = await _repository.GetPhaseWiseComponentPayrollProcessAsync(
                    DbConstants.GetPhaseWiseComponentPayrollProcessDetails, model); // stored proc name constant
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
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

     
    }
}
