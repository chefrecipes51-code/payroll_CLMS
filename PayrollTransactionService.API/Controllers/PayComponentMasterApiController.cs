/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-646                                                                  *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for PayComponentMaster entries.                         *
 *  It includes APIs to retrieve, create, update, and delete PayComponentMaster                     *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllPayComponentMaster : Retrieves all PayComponentMaster records.                          *
 *  - GetPayComponentMasterById: Retrieves a specific PayComponentMaster record by ID.              *
 *  - PostPayComponentMaster   : Adds a new PayComponentMaster record.                              *
 *  - PutPayComponentMaster    : Updates an existing PayComponentMaster record.                     *
 *  - DeletePayComponentMaster : Soft-deletes an PayComponentMaster record.                         *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 09-APril-2025                                                                           *
 *                                                                                                  *
 ****************************************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;
using PayrollTransactionService.DAL.Interface;
using System.Net;

namespace PayrollTransactionService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class PayComponentMasterApiController : ControllerBase
    {
        private readonly IPayComponentMasterRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        public PayComponentMasterApiController(IPayComponentMasterRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }

        #region Pay Component Master Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all pay component details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 08-April-2025
        ///  Last Modified  :- 08-April-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with pay component details or an appropriate message</returns>
        [HttpGet("getallpaycomponentmaster/{companyId}")]
        public async Task<IActionResult> GetAllPayComponentMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int companyId, string selectType = "C")
        {
            var apiResponse = new ApiResponseModel<IEnumerable<PayComponentMaster>>();
            //var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            //if (!isValid)
            //{
            //    //apiResponse.IsSuccess = false;
            //    //apiResponse.Message = "Invalid API Key.";
            //    //apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
            //    //return Unauthorized(apiResponse);
            //}
            // Fetching data from the repository by executing the stored procedure
            var payComponentMaster = await _repository.GetAllByIdAsync(DbConstants.GetEarningDeductionMaster, new { Company_Id = companyId, SelectType = selectType });

            // Check if data exists
            if (payComponentMaster != null && payComponentMaster.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = payComponentMaster;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            else
            {
                // Handle the case where no data is returned
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
            }
        }

        /// <summary>
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail    :- This API retrieves all pay component details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 29-may-2025
        ///  Last Modified  :- 29-may-2025        
        /// </summary>
        /// <returns>A JSON response with pay component details or an appropriate message</returns>
        [HttpGet("getpaycomponentchildmaster/{companyId}")]
        public async Task<IActionResult> GetPayComponentChildMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int companyId, string selectType, int EarningDeduction_Id)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<PayComponentMaster>>();
          
            // Fetching data from the repository by executing the stored procedure
            var payComponentMaster = await _repository.GetAllByIdAsync(DbConstants.GetEarningDeductionMaster, new { Company_Id = companyId, SelectType = selectType, EarningDeduction_Id= EarningDeduction_Id });

            // Check if data exists
            if (payComponentMaster != null && payComponentMaster.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = payComponentMaster;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            else
            {
                // Handle the case where no data is returned
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
            }
        }

        /// <summary>
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve pay component details based on the provided pay component ID. 
        /// This method fetches data from the repository and returns the pay component detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 09-APril-2025
        /// Change Date:  09-APril-2025
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the pay component to retrieve</param>
        /// <returns>Returns an API response with pay component details or an error message.</returns>
        [HttpGet("getpaycomponentmasterbyid/{id}")]
        public async Task<IActionResult> GetPayComponentMasterById([FromHeader(Name = "X-API-KEY")] string apiKey, int id, int companyId)
        {
            ApiResponseModel<PayComponentMaster> apiResponse = new ApiResponseModel<PayComponentMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            var payComponentMaster = await _repository.GetByIdAsync(DbConstants.GetEarningDeductionMasterById, new { EarningDeduction_Id = id, Company_Id = companyId });
            if (payComponentMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = payComponentMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of pay component details based on the provided organization data.
        ///  Created Date   :- 09-APril-2025
        ///  Change Date    :- 09-APril-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="payComponentMaster">pay component detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postpaycomponentmaster")]
        public async Task<IActionResult> PostPayComponentMaster([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] PayComponentMaster payComponentMaster)
        {

            ApiResponseModel<PayComponentMaster> apiResponse = new ApiResponseModel<PayComponentMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            if (payComponentMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddEditEarningDeductionMaster, payComponentMaster);
            apiResponse.IsSuccess = payComponentMaster.MessageType == 1;
            apiResponse.Message = payComponentMaster.StatusMessage;
            apiResponse.MessageType = payComponentMaster.MessageType;
            apiResponse.StatusCode = payComponentMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return payComponentMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the pay component detail based on the provided payComponentMaster and ID. 
        ///                    If the ID does not match or payComponentMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 09-APril-2025
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the pay component to update.</param>
        /// <param name="payComponentMaster">The pay component detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatepaycomponentmaster/{id}")]
        public async Task<IActionResult> PutPayComponentMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] PayComponentMaster payComponentMaster)
        {
            ApiResponseModel<PayComponentMaster> apiResponse = new ApiResponseModel<PayComponentMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Check if the provided payComponentMaster is null or the id doesn't match the EarningDeduction_Id
            if (id <= 0 || payComponentMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            payComponentMaster.EarningDeduction_Id = id;
            // Call the UpdateEarningDeductionAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditEarningDeductionMaster, payComponentMaster);
            apiResponse.IsSuccess = payComponentMaster.MessageType == 1;
            apiResponse.Message = payComponentMaster.StatusMessage;
            apiResponse.MessageType = payComponentMaster.MessageType;
            apiResponse.StatusCode = payComponentMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return payComponentMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided payComponentMaster and ID. 
        ///                    If the ID does not match or payComponentMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 09-APril-2025
        ///  Last Updated   :- 09-APril-2025
        /// </summary>
        /// <param name="id">The ID of the pay component to update.</param>
        /// <param name="payComponentMaster">The pay component detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletepaycomponentmaster/{id}")]
        public async Task<IActionResult> DeletePayComponentMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] PayComponentMaster payComponentMaster)
        {
            ApiResponseModel<PayComponentMaster> apiResponse = new ApiResponseModel<PayComponentMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Check if the provided payComponentMaster is null or the id doesn't match the EarningDeduction_Id
            if (id <= 0 || payComponentMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            payComponentMaster.EarningDeduction_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteEarningDeductionMaster, payComponentMaster);
            apiResponse.IsSuccess = payComponentMaster.MessageType == 1;
            apiResponse.Message = payComponentMaster.StatusMessage;
            apiResponse.MessageType = payComponentMaster.MessageType;
            apiResponse.StatusCode = payComponentMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return payComponentMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- This API retrieves all pay component details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 28-April-2025
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with pay component details or an appropriate message</returns>
        [HttpGet("activateallpaycomponent")]
        public async Task<IActionResult> ActivateAllPayComponentMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int companyId,bool? IsActivate)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<PayComponentMaster>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Fetching data from the repository by executing the stored procedure
            var earningDeductionMasters = await _repository.GetAllByIdAsync(DbConstants.GetEarningDeductionMaster, new { Company_Id = companyId, IncludeInactive = IsActivate });

            // Check if data exists
            if (earningDeductionMasters != null && earningDeductionMasters.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = earningDeductionMasters;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            else
            {
                // Handle the case where no data is returned
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
            }
        }

        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- From the Formula Master Using "Include InActive PayComnent" gets
        ///                    checked then Fire the update query based on Model POPUP.
        ///  Created Date   :- 29-April-2025
        ///  Last Updated   :- 
        /// </summary>      
        [HttpPut("updatepaycomponentfromformula/{id}")]
        public async Task<IActionResult> UpdatePayComponentStatusFromFormula([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] PayComponentActivationRequest payComponentMaster)
        {
            ApiResponseModel<PayComponentActivationRequest> apiResponse = new ApiResponseModel<PayComponentActivationRequest>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            if (id <= 0 || payComponentMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            payComponentMaster.EarningDeduction_Id = id;
            await _repository.UpdatePayComponentStatusAsync(DbConstants.PayComponentActivationMaster, payComponentMaster);
            apiResponse.IsSuccess = payComponentMaster.MessageType == 1;
            apiResponse.Message = payComponentMaster.StatusMessage;
            apiResponse.MessageType = payComponentMaster.MessageType;
            apiResponse.StatusCode = payComponentMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return payComponentMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- This API retrieves all pay component details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 30-April-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with pay component details or an appropriate message</returns>
        [HttpGet("getallactivepaycomponentmaster")]
        public async Task<IActionResult> GetAllActivePayComponentMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int companyId,bool isActive)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<PayComponentMaster>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            isActive = true;
            // Fetching data from the repository by executing the stored procedure
            var earningDeductionMasters = await _repository.GetAllByIdAsync(DbConstants.GetEarningDeductionMaster, new { Company_Id = companyId, IsActive= isActive });

            // Check if data exists
            if (earningDeductionMasters != null && earningDeductionMasters.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = earningDeductionMasters;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            else
            {
                // Handle the case where no data is returned
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
            }
        }
        #endregion
    }
}
