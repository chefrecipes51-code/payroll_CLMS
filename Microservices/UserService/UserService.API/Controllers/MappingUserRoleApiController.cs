/****************************************************************************************************
 *  This controller handles CRUD operations for MappingUserRole entries.                                 
 *  It includes APIs to retrieve, create, update, and delete MappingUserRole                             
 *  records using the repository pattern and stored procedures.                                    
 *                                                                                                  
 *  Methods:                                                                                       
 *  - GetAllMappingUserRole : Retrieves all WageRateMappingUserRoleMaster records.                                         
 *  - GetMappingUserRoleById: Retrieves a specific MappingUserRole record by ID.                              
 *  - PostMappingUserRole   : Adds a new MappingUserRole record.                                             
 *  - PutMappingUserRole    : Updates an existing MappingUserRole record.                                     
 *  - DeleteMappingUserRole : Soft-deletes an MappingUserRole record.                                         
 *                                                                                                  
 *  Author: Harshida Parmar                                                                    
 *  Date  : 13-11-2024                                                                            
 *                                                                                                  
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
    public class MappingUserRoleApiController : ControllerBase
    {
        #region CTOR And Private Variable
        private readonly IMappingUserRoleRepository _repository;
        public MappingUserRoleApiController(IMappingUserRoleRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #endregion
        #region Mapping User Role Endpoint Handlers (CRUD)
        #region Mapping User Role Fetch All And By ID  
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- This API retrieves all AllUserRoleMaster details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 15-Oct-2024
        ///  Last Modified  :- 15-Oct-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with  AllUserRoleMaster details or an appropriate message</returns>
        [HttpGet("getalluserrole")]
        public async Task<IActionResult> GetAllUserRoleMaster([FromQuery] int? roleUserId, [FromQuery] bool? isActive)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<UserRoleMappingRequest>>();
            // Fetching data from the repository by executing the stored procedure
            var UserRoleMaster = await _repository.GetAllUserRoleMappingsAsync(DbConstants.GetMapUserRoleMaster, roleUserId, isActive);
            // Check if data exists
            if (UserRoleMaster != null && UserRoleMaster.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = UserRoleMaster;
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
        /// Developer Name: Harshida Parmar
        /// Message Detail: API to retrieve  AllUserRoleMaster details based on the provided AssignRole_Id. 
        /// This method fetches data from the repository and returns the tbl_map_userrole detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 15-Oct-2024
        /// Change Date: 15-Oct-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the tbl_map_userrole to retrieve</param>
        /// <returns>Returns an API response with  AllUserCompanyMaster details or an error message.</returns>
        [HttpGet("getalluserrolemasterbyid/{id}")]
        public async Task<IActionResult> GetAllUserRoleMasterById(int id)
        {
            ApiResponseModel<UserRoleMappingRequest> apiResponse = new ApiResponseModel<UserRoleMappingRequest>();
            var UserRoleMaster = await _repository.GetUserRoleMappingByIdAsync(DbConstants.GetMapUserRoleMasterById, new { Role_User_Id = id });
            if (UserRoleMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = UserRoleMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        #endregion
        #region Mapping User Role Add
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- This API handles the addition of  AllUserRoleMaster details based on the provided organization data.
        ///  Created Date   :- 15-Oct-2024
        ///  Change Date    :- 15-Oct-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="UserRoleMapModel"> AllUserRoleMapModel detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postuserrolemaster")]
        public async Task<IActionResult> PostUserRoleMaster([FromBody] UserRoleMapping mappingUserRole)
        {
            ApiResponseModel<UserRoleMapping> apiResponse = new ApiResponseModel<UserRoleMapping>();
            if (mappingUserRole == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            try
            {
                // Call the repository method
                var result = await _repository.AddUserRoleMappingAsync(DbConstants.AddEditMapUserRoleMaster, mappingUserRole);

               
                if (result.MessageType == 1) 
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Message = result.StatusMessage;
                    apiResponse.MessageType = result.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.Created;
                    return StatusCode((int)HttpStatusCode.Created, apiResponse);
                }
                else 
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = result.StatusMessage;
                    apiResponse.MessageType = result.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return BadRequest(apiResponse);
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                apiResponse.IsSuccess = false;
                apiResponse.Message = "An error occurred while processing your request.";
                apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion
        #region Mapping User Role Delete
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- Updates the delete column based on the provided UserRoleMaster and ID. 
        ///                    If the ID does not match or UserRoleMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 15-Oct-2024
        ///  Last Updated   :- 15-Oct-2024
        ///  Change Details :- Initial implementation.
        /// </summary>
        /// <param name="id">The ID of the UserRoleMaster to update.</param>
        /// <param name="mappingUserRole">The User Role Master detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deleteuserrolemaster/{id}")]
        public async Task<IActionResult> DeleteUserRoleMaster(int id, [FromBody] UserRoleMapping mappingUserRole)
        {
            ApiResponseModel<UserRoleMapping> apiResponse = new ApiResponseModel<UserRoleMapping>();
            // Check if the provided mappingUserRoleDetail is null or the id doesn't match the AssignRole_Id
            if (mappingUserRole == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);          
            }
            mappingUserRole.Role_User_Id = id;
            var result = await _repository.DeleteUserRoleMappingAsync(DbConstants.DeleteMapUserRoleMaster, mappingUserRole);

            if (result.MessageType == 1)
            {
                apiResponse.IsSuccess = true;
                apiResponse.Message = result.StatusMessage;
                apiResponse.MessageType = result.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                return StatusCode((int)HttpStatusCode.OK, apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = result.StatusMessage;
                apiResponse.MessageType = result.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
        }
        #endregion
        #region Mapping User Role Update
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- Updates the  tbl_map_userrole detail based on the provided AssignRole_Id. 
        ///                    If the ID does not match or tbl_map_userrole is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 15-Oct-2024
        ///  Last Updated   :- 15-Oct-2024
        ///  Change Details :- Initial implementation.
        /// </summary>
        /// <param name="id">The ID of the  tbl_map_userrole to update.</param>
        /// <param name="updateuserrolemaster">The  tbl_map_userrole detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>

        #region Mapping User Role Update
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- This API handles the update of AllUserRoleMaster details based on the provided organization data.
        ///  Created Date   :- 15-Oct-2024
        ///  Change Date    :- 15-Oct-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="id">The ID of the UserRoleMapping to update.</param>
        /// <param name="mappingUserRole">The updated UserRoleMapping details.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPut("updateuserrolemaster/{id}")]
        public async Task<IActionResult> UpdateUserRoleMaster(int id, [FromBody] UserRoleMapping mappingUserRole)
        {
            ApiResponseModel<UserRoleMapping> apiResponse = new ApiResponseModel<UserRoleMapping>();

            // Validate input
            if (id <= 0 || mappingUserRole == null || id != mappingUserRole.Role_User_Id)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            mappingUserRole.Role_User_Id = id;
            try
            {
                // Call the repository method
                var result = await _repository.UpdateUserRoleMappingAsync(DbConstants.AddEditMapUserRoleMaster, mappingUserRole);

                if (result.MessageType == 1)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Message = result.StatusMessage;
                    apiResponse.MessageType = result.MessageType;
                    //apiResponse.StatusCode = ApiResponseStatusConstant.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = result.StatusMessage;
                    apiResponse.MessageType = result.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return BadRequest(apiResponse);
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                apiResponse.IsSuccess = false;
                apiResponse.Message = "An error occurred while processing your request.";
                apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion

        #endregion
        #endregion
        #region User Role Based Menu Endpoint Handlers (CRUD)
        ///
        /// Note:- Date 10-01-'25:=- As per Abhishek Input no need to create separate Controller so Created seprate Region
        ///
        #region Fetch User Role Based Menu 
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- This API retrieves all AllUserRoleBasedMenu details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 10-Jan-'25
        ///  Last Modified  :- 
        ///  Modification   :- 
        /// </summary>
        /// <returns>A JSON response with  AllUserRoleBasedMenu details or an appropriate message</returns>
        [HttpGet("getalluserrolemenu")]
        public async Task<IActionResult> GetAllUserRoleMenu([FromQuery] int? companyid, [FromQuery] int? roleid, [FromQuery] int? userid, [FromQuery] int? userMapLocationId)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<UserRoleBasedMenuRequest>>();
            // Fetching data from the repository by executing the stored procedure
            var UserRoleMenu = await _repository.GetAllUserRoleMenuAsync(DbConstants.GetUserUserRoleBasedMenu, companyid, roleid, userid, userMapLocationId);
            // Check if data exists
            if (UserRoleMenu != null && UserRoleMenu.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = UserRoleMenu;
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

        #endregion

        #region Fetch User Role Based Menu Edit
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all AllUserRoleBasedMenu details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 21 Feb 2025
        ///  Last Modified  :- 
        ///  Modification   :- 
        /// </summary>
        /// <returns>A JSON response with  AllUserRoleBasedMenu details or an appropriate message</returns>
        [HttpGet("getalluserrolemenuedit")]
        public async Task<IActionResult> GetAllUserRoleMenuEdit([FromQuery] int? companyid, [FromQuery] int? roleid, [FromQuery] int? userid, [FromQuery] int? rolemenuheaderid, [FromQuery] int? correspondanceId)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<UserRoleBasedMenuRequest>>();
            // Fetching data from the repository by executing the stored procedure
            var UserRoleMenu = await _repository.GetAllUserRoleMenuEditAsync(DbConstants.GetUserUserRoleBasedMenuEdit, companyid, null, userid, rolemenuheaderid,correspondanceId);
            // Check if data exists
            if (UserRoleMenu != null && UserRoleMenu.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = UserRoleMenu;
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

        #endregion
        #endregion

        [HttpGet("getbreadcrumbbymenuid")]
        public async Task<IActionResult> GetBreadcrumbByMenuId([FromQuery] int? menu_Id)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<BreadCrumbMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var breadcrumb = await _repository.GetBreadcrumbByMenuIdAsync(DbConstants.GetBreadcrumbByMenuId, new { Menu_Id = menu_Id } );
            // Check if data exists
            if (breadcrumb != null && breadcrumb.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = breadcrumb;
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
    }
}
