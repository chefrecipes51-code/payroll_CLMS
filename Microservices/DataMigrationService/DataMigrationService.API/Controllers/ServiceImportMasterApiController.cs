/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-239                                                                  *
 *  Description:                                                                                    *
 *  This controller handles operations for serviceImportMaster entries.                             *
 *  It includes APIs to retrieve, serviceImportMaster                                               *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetServiceImportMasterById: Retrieves a specific ServiceImportMaster record by ID.            *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 03-Dec-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
using DataMigrationService.BAL.Models;
using DataMigrationService.DAL.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using Dapper;
using System.Reflection;

namespace DataMigrationService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ServiceImportMasterApiController : ControllerBase
    {
        private readonly IServiceImportMasterRepository _repository;

        private readonly IConfiguration _configuration;
        private readonly string _connection;
        public ServiceImportMasterApiController(IServiceImportMasterRepository repository, IConfiguration configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _configuration = configuration;
            _connection = configuration.GetConnectionString("DefaultConnection");
        }
        #region Service Import Master APIs Functionality
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API to show Data in the API details based on the provided organization data.
        ///  Created Date   :- 03-Dec-2024
        ///  Change Date    :- 03-Dec-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns an API response with service Import Master details or an error message.</returns>
        [HttpGet("getserviceimportmasterbyid")]
        public async Task<IActionResult> GetServiceImportMasterById(int ServiceType, int Service_Id, bool IsActive)
        {
            ApiResponseModel<ServiceImportMaster> apiResponse = new ApiResponseModel<ServiceImportMaster>();
            // Fetch JSON result directly from the stored procedure
            var result = await _repository.GetServiceImportAsync(DbConstants.GetServiceImportMasterById, new ServiceImportMaster { ServiceType = ServiceType, Service_Id = Service_Id, IsActive = IsActive });            
            // Check if data exists
            if (result != null && result.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Data = result;
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

        #region Service Master APIs Functionality
        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API to show Data in the API details based on the Module data.
        ///  Created Date   :- 20-March-2025
        ///  Change Date    :- Not yet modified
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns an API response with service Master details or an error message.</returns>
        [HttpGet("getservicemasterbyModuleId")]
        public async Task<IActionResult> GetServiceMasterById(int ModuleId)
        {
            ApiResponseModel<ServiceMaster> apiResponse = new ApiResponseModel<ServiceMaster>();
            // Fetch JSON result directly from the stored procedure
            var result = await _repository.GetServiceAsync(DbConstants.GetServiceMaster, new ServiceMaster { ModuleID = ModuleId });
            // Check if data exists
            if (result != null && result.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Data = result;
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

        #region Service wise Fetch Staging Data APIs Functionality
        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API to Get Service wise Pending data for approval.
        ///  Created Date   :- 24-March-2025
        ///  Change Date    :- Not yet modified
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns an API response with service Master details or an error message.</returns>
        [HttpGet("GetSesrviceWiseList")]
        public async Task<IActionResult> GetData(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
                return BadRequest(new { success = false, message = "Service name is required" });

            // Call the repository method using the stored procedure name from your DbConstants
            var (data, returnCount) = await _repository.GetServiceDataAsync(DbConstants.GetGenericServiceData, new { ServiceName = serviceName });

            if (data == null || !data.Any())
                return NotFound(new { success = false, message = "No data found", returnCount });

            return Ok(new { success = true, data, returnCount });
        }

        #endregion

        #region Post Approval data staging to Final

        [HttpPost("postapprovaldatastagingtoFinal")]
        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API is for Posting data from staging to Final Data.
        ///  Created Date   :- 24-March-2025
        ///  Change Date    :- Not yet modified
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns an API response with service Master details or an error message.</returns>
        public async Task<IActionResult> postapprovaldatastagingtoFinal([FromBody] ServiceMaster request)
        {
            ApiResponseModel<object> apiResponse = new ApiResponseModel<object>();

            if (request == null || request.SelectedIds == null || !request.SelectedIds.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid data received.";
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@ServiceId", request.ServiceId);
            parameters.Add("@UpdatedBy", request.UpdatedBy);
            parameters.Add("@IsReturn", 0);
            // Ensure UDT parameter is correctly passed
            var selectedIdsTable = ConvertToUDT(request.SelectedIds);
            parameters.Add("@SelectedIds", selectedIdsTable.AsTableValuedParameter("UDT_Service"));
            //parameters.Add("@SelectedIds", ConvertToUDT(request.SelectedIds));

            // Call Parent SP that triggers Child SP based on ServiceId
            var response =  await _repository.AddExecuteAsync(DbConstants.ValidateApprovalMigrationToFinal, parameters);
            if (response != null && response.Any())
            {
                var firstRow = response.First() as IDictionary<string, object>;

                if (firstRow != null)
                {
                    apiResponse.Message = firstRow.ContainsKey("ApplicationMessage") ? firstRow["ApplicationMessage"].ToString() : "Unknown message";
                    apiResponse.MessageType = firstRow.ContainsKey("ApplicationMessageType") ? Convert.ToInt32(firstRow["ApplicationMessageType"]) : 0;
                }
                else
                {
                    apiResponse.Message = "No response from stored procedure.";
                    apiResponse.StatusCode = 0;
                }
            }
            apiResponse.IsSuccess = true;
            return Ok(apiResponse);
        }
        #endregion

        #region Updating Return status during approval process
        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API is for updating retrun stauts in staging table.
        ///  Created Date   :- 02-APR-2025
        ///  Change Date    :- Not yet modified
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns an API response with service Master details or an error message.</returns>
        [HttpPost("putapprovaldatareturnstatus")]
        public async Task<IActionResult> putapprovaldatareturnstatus([FromBody] ServiceMaster request)
        {
            ApiResponseModel<object> apiResponse = new ApiResponseModel<object>();

            if (request == null || request.SelectedIds == null || !request.SelectedIds.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid data received.";
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@UpdatedBy", request.UpdatedBy);
            parameters.Add("@ServiceId", request.ServiceId);
            parameters.Add("@IsReturn", 1);

            // Ensure UDT parameter is correctly passed
            var selectedIdsTable = ConvertToUDT(request.SelectedIds);
            parameters.Add("@SelectedIds", selectedIdsTable.AsTableValuedParameter("UDT_Service"));
            //parameters.Add("@SelectedIds", ConvertToUDT(request.SelectedIds));

            // Call Parent SP that triggers Child SP based on ServiceId
            var response = await _repository.AddExecuteAsync(DbConstants.ValidateApprovalMigrationToFinal, parameters);
            if (response != null && response.Any())
            {
                var firstRow = response.First() as IDictionary<string, object>;

                if (firstRow != null)
                {
                    apiResponse.Message = firstRow.ContainsKey("ApplicationMessage") ? firstRow["ApplicationMessage"].ToString() : "Unknown message";
                    apiResponse.MessageType = firstRow.ContainsKey("ApplicationMessageType") ? Convert.ToInt32(firstRow["ApplicationMessageType"]) : 0;
                }
                else
                {
                    apiResponse.Message = "No response from stored procedure.";
                    apiResponse.StatusCode = 0;
                }
            }
            apiResponse.IsSuccess = true;
            return Ok(apiResponse);
        }
        private DataTable ConvertToUDT(List<int> selectedIds)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach (var id in selectedIds)
            {
                dt.Rows.Add(id);
            }

            return dt;
        }
        #endregion

        #region Get Return List Data
        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API is for Get list data with return status 1.
        ///  Created Date   :- 03-APR-2025
        ///  Change Date    :- Not yet modified
        ///  Change detail  :- Not yet modified.
        /// </summary>
        [HttpGet("GetReturnedData")]
        public async Task<IActionResult> GetReturnedData(int serviceId)
        {
            if (serviceId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid Service ID" });
            }

            // Use the repository to fetch returned data.
            var result = await _repository.GetReturnedDataAsync(DbConstants.GetReturnedMigrationData, new { ServiceId = serviceId });

            if (result == null || !result.Any())
            {
                return NotFound(new { success = false, message = "No returned data available." });
            }

            return Ok(new { success = true, data = result });
        }
        #endregion


    }
}