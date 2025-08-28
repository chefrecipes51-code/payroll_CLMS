using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;

namespace PayrollMasterService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class EntityTaxStatutoryApiController : ControllerBase
    {

        private readonly IEntityTaxStatutoryRepository _repository;

        public EntityTaxStatutoryApiController(IEntityTaxStatutoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        ///  Developer Name :- Krunali Gohil
        ///  Message detail :- This API retrieves all Entity Tax Statutory details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 30-January-2025
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Company statutory or an appropriate message</returns>

        [HttpGet("getallentitytaxstatutory")]
        public async Task <IActionResult> GetAllEntityTaxStatutory()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<EntityTaxStatutory>>();
            var entityTaxStatutory = await _repository.GetAllAsync(DbConstants.GetAllEntityTaxStatutory);

            if (entityTaxStatutory != null && entityTaxStatutory.Any())
            {
                apiResponse.IsSuccess= true;
                apiResponse.Result= entityTaxStatutory;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }

        [HttpGet("getentitytaxstatutorybyid/{id}")]
        public async Task<IActionResult> GetEntityTaxStatutoryById(int id)
        {
            ApiResponseModel<EntityTaxStatutory> apiResponse = new ApiResponseModel<EntityTaxStatutory>();
            var entityTaxStatuatory = await _repository.GetByIdAsync(DbConstants.GetAllEntityTaxStatutory, new { Entity_Statutory_Id = id });
            if (entityTaxStatuatory == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = entityTaxStatuatory;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Krunali Gohil
        ///  Message detail :- This API add record of Entity Tax Statutory in the database table
        ///                    using a stored procedure. 
        ///  Created Date   :- 30-January-2025
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with appropriate message</returns>

        [HttpPost("postentitytaxstatutory")]
        public async Task<IActionResult> PostEntityTaxStatutory([FromBody]EntityTaxStatutory entityTaxStatutory)
        {
            ApiResponseModel<EntityTaxStatutory> apiResponse = new ApiResponseModel<EntityTaxStatutory>();
            if (entityTaxStatutory == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
           
            var jsonResult = await _repository.AddAsync(DbConstants.AddEditEntityTaxStatutory, entityTaxStatutory);
            if(entityTaxStatutory.MessageType ==1 )
            {
                apiResponse.IsSuccess = true;
                apiResponse.Message = ApiResponseMessageConstant.CreatedSuccessfully;
                apiResponse.MessageType = entityTaxStatutory.MessageType;
                apiResponse.StatusCode= ApiResponseStatusConstant.Created;
                return StatusCode((int)HttpStatusCode.Created,apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message= entityTaxStatutory.StatusMessage;
                apiResponse.MessageType = entityTaxStatutory.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);

            }
        }

        /// <summary>
        ///  Developer Name :- Krunali Gohil
        ///  Message detail :- This API update record of Entity Tax Statutory in the database table
        ///                    using a stored procedure with particular Id. 
        ///  Created Date   :- 30-January-2025
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with appropriate message</returns>
        
        [HttpPut("updateentitytaxstatutory/{id}")]
        public async Task<IActionResult> PutEntityTaxStatutory(int id,[FromBody] EntityTaxStatutory entityTaxStatutory)
        {
            ApiResponseModel<EntityTaxStatutory> apiResponse = new ApiResponseModel<EntityTaxStatutory>();
            if (entityTaxStatutory == null && id<=0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);      
            }
            entityTaxStatutory.Entity_statutory_Id = id;
            await _repository.UpdateAsync(DbConstants.AddEditEntityTaxStatutory, entityTaxStatutory);
            apiResponse.IsSuccess = entityTaxStatutory.MessageType ==1;
            apiResponse.Message = entityTaxStatutory.StatusMessage;
            apiResponse.MessageType = entityTaxStatutory.MessageType;
            apiResponse.StatusCode = entityTaxStatutory.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return entityTaxStatutory.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Krunali Gohil
        ///  Message detail :- This API Delete record of Entity Tax Statutory in the database table
        ///                    using a stored procedure with particular Id. 
        ///  Created Date   :- 30-January-2025
        ///  Last Modified  :- 
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with appropriate message</returns>
        
        [HttpDelete("deleteentitytaxstatutory/{id}")]
        public async Task<IActionResult> DeleteEntityTaxStatutory(int id,[FromBody] EntityTaxStatutory entityTaxStatutory)
        {
            ApiResponseModel<EntityTaxStatutory> apiResponse = new ApiResponseModel<EntityTaxStatutory>();
            if(entityTaxStatutory == null && id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            entityTaxStatutory.Entity_statutory_Id = id;
            await _repository.DeleteAsync(DbConstants.DeleteEntityTaxStatutory, entityTaxStatutory);
            apiResponse.IsSuccess = entityTaxStatutory.MessageType == 1;
            apiResponse.Message = entityTaxStatutory.StatusMessage;
            apiResponse.MessageType= entityTaxStatutory.MessageType;
            apiResponse.StatusCode = entityTaxStatutory.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return entityTaxStatutory.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);

        }
    }
}
