/****************************************************************************************************
 *  Jira Task Ticket :  Payroll-593                                                                 *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for Module Master.                                      *
 *  It includes APIs to retrieve, create, update, and delete Module Master                          *
 *  Author: Chirag Gurjar                                                                           *
 *  Date  : 16-Mar-2025                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using System.Net;
using UserService.BAL.Models;
using UserService.BAL.Requests;
using UserService.DAL.Interface;

namespace UserService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ModuleMasterApiController : ControllerBase
    {
        private readonly IModuleMasterRepository _repository;
        public ModuleMasterApiController(IModuleMasterRepository repository) 
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #region Counrty Functionality

       [HttpGet("getallmodules")]
        public async Task<IActionResult> GetAllModules()
        {
            ApiResponseModel<IEnumerable<ModuleMaster>> apiResponse = new ApiResponseModel<IEnumerable<ModuleMaster>>();
            try
            {
                var modules = await _repository.GetAllAsync(DbConstants.GetAllModules);
                if (modules != null)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = modules;
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
