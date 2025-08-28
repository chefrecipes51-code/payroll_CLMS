using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using System.Net;
using UserService.BAL.Models;
using UserService.DAL.Interface;

namespace UserService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserTypeMasterApiController : ControllerBase
    {
        private readonly IUserTypeMasterRepository _repository;
        public UserTypeMasterApiController(IUserTypeMasterRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #region User Core Functionality

        [HttpGet("getallusertypemaster")]
        public async Task<IActionResult> GetAllUserTypeMaster()
        {
            ApiResponseModel<IEnumerable<UserTypeMaster>> apiResponse = new ApiResponseModel<IEnumerable<UserTypeMaster>>();
            try
            {
                var countries = await _repository.GetAllAsync(DbConstants.GetAllUserTypeMaster);
                if (countries != null)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = countries;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = MessageConstants.DataNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return NotFound(apiResponse);
                }
            }
            catch
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion
    }
}
