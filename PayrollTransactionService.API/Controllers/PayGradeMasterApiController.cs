using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.Repository.Interface;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.DAL.Interface;
using System.Net;

namespace PayrollTransactionService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class PayGradeMasterApiController : ControllerBase
    {
        private readonly IPayGradeMasterRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        public PayGradeMasterApiController(IPayGradeMasterRepository repository, ApiKeyValidatorHelper apiKeyValidatorHelper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidatorHelper;
        }
        #region Pay Grade Master Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all wage grade details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 13-Sep-2024
        ///  Last Modified  :- 13-Sep-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with wage grade details or an appropriate message</returns>
        [HttpGet("getallpaygrademaster")]
        public async Task<IActionResult> GetAllPayGradeMaster([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<PayGradeMaster>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            // Fetching data from the repository by executing the stored procedure
            var payGradeDetails = await _repository.GetAllAsync(DbConstants.GetPayGradeMaster);
            // Check if data exists
            if (payGradeDetails != null && payGradeDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = payGradeDetails;
                //apiResponse.Message = ApiResponseMessageConstant.FetchAllRecords;
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
        /// Message Detail: API to retrieve wage grade details based on the provided wage grade ID. 
        /// This method fetches data from the repository and returns the wage grade detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 13-Sep-2024
        /// Change Date: 13-Sep-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the wage grade to retrieve</param>
        /// <returns>Returns an API response with wage grade details or an error message.</returns>
        [HttpGet("getpaygrademasterbyid/{id}")]
        public async Task<IActionResult> GetPayGradeMasterById([FromHeader(Name = "X-API-KEY")] string apiKey, int id)
        {
            ApiResponseModel<PayGradeMaster> apiResponse = new ApiResponseModel<PayGradeMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            // Attempt to retrieve from cache or fetch and create cache entry
            var payGradeMaster = await _repository.GetByIdAsync(DbConstants.GetPayGradeMaster, new { PayGrade_Id = id }); // Cache expiration set to 10 minutes
            if (payGradeMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = payGradeMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of wage grade details based on the provided organization data.
        ///  Created Date   :- 16-Sep-2024
        ///  Change Date    :- 16-Sep-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="wageGradeDetail">Wage grade detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postpaygrademaster")]
        public async Task<IActionResult> PostPayGradeMaster([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] PayGradeMaster payGradeDetail)
        {
            ApiResponseModel<PayGradeMaster> apiResponse = new ApiResponseModel<PayGradeMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (payGradeDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddEditPayGradeMaster, payGradeDetail);
            apiResponse.IsSuccess = payGradeDetail.MessageType == 1;
            apiResponse.Message = payGradeDetail.StatusMessage;
            apiResponse.MessageType = payGradeDetail.MessageType;
            apiResponse.StatusCode = payGradeDetail.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return payGradeDetail.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the wage grade detail based on the provided wageGradeDetail and ID. 
        ///                    If the ID does not match or wageGradeDetail is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 16-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the Wage Grade to update.</param>
        /// <param name="payGradeDetail">The wage grade detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatepaygrademaster/{id}")]
        public async Task<IActionResult> PutWageGradeMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] PayGradeMaster payGradeDetail)
        {
            ApiResponseModel<PayGradeMaster> apiResponse = new ApiResponseModel<PayGradeMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            // Check if the provided wageGradeDetail is null or the id doesn't match the Wage_Id
            if (id <= 0 || payGradeDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            payGradeDetail.PayGrade_Id = id;
            // Call the UpdateWageGradeAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditPayGradeMaster, payGradeDetail);
            apiResponse.IsSuccess = payGradeDetail.MessageType == 1;
            apiResponse.Message = payGradeDetail.StatusMessage;
            apiResponse.MessageType = payGradeDetail.MessageType;
            apiResponse.StatusCode = payGradeDetail.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return payGradeDetail.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided wageGradeDetail and ID. 
        ///                    If the ID does not match or wageGradeDetail is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 19-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the earning deduction to update.</param>
        /// <param name="wageGradeDetail">The Wage Grade detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletepaygrademaster/{id}")]
        public async Task<IActionResult> DeletePayGradeMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] PayGradeMaster payGradeDetail)
        {
            ApiResponseModel<PayGradeMaster> apiResponse = new ApiResponseModel<PayGradeMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            // Check if the provided wageGradeDetail is null or the id doesn't match the Wage_Id
            if (id <= 0 || payGradeDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            payGradeDetail.PayGrade_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeletePayGradeMaster, payGradeDetail);
            apiResponse.IsSuccess = payGradeDetail.MessageType == 1;
            apiResponse.Message = payGradeDetail.StatusMessage;
            apiResponse.MessageType = payGradeDetail.MessageType;
            apiResponse.StatusCode = payGradeDetail.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return payGradeDetail.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all wage grade details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 13-Sep-2024
        ///  Last Modified  :- 13-Sep-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with wage grade details or an appropriate message</returns>
        [HttpGet("getallactivepaygrademaster")]
        public async Task<IActionResult> GetAllActivePayGradeMaster([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<PayGradeMaster>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            // Fetching data from the repository by executing the stored procedure
            var payGradeDetails = await _repository.GetAllActiveAsync(DbConstants.GetPayGradeMaster, new { IsActive = true });
            // Check if data exists
            if (payGradeDetails != null && payGradeDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = payGradeDetails;
                //apiResponse.Message = ApiResponseMessageConstant.FetchAllRecords;
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
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all wage grade details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 13-Sep-2024
        ///  Last Modified  :- 13-Sep-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with wage grade details or an appropriate message</returns>
        [HttpGet("getalltrademaster")]
        public async Task<IActionResult> GetAllTradeMaster(
         [FromHeader(Name = "X-API-KEY")] string apiKey,
         int companyLocationID,
         int? trade_mst_Id,
         bool isActive)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<TradeMaster>>();

            // Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidApiKey;
                return Unauthorized(apiResponse);
            }

            var tradeDetails = await _repository.GetAllTradeAsync(DbConstants.GetTradeMaster, new { Company_Location_ID = companyLocationID, Trade_mst_Id = trade_mst_Id, IsActive = isActive });

            if (tradeDetails != null && tradeDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = tradeDetails;
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
        ///  Message detail    :- This API retrieves all wage grade details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 13-Sep-2024
        ///  Last Modified  :- 13-Sep-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with wage grade details or an appropriate message</returns>
        [HttpGet("getallskillcategory")]
        public async Task<IActionResult> GetAllSkillCategory(
         [FromHeader(Name = "X-API-KEY")] string apiKey,
         int correspondance_ID,
         int? skillcategory_Id,
         bool isActive)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<SkillCategory>>();

            // Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidApiKey;
                return Unauthorized(apiResponse);
            }

            var skillCategoryDetails = await _repository.GetAllSkillCategoryAsync(DbConstants.GetSkillCategory, new { Correspondance_ID = correspondance_ID, Skillcategory_Id = skillcategory_Id, IsActive = isActive });

            if (skillCategoryDetails != null && skillCategoryDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = skillCategoryDetails;
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
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve wage grade details based on the provided wage grade ID. 
        /// This method fetches data from the repository and returns the wage grade detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 13-Sep-2024
        /// Change Date: 13-Sep-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the wage grade to retrieve</param>
        /// <returns>Returns an API response with wage grade details or an error message.</returns>
        [HttpGet("getdistinctlocationbyid")]
        public async Task<IActionResult> GetDistinctLocationById([FromHeader(Name = "X-API-KEY")] string apiKey, int company_ID)
        {
            ApiResponseModel<IEnumerable<DistinctLocation>> apiResponse = new ApiResponseModel<IEnumerable<DistinctLocation>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            // Attempt to retrieve from cache or fetch and create cache entry
            var distinctMaster = await _repository.GetAllDistinctLocationAsync(DbConstants.GetDistinctLocation, new { Company_ID = company_ID }); 
            if (distinctMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = distinctMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        #endregion

        #region Pay Grade Config Master Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all wage grade details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 13-Sep-2024
        ///  Last Modified  :- 13-Sep-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with wage grade details or an appropriate message</returns>
        [HttpGet("getallpaygradeconfigmaster/{id}")]
        public async Task<IActionResult> GetAllPayGradeConfigMaster([FromHeader(Name = "X-API-KEY")] string apiKey,int id)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<PayGradeConfigMaster>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            // Fetching data from the repository by executing the stored procedure
            var payGradeConfigDetails = await _repository.GetAllPayGradeConfigAsync(DbConstants.GetPayGradeConfigMaster,new { Cmp_Id = id });
            // Check if data exists
            if (payGradeConfigDetails != null && payGradeConfigDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = payGradeConfigDetails;
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
        /// Message Detail: API to retrieve wage grade details based on the provided wage grade ID. 
        /// This method fetches data from the repository and returns the wage grade detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 13-Sep-2024
        /// Change Date: 13-Sep-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the wage grade to retrieve</param>
        /// <returns>Returns an API response with wage grade details or an error message.</returns>
        [HttpGet("getpaygradeconfigmasterbyid/{id}")]
        public async Task<IActionResult> GetPayGradeconfigMasterById([FromHeader(Name = "X-API-KEY")] string apiKey, int id)
        {
            ApiResponseModel<PayGradeConfigMaster> apiResponse = new ApiResponseModel<PayGradeConfigMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            // Attempt to retrieve from cache or fetch and create cache entry
            var payGradeConfigMaster = await _repository.GetPayGradeConfigByIdAsync(DbConstants.GetPayGradeConfigMaster, new { PayGradeConfig_Id = id }); // Cache expiration set to 10 minutes
            if (payGradeConfigMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = payGradeConfigMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of wage grade details based on the provided organization data.
        ///  Created Date   :- 16-Sep-2024
        ///  Change Date    :- 16-Sep-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="wageGradeDetail">Wage grade detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postpaygradeconfigmaster")]
        public async Task<IActionResult> PostPayGradeConfigMaster([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] PayGradeConfigMaster payGradeConfigDetail)
        {
            ApiResponseModel<PayGradeConfigMaster> apiResponse = new ApiResponseModel<PayGradeConfigMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (payGradeConfigDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddPayGradeConfigAsync(DbConstants.AddEditPayGradeConfigMaster, payGradeConfigDetail);
            apiResponse.IsSuccess = payGradeConfigDetail.MessageType == 1;
            apiResponse.Message = payGradeConfigDetail.StatusMessage;
            apiResponse.MessageType = payGradeConfigDetail.MessageType;
            apiResponse.StatusCode = payGradeConfigDetail.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return payGradeConfigDetail.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the wage grade detail based on the provided wageGradeDetail and ID. 
        ///                    If the ID does not match or wageGradeDetail is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 16-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the Wage Grade to update.</param>
        /// <param name="payGradeDetail">The wage grade detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatepaygradeconfigmaster/{id}")]
        public async Task<IActionResult> PutWageGradeConfigMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] PayGradeConfigMaster payGradeConfigDetail)
        {
            ApiResponseModel<PayGradeConfigMaster> apiResponse = new ApiResponseModel<PayGradeConfigMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            // Check if the provided wageGradeDetail is null or the id doesn't match the Wage_Id
            if (id <= 0 || payGradeConfigDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            payGradeConfigDetail.PayGradeConfig_Id = id;
            // Call the UpdateWageGradeAsync method in the repository
            await _repository.UpdatePayGradeConfigAsync(DbConstants.AddEditPayGradeConfigMaster, payGradeConfigDetail);
            apiResponse.IsSuccess = payGradeConfigDetail.MessageType == 1;
            apiResponse.Message = payGradeConfigDetail.StatusMessage;
            apiResponse.MessageType = payGradeConfigDetail.MessageType;
            apiResponse.StatusCode = payGradeConfigDetail.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return payGradeConfigDetail.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided wageGradeDetail and ID. 
        ///                    If the ID does not match or wageGradeDetail is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 19-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the earning deduction to update.</param>
        /// <param name="payGradeConfigDetail">The Wage Grade detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletepaygradeconfigmaster/{id}")]
        public async Task<IActionResult> DeletePayGradeConfigMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] PayGradeConfigMaster payGradeConfigDetail)
        {
            ApiResponseModel<PayGradeConfigMaster> apiResponse = new ApiResponseModel<PayGradeConfigMaster>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            // Check if the provided wageGradeDetail is null or the id doesn't match the Wage_Id
            if (id <= 0 || payGradeConfigDetail == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            payGradeConfigDetail.PayGradeConfig_Id = id;
            // Call the UpdateAsync method in the repository
            await _repository.DeletePayGradeConfigAsync(DbConstants.DeletePayGradeConfigMaster, payGradeConfigDetail);
            apiResponse.IsSuccess = payGradeConfigDetail.MessageType == 1;
            apiResponse.Message = payGradeConfigDetail.StatusMessage;
            apiResponse.MessageType = payGradeConfigDetail.MessageType;
            apiResponse.StatusCode = payGradeConfigDetail.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return payGradeConfigDetail.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        #endregion
    }
}
