/****************************************************************************************************
 *  Jira Task Ticket :                                                                              *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for CompanyCurrencyMaster entries.                      *
 *  It includes APIs to retrieve, create, update, and delete CompanyCurrencyMaster                  *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllCompanyCurrencyMaster : Retrieves all CompanyCurrencyMaster records.                    *
 *  - GetCompanyCurrencyMasterById: Retrieves a specific CompanyCurrencyMaster record by ID.        *
 *  - PostCompanyCurrencyMaster   : Adds a new CompanyCurrencyMaster record.                        *
 *  - PutCompanyCurrencyMaster    : Updates an existing CompanyCurrencyMaster record.               *
 *  - DeleteCompanyCurrencyMaster : Soft-deletes an CompanyCurrencyMaster record.                   *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 25-Sep-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class CompanyCurrencyMasterApiController : ControllerBase
    {
        private readonly ICompanyCurrencyMasterRepository _repository;
        public CompanyCurrencyMasterApiController(ICompanyCurrencyMasterRepository repository)
        {
            _repository = repository;
        }
        #region CompanyCurrencyMaster CRUD APIs Functionality
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all CompanyCurrencyMaster details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 25-Sep-2024
        ///  Last Modified  :- 25-Sep-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with CompanyCurrencyMaster details or an appropriate message</returns>
        [HttpGet("getallcompanycurrencymaster")]
        public async Task<IActionResult> GetAllCompanyCurrencyMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<CompanyCurrencyMaster>>();
            var companyCurrencyMasters = await _repository.GetAllAsync(DbConstants.GetCompanyCurrencyMaster);
            if (companyCurrencyMasters != null && companyCurrencyMasters.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = companyCurrencyMasters;
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
        /// Message Detail: API to retrieve CompanyCurrencyMaster details based on the provided ID.
        /// Created Date: 25-Sep-2024
        /// </summary>
        /// <param name="id">The ID of the CompanyCurrencyMaster to retrieve</param>
        /// <returns>Returns an API response with CompanyCurrencyMaster details or an error message.</returns>
        [HttpGet("getcompanycurrencymasterbyid/{id}")]
        public async Task<IActionResult> GetCompanyCurrencyMasterById(int id)
        {
            ApiResponseModel<CompanyCurrencyMaster> apiResponse = new ApiResponseModel<CompanyCurrencyMaster>();
            var companyCurrencyMaster = await _repository.GetByIdAsync(DbConstants.GetCompanyCurrencyMasterById, new { CompanyCurrency_Id = id });
            if (companyCurrencyMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = companyCurrencyMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of CompanyCurrencyMaster details.
        ///  Created Date   :- 25-Sep-2024
        /// </summary>
        /// <param name="companyCurrencyMaster">CompanyCurrencyMaster details to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postcompanycurrencymaster")]
        public async Task<IActionResult> PostCompanyCurrencyMaster([FromBody] CompanyCurrencyMaster companyCurrencyMaster)
        {
            ApiResponseModel<CompanyCurrencyMaster> apiResponse = new ApiResponseModel<CompanyCurrencyMaster>();
            if (companyCurrencyMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddEditCompanyCurrencyMaster, companyCurrencyMaster);
            apiResponse.IsSuccess = companyCurrencyMaster.MessageType == 1;
            apiResponse.Message = companyCurrencyMaster.StatusMessage;
            apiResponse.MessageType = companyCurrencyMaster.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- Updates CompanyCurrencyMaster based on the provided details and ID.
        ///  Created Date   :- 25-Sep-2024
        ///  Change Date    :- 21-Nov-2024
        ///  Change detail  :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the CompanyCurrencyMaster to update.</param>
        /// <param name="companyCurrencyMaster">The CompanyCurrencyMaster details to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatecompanycurrencymaster/{id}")]
        public async Task<IActionResult> PutCompanyCurrencyMaster(int id, [FromBody] CompanyCurrencyMaster companyCurrencyMaster)
        {
            ApiResponseModel<CompanyCurrencyMaster> apiResponse = new ApiResponseModel<CompanyCurrencyMaster>();
            if (companyCurrencyMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            companyCurrencyMaster.CompanyCurrency_Id = id;
            await _repository.UpdateAsync(DbConstants.AddEditCompanyCurrencyMaster, companyCurrencyMaster);
            apiResponse.IsSuccess = companyCurrencyMaster.MessageType == 1;
            apiResponse.Message = companyCurrencyMaster.StatusMessage;
            apiResponse.MessageType = companyCurrencyMaster.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- Updates the delete column based on the provided CompanyCurrencyMaster and ID.
        ///  Created Date   :- 25-Sep-2024
        ///  Change Date    :- 21-Nov-2024
        ///  Change detail  :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the CompanyCurrencyMaster to delete.</param>
        /// <param name="companyCurrencyMaster">The CompanyCurrencyMaster details to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletecompanycurrencymaster/{id}")]
        public async Task<IActionResult> DeleteCompanyCurrencyMaster(int id, [FromBody] CompanyCurrencyMaster companyCurrencyMaster)
        {
            ApiResponseModel<CompanyCurrencyMaster> apiResponse = new ApiResponseModel<CompanyCurrencyMaster>();
            if (companyCurrencyMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            companyCurrencyMaster.CompanyCurrency_Id = id;
            await _repository.DeleteAsync(DbConstants.DeleteCompanyCurrencyMaster, companyCurrencyMaster);
            apiResponse.IsSuccess = companyCurrencyMaster.MessageType == 1;
            apiResponse.Message = companyCurrencyMaster.StatusMessage;
            apiResponse.MessageType = companyCurrencyMaster.MessageType;
            apiResponse.StatusCode = apiResponse.IsSuccess ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
    }
}
