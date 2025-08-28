/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-646                                                                  *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for SalaryStructure entries.                         *
 *  It includes APIs to retrieve, create, update, and delete SalaryStructure                     *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllSalaryStructure : Retrieves all SalaryStructure records.                          *
 *  - GetSalaryStructureById: Retrieves a specific SalaryStructure record by ID.              *
 *  - PostSalaryStructure   : Adds a new SalaryStructure record.                              *
 *  - PutSalaryStructure    : Updates an existing SalaryStructure record.                     *
 *  - DeleteSalaryStructure : Soft-deletes an SalaryStructure record.                         *
 *                                                                                                *
 *  Author: Chirag Gurjar                                                                         *
 *  Date  : 09-APril-2025                                                                          *
 *                                                                                                 *
 ****************************************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class SalaryStructureApiController : ControllerBase
    {
        private readonly ISalaryStructureRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        public SalaryStructureApiController(ISalaryStructureRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }

        #region Pay Component Master Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail    :- This API retrieves all pay component details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 08-April-2025
        ///  Last Modified  :- 08-April-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with pay component details or an appropriate message</returns>
        [HttpGet("getallsalarystructure/{Id}")]
        public async Task<IActionResult> GetAllSalaryStructure( int? Id)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<SalaryStructureGrid>>();
           // var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            //if (!isValid)
            //{
            //    apiResponse.IsSuccess = false;
            //    apiResponse.Message = "Invalid API Key.";
            //    apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
            //    return Unauthorized(apiResponse);
            //}
            // Fetching data from the repository by executing the stored procedure
            var objSalaryStructure = await _repository.GetAllByIdAsync(DbConstants.GetSalaryStructureAll, new { Company_Id = Id });

            // Check if data exists
            if (objSalaryStructure != null && objSalaryStructure.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = objSalaryStructure;
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
        /// Developer Name: Chirag Gurjar
        /// Message Detail: API to retrieve pay component details based on the provided pay component ID. 
        /// This method fetches data from the repository and returns the pay component detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 09-APril-2025
        /// Change Date:  09-APril-2025
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the pay component to retrieve</param>
        /// <returns>Returns an API response with pay component details or an error message.</returns>
        [HttpGet("getsalarystructurebyid/{id}")]
        public async Task<IActionResult> GetSalaryStructureById( int id)
        {
            ApiResponseModel<SalaryStructureDTO> apiResponse = new ApiResponseModel<SalaryStructureDTO>();
            //var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            //if (!isValid)
            //{
            //    //apiResponse.IsSuccess = false;
            //    //apiResponse.Message = "Invalid API Key.";
            //    //apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
            //    //return Unauthorized(apiResponse);
            //}
            var objSalaryStructure = await _repository.GetByIdAsync(DbConstants.GetSalaryStructureById, new { SalaryStructure_Hdr_Id = id, Company_Id = 0 });
            if (objSalaryStructure == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = objSalaryStructure;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail :- This API handles the addition of pay component details based on the provided organization data.
        ///  Created Date   :- 09-APril-2025
        ///  Change Date    :- 09-APril-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="objSalaryStructure">pay component detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postsalarystructure")]
        public async Task<IActionResult> PostSalaryStructure([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] SalaryStructureDTO objSalaryStructure)
        {

            ApiResponseModel<SalaryStructureDTO> apiResponse = new ApiResponseModel<SalaryStructureDTO>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            if (objSalaryStructure == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddUpdateSalaryStructure(DbConstants.AddEditSalaryStructure, objSalaryStructure);
            apiResponse.IsSuccess = objSalaryStructure.MessageType == 1;
            apiResponse.Message = objSalaryStructure.StatusMessage;
            apiResponse.MessageType = objSalaryStructure.MessageType;
            apiResponse.StatusCode = objSalaryStructure.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return objSalaryStructure.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail :- This API handles the addition of pay component details based on the provided organization data.
        ///  Created Date   :- 09-APril-2025
        ///  Change Date    :- 09-APril-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="objSalaryStructure">pay component detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("calculatesalarystructure1")]
        public async Task<IActionResult> CalculateSalaryStructure1([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] SalarySimulatorDTO objSalaryStructure)
        {

            ApiResponseModel<SalarySimulatorDTO> apiResponse = new ApiResponseModel<SalarySimulatorDTO>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            if (objSalaryStructure == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.CalculateSalaryStructure(DbConstants.CalculateSalaryStructure, objSalaryStructure);
            apiResponse.IsSuccess = objSalaryStructure.MessageType == 1;
            apiResponse.Message = objSalaryStructure.StatusMessage;
            apiResponse.MessageType = objSalaryStructure.MessageType;
            apiResponse.StatusCode = objSalaryStructure.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return objSalaryStructure.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        [HttpPost("calculatesalarystructure")]
        public async Task<IActionResult> CalculateSalaryStructure(
    [FromHeader(Name = "X-API-KEY")] string apiKey,
    [FromBody] SalarySimulatorDTO objSalaryStructure)
        {
            ApiResponseModel<SalarySimulatorDTO> apiResponse = new ApiResponseModel<SalarySimulatorDTO>();
            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            if (objSalaryStructure == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            var result = await _repository.CalculateSalaryStructure(DbConstants.CalculateSalaryStructure, objSalaryStructure);
            apiResponse.Data = result;
            apiResponse.IsSuccess = true;
            apiResponse.Message = "Calculation successful.";
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;

            return Ok(apiResponse);
        }
        /// <summary>
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail    :- Updates the pay component detail based on the provided SalaryStructure and ID. 
        ///                    If the ID does not match or SalaryStructure is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 09-APril-2025
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the pay component to update.</param>
        /// <param name="objSalaryStructure">The pay component detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        /// 
        [HttpPut("updatesalarystructure/{id}")]
        public async Task<IActionResult> PutSalaryStructure([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] SalaryStructureDTO objSalaryStructure)
        {
            ApiResponseModel<SalaryStructureDTO> apiResponse = new ApiResponseModel<SalaryStructureDTO>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Check if the provided objSalaryStructure is null or the id doesn't match the SalaryStructure_Hdr_Id
            if (id <= 0 || objSalaryStructure == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            objSalaryStructure.SalaryStructure_Hdr_Id = id;
            // Call the UpdateEarningDeductionAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditSalaryStructure, objSalaryStructure);
            apiResponse.IsSuccess = objSalaryStructure.MessageType == 1;
            apiResponse.Message = objSalaryStructure.StatusMessage;
            apiResponse.MessageType = objSalaryStructure.MessageType;
            apiResponse.StatusCode = objSalaryStructure.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return objSalaryStructure.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail    :- Updates the delete column based on the provided SalaryStructure and ID. 
        ///                    If the ID does not match or SalaryStructure is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 09-APril-2025
        ///  Last Updated   :- 09-APril-2025
        /// </summary>
        /// <param name="id">The ID of the pay component to update.</param>
        /// <param name="objSalaryStructure">The pay component detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletesalarystructure/{id}")]
        public async Task<IActionResult> DeleteSalaryStructure( int id, [FromBody] SalaryStructureDTO objSalaryStructure)
        {
            ApiResponseModel<SalaryStructureDTO> apiResponse = new ApiResponseModel<SalaryStructureDTO>();
            //var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            //if (!isValid)
            //{
            //    //apiResponse.IsSuccess = false;
            //    //apiResponse.Message = "Invalid API Key.";
            //    //apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
            //    //return Unauthorized(apiResponse);
            //}
            // Check if the provided SalaryStructure is null or the id doesn't match the Id
            if (id <= 0 || objSalaryStructure == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            objSalaryStructure.SalaryStructure_Hdr_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteSalaryStructure, objSalaryStructure);
            apiResponse.IsSuccess = objSalaryStructure.MessageType == 1;
            apiResponse.Message = objSalaryStructure.StatusMessage;
            apiResponse.MessageType = objSalaryStructure.MessageType;
            apiResponse.StatusCode = objSalaryStructure.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return objSalaryStructure.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail :- This API handles the addition of pay component details based on the provided organization data.
        ///  Created Date   :- 09-APril-2025
        ///  Change Date    :- 09-APril-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="objSalaryStructure">pay component detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("addeditsalarystructure")]
        public async Task<IActionResult> AddEditSalaryStructure([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] SalaryStructureDTO objSalaryStructure)
        {

            ApiResponseModel<SalaryStructureDTO> apiResponse = new ApiResponseModel<SalaryStructureDTO>();
            var isValid = true; // _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            if (objSalaryStructure == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddUpdateSalaryStructure(DbConstants.AddEditSalaryStructure, objSalaryStructure);
            apiResponse.IsSuccess = objSalaryStructure.MessageType == 1;
            apiResponse.Message = objSalaryStructure.StatusMessage;
            apiResponse.MessageType = objSalaryStructure.MessageType;
            apiResponse.StatusCode = objSalaryStructure.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return objSalaryStructure.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }


        #endregion
    }
}

