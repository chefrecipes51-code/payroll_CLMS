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
    public class PTaxSlabApiController : ControllerBase
    {

        private readonly IPTaxSlabRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;

        public PTaxSlabApiController(IPTaxSlabRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- This API retrieves all PTax Slab details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 06-05-25
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Pay Month details or an appropriate message</returns>

        [HttpGet("getallptakslab")]
        public async Task<IActionResult> GetAllPTaxSlab(
                                    [FromHeader(Name = "X-API-KEY")] string apiKey, int company_Id)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<ProfessionalTaxSlabViewModel>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            var pTaxSlabDetails = await _repository.GetAllAsyncWithId(DbConstants.GetPtaxSlab, new { Company_Id = company_Id });
            if (pTaxSlabDetails != null && pTaxSlabDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = pTaxSlabDetails;
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
        /// Developer Name: Harshida Parmar
        /// Message Detail: API to retrieve pay PTaxSlab details based on the provided ID. 
        /// This method fetches data from the repository and returns the PTaxSlab detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 06-05-2025
        /// Change Date:  
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of thePTaxSlab to retrieve</param>
        /// <returns>Returns an API response with PTaxSlab details or an error message.</returns>
        [HttpGet("getallptakslabbyid/{id}")]
        public async Task<IActionResult> GetPTaxSlabById([FromHeader(Name = "X-API-KEY")] string apiKey, int id)
        {
            ApiResponseModel<PtaxSlabRequest> apiResponse = new ApiResponseModel<PtaxSlabRequest>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            var pTaxSlabDetails = await _repository.GetByIdAsync(DbConstants.GetPtaxSlabById, new { Ptax_Slab_Id = id });
            if (pTaxSlabDetails == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = pTaxSlabDetails;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        /// Developer Name: Harshida Parmar
        /// Message Detail: API to retrieve TaxParam details based on the provided ID. 
        /// This method fetches data from the repository and returns the TaxParam detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 08-05-2025
        /// Change Date:  
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of TaxParam to retrieve</param>
        /// <returns>Returns an API response with TaxParam details or an error message.</returns>
        [HttpGet("getselecttaxparam")]
        public async Task<IActionResult> GetSelectTaxParam([FromHeader(Name = "X-API-KEY")] string apiKey, int id, int sid)
        {
            ApiResponseModel<IEnumerable<TaxParamRequest>> apiResponse = new ApiResponseModel<IEnumerable<TaxParamRequest>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            var pTaxParamDetails = await _repository.GetTaxParamAsync(DbConstants.GetTaxParam, id, sid);
            if (pTaxParamDetails == null || !pTaxParamDetails.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = pTaxParamDetails;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Harshida Par,ar
        ///  Message detail :- This API handles the addition of pTaxSlab details based on the provided organization data.
        ///  Created Date   :- 06-05-2025
        ///  Change Date    :- 
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="payComponentMaster">pTaxSlab detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postptaxslab")]
        public async Task<IActionResult> PostPTaxSlab([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] PtaxSlabRequest pTaxSlab)
        {

            ApiResponseModel<PtaxSlabRequest> apiResponse = new ApiResponseModel<PtaxSlabRequest>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            if (pTaxSlab == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddEditPtaxSlab, pTaxSlab);
            apiResponse.IsSuccess = pTaxSlab.MessageType == 1;
            apiResponse.Message = pTaxSlab.StatusMessage;
            apiResponse.MessageType = pTaxSlab.MessageType;
            apiResponse.StatusCode = pTaxSlab.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
           // return pTaxSlab.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
            return pTaxSlab.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);

        }

        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- Updates the PtaxSlabRequest detail based on the provided PtaxSlabRequest and ID. 
        ///                    If the ID does not match or PtaxSlabRequest is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 06-05-2025
        ///  Last Updated   :-
        /// </summary>
        /// <param name="id">The ID of the pTaxSlab to update.</param>
        /// <param name="pTaxSlab">The pTaxSlab detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updateptaxslab/{id}")]
        public async Task<IActionResult> PutPTaxSlab([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] PtaxSlabRequest pTaxSlab)
        {
            ApiResponseModel<PtaxSlabRequest> apiResponse = new ApiResponseModel<PtaxSlabRequest>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Check if the provided PtaxSlabRequest is null or the id doesn't match the Ptax_Slab_Id
            if (id <= 0 || pTaxSlab == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            pTaxSlab.Ptax_Slab_Id = id;
            // Call the UpdateEarningDeductionAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditPtaxSlab, pTaxSlab);
            apiResponse.IsSuccess = pTaxSlab.MessageType == 1;
            apiResponse.Message = pTaxSlab.StatusMessage;
            apiResponse.MessageType = pTaxSlab.MessageType;
            apiResponse.StatusCode = pTaxSlab.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // return pTaxSlab.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
            return pTaxSlab.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- Updates the delete column based on the provided ptaxslab and ID. 
        ///                    If the ID does not match or ptaxslab is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 06-05-2025
        ///  Last Updated   :-
        /// </summary>
        /// <param name="id">The ID of the ptaxslab to update.</param>
        /// <param name="ptaxslab">The ptaxslab detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deleteptaxslab/{id}")]
        public async Task<IActionResult> Deleteptaxslab([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] PtaxSlabRequest ptaxslab)
        {
            ApiResponseModel<PtaxSlabRequest> apiResponse = new ApiResponseModel<PtaxSlabRequest>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            if (id <= 0 || ptaxslab == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            ptaxslab.Ptax_Slab_Id = id;
            await _repository.DeleteAsync(DbConstants.DeletePtaxSlab, ptaxslab);
            apiResponse.IsSuccess = ptaxslab.MessageType == 1;
            apiResponse.Message = ptaxslab.StatusMessage;
            apiResponse.MessageType = ptaxslab.MessageType;
            apiResponse.StatusCode = ptaxslab.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return ptaxslab.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

    }
}
