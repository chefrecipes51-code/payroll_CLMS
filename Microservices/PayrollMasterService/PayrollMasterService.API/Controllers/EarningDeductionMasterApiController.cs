/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-66                                                                   *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for EarningDeductionMaster entries.                     *
 *  It includes APIs to retrieve, create, update, and delete EarningDeductionMaster                 *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllEarningDeductionMaster : Retrieves all EarningDeductionMaster records.                  *
 *  - GetEarningDeductionMasterById: Retrieves a specific EarningDeductionMaster record by ID.      *
 *  - PostEarningDeductionMaster   : Adds a new EarningDeductionMaster record.                      *
 *  - PutEarningDeductionMaster    : Updates an existing EarningDeductionMaster record.             *
 *  - DeleteEarningDeductionMaster : Soft-deletes an EarningDeductionMaster record.                 *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 19-Sep-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using PayrollMasterService.DAL.Service;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class EarningDeductionMasterApiController : ControllerBase
    {
        private readonly IEarningDeductionMasterRepository _repository;
        public EarningDeductionMasterApiController(IEarningDeductionMasterRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #region Earning Deduction Master Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all earning deduction details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 19-Sep-2024
        ///  Last Modified  :- 19-Sep-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with earning deduction details or an appropriate message</returns>
        [HttpGet("getallearningdeductiomaster")]
        public async Task<IActionResult> GetAllEarningDeductioMaster(int companyId)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<EarningDeductionMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var earningDeductionMasters = await _repository.GetAllByIdAsync(DbConstants.GetEarningDeductionMaster, new { Company_Id = companyId });

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
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve earning deduction details based on the provided Earning Deduction ID. 
        /// This method fetches data from the repository and returns the earning deduction detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 19-Sep-2024
        /// Change Date: 19-Sep-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the earning deduction to retrieve</param>
        /// <returns>Returns an API response with earning deduction details or an error message.</returns>
        [HttpGet("getearningdeductiomasterbyid/{id}")]
        public async Task<IActionResult> GetEarningDeductioMasterById(int id,int companyId)
        {
            ApiResponseModel<EarningDeductionMaster> apiResponse = new ApiResponseModel<EarningDeductionMaster>();
            var earningDeductionMaster = await _repository.GetByIdAsync(DbConstants.GetEarningDeductionMasterById, new { EarningDeduction_Id = id, Company_Id = companyId });
            if (earningDeductionMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = earningDeductionMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of earning deduction details based on the provided organization data.
        ///  Created Date   :- 19-Sep-2024
        ///  Change Date    :- 19-Sep-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="earningDeductionMaster">Earning Deduction detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postearningdeductiomaster")]
        public async Task<IActionResult> PostEarningDeductioMaster([FromBody] EarningDeductionMaster earningDeductionMaster)
        {
            ApiResponseModel<EarningDeductionMaster> apiResponse = new ApiResponseModel<EarningDeductionMaster>();
            if (earningDeductionMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddEditEarningDeductionMaster, earningDeductionMaster);
            apiResponse.IsSuccess = earningDeductionMaster.MessageType == 1;
            apiResponse.Message = earningDeductionMaster.StatusMessage;
            apiResponse.MessageType = earningDeductionMaster.MessageType;
            apiResponse.StatusCode = earningDeductionMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return earningDeductionMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the earning deduction detail based on the provided earningDeductionMaster and ID. 
        ///                    If the ID does not match or earningDeductionMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 19-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the earning deduction to update.</param>
        /// <param name="earningDeductionMaster">The earning deduction detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updateearningdeductiomaster/{id}")]
        public async Task<IActionResult> PutEarningDeductioMaster(int id, [FromBody] EarningDeductionMaster earningDeductionMaster)
        {
            ApiResponseModel<EarningDeductionMaster> apiResponse = new ApiResponseModel<EarningDeductionMaster>();
            // Check if the provided earningDeductionMaster is null or the id doesn't match the EarningDeduction_Id
            if (id <= 0 || earningDeductionMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            earningDeductionMaster.EarningDeduction_Id = id;
            // Call the UpdateEarningDeductionAsync method in the repository
            await _repository.UpdateAsync(DbConstants.AddEditEarningDeductionMaster, earningDeductionMaster);
            apiResponse.IsSuccess = earningDeductionMaster.MessageType == 1;
            apiResponse.Message = earningDeductionMaster.StatusMessage;
            apiResponse.MessageType = earningDeductionMaster.MessageType;
            apiResponse.StatusCode = earningDeductionMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return earningDeductionMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided earningDeductionMaster and ID. 
        ///                    If the ID does not match or earningDeductionMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 19-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the earning deduction to update.</param>
        /// <param name="earningDeductionMaster">The earning deduction detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deleteearningdeductiomaster/{id}")]
        public async Task<IActionResult> DeleteEarningDeductioMaster(int id, [FromBody] EarningDeductionMaster earningDeductionMaster)
        {
            ApiResponseModel<EarningDeductionMaster> apiResponse = new ApiResponseModel<EarningDeductionMaster>();
            // Check if the provided earningDeductionMaster is null or the id doesn't match the EarningDeduction_Id
            if (id <= 0 || earningDeductionMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            earningDeductionMaster.EarningDeduction_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteEarningDeductionMaster, earningDeductionMaster);
            apiResponse.IsSuccess = earningDeductionMaster.MessageType == 1;
            apiResponse.Message = earningDeductionMaster.StatusMessage;
            apiResponse.MessageType = earningDeductionMaster.MessageType;
            apiResponse.StatusCode = earningDeductionMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return earningDeductionMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        #endregion
    }
}
