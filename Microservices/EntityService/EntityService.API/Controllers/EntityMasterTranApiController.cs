using EntityService.BAL.Models;
using EntityService.DAL.Interface;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using System.Net;

namespace EntityService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class EntityMasterTranApiController : ControllerBase
    {
        private readonly IEntityMasterTranRepository _repository;
        public EntityMasterTranApiController(IEntityMasterTranRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        ///  Developer Name :- Chirag Gurjar
        ///  Message detail :- Updates the Entity master staging detail based on the provided data with is approved or not by Log_ID. 
        ///  Created Date   :- 30-Jan-2025
        ///  Last Updated   :- 
        ///  Change Details :- 
        /// </summary>
        [HttpPut("assignwagetoentity")]
        public async Task<IActionResult> AssignWageToEntity([FromBody] EntityMasterAssignWage entityMasterAssignWage)
        {
            ApiResponseModel<EntityMasterAssignWage> apiResponse = new ApiResponseModel<EntityMasterAssignWage>();
            if (entityMasterAssignWage == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            // Call the UpdateAsync method in the repository

            var jsonResult = await _repository.AssignWageToEntityAsync(DbConstants.UpdateEntityMasterAssignWage, entityMasterAssignWage);
            apiResponse.IsSuccess = entityMasterAssignWage.MessageType == 1;
            apiResponse.Message = entityMasterAssignWage.StatusMessage;
            apiResponse.MessageType = entityMasterAssignWage.MessageType;
            if (apiResponse.IsSuccess)
            {
                apiResponse.StatusCode = ApiResponseStatusConstant.Created;
            }
            else
            {
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
            }            
            
            // Return response based on success or failure
            return apiResponse.IsSuccess ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        [HttpGet("getwagegrades")]
        public IActionResult GetWageGrades()
        {
            var wageGrades = _repository.GetWageGradesForDropdown();
            return Ok(wageGrades); 
        }

    }
}
