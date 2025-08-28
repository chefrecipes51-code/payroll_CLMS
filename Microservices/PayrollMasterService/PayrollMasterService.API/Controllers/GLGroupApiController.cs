/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-1042                                                                 *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for GeneralAccounting entries.                          *
 *  It includes APIs to retrieve, create, update, and delete GeneralAccounting                      *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllGLGroups : Retrieves all GLGroups records.                                              *
 *  - GetGLGroupById: Retrieves a specific GLGroups by ID.                                          *
 *  - GetParentOrSubGroups: Retrieves a specific GLGroups by ID.                                    *
 *  - PostGLGroup   : Adds a new GLGroups record.                                                   *
 *  - PutGLGroup    : Updates an existing GLGroups record.                                          *
 *  - DeleteGLGroup : Soft-deletes an GLGroups record.                                              *
 *                                                                                                  *
 *  Author: HARSHIDA PARMAR                                                                         *
 *  Date  : 26-June-2025                                                                            *
 *                                                                                                  *
 ****************************************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Requests;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class GLGroupApiController : ControllerBase
    {
        private readonly IGLGroupRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;

        public GLGroupApiController(IGLGroupRepository repository, ApiKeyValidatorHelper apiKeyValidatorHelper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidatorHelper;
        }

        [HttpGet("getallglgroups")]
        public async Task<IActionResult> GetAllGLGroups([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<GlGroupRequest>>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            var result = await _repository.GetAllAsync(DbConstants.GetGLGroupMaster);

            if (result != null && result.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = result;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }

            apiResponse.IsSuccess = false;
            apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
            return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
        }

        [HttpGet("getglgroupbyid/{id}")]
        public async Task<IActionResult> GetGLGroupById([FromHeader(Name = "X-API-KEY")] string apiKey, int id)
        {
            var apiResponse = new ApiResponseModel<GlGroupRequest>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            var result = await _repository.GetByIdAsync(DbConstants.GetGLGroupMasterById, new { GL_Group_Id = id });

            if (result == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = result;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        [HttpGet("getparentsubglgroups/{parentId}")]
        public async Task<IActionResult> GetParentOrSubGroups(int parentId)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<GlGroupRequest>>();

            var result = await _repository.GetParentOrSubGLGroupsAsync("Sp_Select_Parent_Sub_GL_Group", parentId);

            if (result != null && result.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = result;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }

            apiResponse.IsSuccess = false;
            apiResponse.Message = "No records found.";
            apiResponse.StatusCode = ApiResponseStatusConstant.NoContent;
            return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
        }

        [HttpPost("postglgroup")]
        public async Task<IActionResult> PostGLGroup([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] GlGroupRequest model)
        {
            var apiResponse = new ApiResponseModel<GlGroupRequest>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            if (model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            await _repository.AddAsync(DbConstants.AddEditGLGroup, model);

            apiResponse.IsSuccess = model.MessageType == 1;
            apiResponse.Result = model;
            apiResponse.Message = model.StatusMessage;
            apiResponse.MessageType = model.MessageType;
            apiResponse.StatusCode = model.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;

            return model.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        [HttpPut("updateglgroup/{id}")]
        public async Task<IActionResult> PutGLGroup([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] GlGroupRequest model)
        {
            var apiResponse = new ApiResponseModel<GlGroupRequest>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            if (id <= 0 || model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            model.GL_Group_Id = id;
            await _repository.UpdateAsync(DbConstants.AddEditGLGroup, model);

            apiResponse.IsSuccess = model.MessageType == 1;
            apiResponse.Result = model;
            apiResponse.Message = model.StatusMessage;
            apiResponse.MessageType = model.MessageType;
            apiResponse.StatusCode = model.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;

            return model.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }

        [HttpDelete("deleteglgroup/{id}")]
        public async Task<IActionResult> DeleteGLGroup([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] GlGroupRequest model)
        {
            var apiResponse = new ApiResponseModel<GlGroupRequest>();

            if (!_apiKeyValidatorHelper.Validate(apiKey))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            if (id <= 0 || model == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            model.GL_Group_Id = id;
            await _repository.DeleteAsync(DbConstants.DeleteGLGroup, model);

            apiResponse.IsSuccess = model.MessageType == 1;
            apiResponse.Message = model.StatusMessage;
            apiResponse.MessageType = model.MessageType;
            apiResponse.StatusCode = model.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;

            return model.MessageType == 1
                ? StatusCode((int)HttpStatusCode.Created, apiResponse)
                : Ok(apiResponse);
        }
    }
}
