/****************************************************************************************************
 *  Jira Task Ticket :  Payroll-593                                                                 *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for Service Master.                                     *
 *  It includes APIs to retrieve, create, update, and delete Service Master                         *
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
    public class ServiceMasterApiController : ControllerBase
    {
        private readonly IServiceMasterRepository _repository;
        public ServiceMasterApiController(IServiceMasterRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #region Service Functionality

        [HttpGet("getservicesbymoduleid/{id}")]
        public async Task<IActionResult> GetServicesByModuleId(int? id)
        {
            //int? id = 7;
            ApiResponseModel<IEnumerable<ServiceMaster>> apiResponse = new ApiResponseModel<IEnumerable<ServiceMaster>>();
            try
            {
                var Services = await _repository.GetServicesByModuleId(DbConstants.GetAllServices, new { Module_Id = id });
                if (Services != null)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = Services;
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

        [HttpGet("getservicebyid/{id}")]
        public async Task<IActionResult> GetServiceById(int? id)
        {
            //int? id = 7;
            ApiResponseModel<ServiceMaster> apiResponse = new ApiResponseModel<ServiceMaster>();
            try
            {
                var Services = await _repository.GetServiceById(id);
                if (Services != null)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = Services;
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
