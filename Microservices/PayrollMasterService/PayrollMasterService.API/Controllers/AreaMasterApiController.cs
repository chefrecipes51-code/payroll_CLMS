/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-75                                                                   *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for AreaMaster entries.                                 *
 *  It includes APIs to retrieve, create, update, and delete AreaMaster                             *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllAreaMaster : Retrieves all AreaMaster records.                                          *
 *  - GetAreaMasterById: Retrieves a specific AreaMaster record by ID.                              *
 *  - PostAreaMaster   : Adds a new AreaMaster record.                                              *
 *  - PutAreaMaster    : Updates an existing AreaMaster record.                                     *
 *  - DeleteAreaMaster : Soft-deletes an AreaMaster record.                                         *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 24-Sep-2024                                                                             *
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
    public class AreaMasterApiController : ControllerBase
    {
        private readonly IAreaMasterRepository _repository;
        public AreaMasterApiController(IAreaMasterRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #region Area Master Crud APIs Functionality

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all Area details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 24-Sep-2024
        ///  Last Modified  :- 24-Sep-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Area details or an appropriate message</returns>
        [HttpGet("getallareamaster")]
        public async Task<IActionResult> GetAllAreaMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<AreaMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var areaMasters = await _repository.GetAllAsync(DbConstants.GetAreaMaster);

            // Check if data exists
            if (areaMasters != null && areaMasters.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = areaMasters;
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
        /// Message Detail: API to retrieve Area details based on the provided  Area ID. 
        /// This method fetches data from the repository and returns the  area detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 24-Sep-2024
        /// Change Date: 24-Sep-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the  area to retrieve</param>
        /// <returns>Returns an API response with area details or an error message.</returns>
        [HttpGet("getareamasterbyid/{id}")]
        public async Task<IActionResult> GetAreaMasterById(int id)
        {
            ApiResponseModel<IEnumerable<AreaMaster>> apiResponse = new ApiResponseModel<IEnumerable<AreaMaster>>();

            //ApiResponseModel<AreaMaster> apiResponse = new ApiResponseModel<AreaMaster>();
            var areaMaster = await _repository.GetAllByIdAsync(DbConstants.GetAreaMasterById, new { Area_Id = id });
            if (areaMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = areaMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of area details based on the provided organization data.
        ///  Created Date   :- 24-Sep-2024
        ///  Change Date    :- 24-Sep-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="areaMaster"> area detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postareamaster")]
        public async Task<IActionResult> PostAreaMaster([FromBody] AreaMaster areaMaster)
        {
            ApiResponseModel<AreaMaster> apiResponse = new ApiResponseModel<AreaMaster>();
            if (areaMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddEditAreaMaster, areaMaster);
            if (areaMaster.MessageType == 1)
            {
                apiResponse.IsSuccess = true;
                apiResponse.Message = areaMaster.StatusMessage;
                apiResponse.MessageType = areaMaster.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.Created;
                return StatusCode((int)HttpStatusCode.Created, apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = areaMaster.StatusMessage;
                apiResponse.MessageType = areaMaster.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- Updates an existing AreaMaster record in the database based on the provided ID and data.
        ///                       Validates the input, and if successful, updates the record and returns a success message.
        ///  Created Date   :- 24-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the AreaMaster record to update.</param>
        /// <param name="areaMaster">The AreaMaster object containing the updated data.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updateareamaster/{id}")]
        public async Task<IActionResult> PutAreaMaster(int id, [FromBody] AreaMaster areaMaster)
        {
            ApiResponseModel<AreaMaster> apiResponse = new ApiResponseModel<AreaMaster>();
            // Check if the provided areaMaster is null or the id doesn't match the Area_Id
            if (areaMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }
            areaMaster.Area_Id = id;
            await _repository.UpdateAsync(DbConstants.AddEditAreaMaster, areaMaster);// Call the UpdateAsync method in the repository
            apiResponse.IsSuccess = areaMaster.MessageType == 1;
            apiResponse.Message = areaMaster.StatusMessage;
            apiResponse.MessageType = areaMaster.MessageType;
            apiResponse.StatusCode = areaMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return areaMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- Soft-deletes an AreaMaster record by updating its delete column based on the provided ID and data.
        ///                    Validates the input, and if successful, marks the record as deleted and returns a success message.
        ///  Created Date   :- 24-Sep-2024
        ///  Last Updated   :- 21-Nov-2024
        ///  Change Details :- Remove GetById method according to Payroll-197 Jira Ticket.
        /// </summary>
        /// <param name="id">The ID of the AreaMaster record to delete.</param>
        /// <param name="areaMaster">The AreaMaster object for validation and soft delete operation.</param>
        /// <returns>A JSON response indicating the success or failure of the delete operation.</returns>
        [HttpDelete("deleteareamaster/{id}")]
        public async Task<IActionResult> DeleteAreaMaster(int id, [FromBody] AreaMaster areaMaster)
        {
            ApiResponseModel<AreaMaster> apiResponse = new ApiResponseModel<AreaMaster>();
            // Check if the provided area is null or the id doesn't match the Area_Id
            if (areaMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }

            areaMaster.Area_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteAsync(DbConstants.DeleteAreaMaster, areaMaster);
            apiResponse.IsSuccess = areaMaster.MessageType == 1;
            apiResponse.Message = areaMaster.StatusMessage;
            apiResponse.MessageType = areaMaster.MessageType;
            apiResponse.StatusCode = areaMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return areaMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }

        [HttpGet("getarealocationmasterbyid/{id}")]
        public async Task<IActionResult> GetAreaLocationMasterById(int id)
        {
            ApiResponseModel<IEnumerable<AreaMaster>> apiResponse = new ApiResponseModel<IEnumerable<AreaMaster>>();

            //ApiResponseModel<AreaMaster> apiResponse = new ApiResponseModel<AreaMaster>();
            //var areaMaster = await _repository.GetAllByIdAsync(DbConstants.GetAreaMasterById, new { Location_Id = id });
            var areaMaster = await _repository.GetAllAreaByLocationIdAsync(DbConstants.GetAreaMasterById, new { Location_Id = id });
            if (areaMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = areaMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        #endregion
    }
}
