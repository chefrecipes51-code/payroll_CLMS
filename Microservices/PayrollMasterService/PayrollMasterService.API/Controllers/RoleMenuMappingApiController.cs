using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    /// <summary>
    /// Developer Name :- Harshida Parmar
    /// Created Date   :- 14-11-'24
    /// Message detail :- Provides API endpoints for managing RoleMenuMapping.
    /// </summary>
    [Route("api/")]
    [ApiController]
    public class RoleMenuMappingApiController : ControllerBase
    {
        #region Constructor 
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleMenuMappingApiController"/> class.
        /// </summary>
        /// <param name="repository">The service repository for managing Role Menu Mappung.</param>

        private readonly IRoleMenuMappingRepository _repository;
        public RoleMenuMappingApiController(IRoleMenuMappingRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #endregion
        #region RoleMenuMapping Endpoint Handlers (CRUD)

        #region RoleMenuMapping Add
        /// <summary>
        /// Handles HTTP POST requests to add a RoleMenuMapping record.
        /// </summary>
        /// <param name="RoleMenuMapping">
        /// The wage rate master details sent in the request body as JSON.
        /// </param>
        /// <returns>
        /// An IActionResult indicating the outcome of the operation. It could return a success
        /// status like 200 OK with the created RoleMenuMapping, or an error response
        /// if the input is invalid or the operation fails.
        /// </returns>
        [HttpPost("postrolemenudetails")]
        public async Task<IActionResult> PostRoleMenuDetail([FromBody] RoleMenuMappingRequest roleMenuMappingRequest)
        {
            ApiResponseModel<RoleMenuMappingRequest> apiResponse = new ApiResponseModel<RoleMenuMappingRequest>();
            // Validate the incoming request
            if (roleMenuMappingRequest == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }

            //try
            //{
            //    var result = await _repository.AddAsync(DbConstants.AddEditRoleMenu, roleMenuMappingRequest);
            //    if (result != null)
            //    {
            //        return Ok(new { message = "RoleMenuMapping successfully added.", data = result });
            //    }

            //    return Ok("Failed to add RoleMenuMapping.");
            //}
            //catch (Exception ex)
            //{

            //    return StatusCode(500, "Internal server error while processing your request.");
            //}

            await _repository.AddAsync(DbConstants.AddEditRoleMenu, roleMenuMappingRequest);
            apiResponse.IsSuccess = roleMenuMappingRequest.MessageType == 1;
            apiResponse.Message = roleMenuMappingRequest.StatusMessage;
            apiResponse.MessageType = roleMenuMappingRequest.MessageType;
            apiResponse.Data = roleMenuMappingRequest;
            apiResponse.StatusCode = roleMenuMappingRequest.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return roleMenuMappingRequest.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);

        }
        #endregion
        #region DELETE
        //WE WILL NOT PASS HEADER ID OTHRWISE IN PARENT AS WELL AS IN DETAILS SOFT DELETE WILL HAPPEN 
        [HttpDelete("deleterolemenumappingMaster/{id}")]
        public async Task<IActionResult> deleterolemenumappingMaster(int id)
        {
            RoleMenuMappingRequest obj = new RoleMenuMappingRequest();
            ApiResponseModel<RoleMenuMappingRequest> apiResponse = new ApiResponseModel<RoleMenuMappingRequest>();
            RoleMenuMappingRequest roleMenuMappingHeader = new RoleMenuMappingRequest
            {
                Header = new RoleMenuMappingHeader(), // Initialize the Header property
                Details = new List<RoleMenuDetail>() // Initialize the Details list
            };
            roleMenuMappingHeader.Header.Role_Menu_Hdr_Id = id;
            roleMenuMappingHeader.Details.Add(new RoleMenuDetail
            {
                Role_Menu_Dtl_Id = 0
            });
            roleMenuMappingHeader.Header.UpdatedBy = 1;
            // Validate input
            if (id == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }

            try
            {

                // Retrieve the existing record based on the id
                    //roleMenuDetail.Header.Role_Menu_Hdr_Id = id;
                   // Update necessary fields
                   await _repository.DeleteAsync(DbConstants.DeleteRoleMenu, roleMenuMappingHeader);

                //var statusMessage = roleMenuDetail.StatusMessage;

                // Check the result
                if (roleMenuMappingHeader.MessageType == 1)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Message = ApiResponseMessageConstant.DeleteSuccessfully;
                    apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                    return StatusCode((int)HttpStatusCode.OK, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    //apiResponse.Message = statusMessage;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return Ok(apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }

        [HttpDelete("deleterolemenumappingDetail/{id}")]
        public async Task<IActionResult> DeleteRoleMenuMappingDetail(int id)
        {
            RoleMenuMappingRequest obj = new RoleMenuMappingRequest();
            ApiResponseModel<RoleMenuMappingRequest> apiResponse = new ApiResponseModel<RoleMenuMappingRequest>();
          
            RoleMenuMappingRequest roleMenuMappingDetail = new RoleMenuMappingRequest
            {
                Header = new RoleMenuMappingHeader(), // Initialize the Header property
                Details = new List<RoleMenuDetail>() // Initialize the Details list
            };
            roleMenuMappingDetail.Header.Role_Menu_Hdr_Id = 0;
            roleMenuMappingDetail.Details.Add(new RoleMenuDetail
            {
                Role_Menu_Dtl_Id = id
            });
            roleMenuMappingDetail.Header.UpdatedBy = 1;
            // Validate input
            if (id == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }

            try
            {
                // Retrieve the existing record based on the id
                //roleMenuDetail.Header.Role_Menu_Hdr_Id = id;
                // Update necessary fields
                await _repository.DeleteAsync(DbConstants.DeleteRoleMenu, roleMenuMappingDetail);

                //var statusMessage = roleMenuDetail.StatusMessage;

                // Check the result
                if (roleMenuMappingDetail.MessageType == 1)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Message = ApiResponseMessageConstant.DeleteSuccessfully;
                    apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                    return StatusCode((int)HttpStatusCode.OK, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    //apiResponse.Message = statusMessage;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return Ok(apiResponse);
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion

        /// <summary>
        /// jira:185  Added by Chirag Gurjar  25/11/2024  
        /// </summary>
        /// <param name="id">role id</param>
        /// <param name="Company_id"> companny id</param>
        /// <param name="IsRenderInMenu">is render in menu optional</param>
        /// <returns></returns>
        /// Updated By Priyanshi Jain 10 Jan 2025
        /// Change roleId to Role_Menu_Hdr_Id and add some parameters in model.
        #region selectHierarchy
        [HttpGet("getuserrolemenubyroleId/{id}")]
        public async Task<IActionResult> GetUserRoleMenuByRoleId(int id, int Company_id, bool? IsRenderInMenu)
        {
            ApiResponseModel<IEnumerable<UserRoleMenu>> apiResponse = new ApiResponseModel<IEnumerable<UserRoleMenu>>();
            var userRoleMenu = await _repository.GetRoleMenuByIdAsync(DbConstants.GetUserRoleMenuByRoleId, new { Role_Menu_Hdr_Id = id, Company_Id = Company_id, IsRenderInMenu = IsRenderInMenu });

            if (userRoleMenu == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = userRoleMenu;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        #endregion
        #endregion
    }
}
