using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Payroll.Common.FtpUtility;
using Payroll.WebApp.Common;
using Payroll.WebApp.Helpers;
using System.Net.Http;
using Payroll.Common.ApplicationModel;
using Payroll.WebApp.Models.DTOs;
using Payroll.Common.ViewModels;
using System.Text;
using Payroll.WebApp.Extensions;
using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper;

namespace Payroll.WebApp.Controllers.FinalDataApprovalComponent
{
    /// <summary>
    ///  Developer Name :- Abhishek Yadav
    ///  Message detail :- FinalDataApprovalController that manage fetch service wise staging table activity.
    ///  Created Date   :- 21-March-2025
    /// </summary>
    public class FinalDataApprovalController : Controller
    {
        #region CTOR
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly FtpService _ftpService;
        private readonly RestApiDataMigrationServiceHelper _migrationServiceHelper;
        int DPId = 0;
        public FinalDataApprovalController(RestApiDataMigrationServiceHelper datamigrationServiceHelper, FtpService ftpService, IOptions<ApiSettings> apiSettings, IMapper mapper)
        {
            _ftpService = ftpService;
            _apiSettings = apiSettings.Value;
            _mapper = mapper;
            _migrationServiceHelper = datamigrationServiceHelper;
        }
        #endregion        
        public async Task<IActionResult> Index()
        {
            DPId = 2; //CLSM 
            var model = await BindServiceNameAsync(); // Get the model populated with service data
            return View(model); // Pass the populated model to the view
        }

        #region CommonMethodForServiceBindDropDown
        private async Task<PayrollValidationViewModel> BindServiceNameAsync()

        {
            PayrollValidationViewModel obj = new PayrollValidationViewModel();
            #region Bind ServiceName
            string serviceNameApiUrl = _apiSettings.BaseUrlPayrollDataMigrationServiceEndpoints.GetServiceNameforApprovalProcess;
            string queryParams = $"?ServiceType={DPId}&Service_Id=0&IsActive=true";

            var serviceList = await _migrationServiceHelper.GetServiceNameAsync($"{serviceNameApiUrl}{queryParams}");

            if (serviceList != null && serviceList.Any())
            {
                // Populate the ServiceDropdown list in the model
                obj.ServiceDropdown = serviceList.Select(item => new SelectListItem
                {
                    Value = item.ServiceId.ToString(),
                    Text = item.ServiceName
                }).ToList();
            }
            else
            {
                // Handle the case where no services are returned
                obj.ServiceDropdown = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "No Services Available" }
                };
            }
            #endregion

            return obj;
        }
        #endregion
        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API retrieves all unverified service wise data for approval process get Response.
        ///  Created Date   :- 21-Mar-2025
        ///  Change Date    :- Not yet modified.
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> GetServiceData(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
                return BadRequest(new { success = false, message = "Service name is required" });

            // Construct API URL from settings
            string serviceDataApiUrl = _apiSettings.BaseUrlPayrollDataMigrationServiceEndpoints.GetServiceWisePendingApproverUrl;
            string queryParams = $"?serviceName={serviceName}";

            // Call microservice API using the existing API helper
            var apiResponse = await _migrationServiceHelper.GetReturnServiceDataAsync($"{serviceDataApiUrl}{queryParams}");

            // Extract Data and ReturnCount
            var responseData = apiResponse.dataList as List<Dictionary<string, object>> ?? new List<Dictionary<string, object>>();
            int returnCount = apiResponse.returnCount; // Prefer API's returnCount if available

            if (responseData.Any())
            {
                var columnNames = responseData.First().Keys.ToList(); // Extract column names dynamically

                return Json(new { success = true, columns = columnNames, data = responseData, returnCount });
            }

            return Json(new { success = false, message = "No data found", returnCount = 0 });
        }


        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API Update verify status in staging table and move record from staging to final table 
        ///  Created Date   :- 25-Mar-2025
        ///  Change Date    :- Not yet modified.
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> VerifyAndMoveRecords([FromBody] ServiceRequestDTO request)
        {
            try
            {
                if (request == null || request.SelectedIds == null || !request.SelectedIds.Any())
                {
                    return Json(new { success = false, message = "Invalid data received" });
                }
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
                request.UpdatedBy = userId;
                // Single API Endpoint for all services
                string apiUrl = _apiSettings.BaseUrlPayrollDataMigrationServiceEndpoints.PostApprovalDataStgingtofinal;

                var apiResponse = await _migrationServiceHelper.PostCommonAsync<ServiceRequestDTO, ApiResponseModel<object>>(apiUrl, request);

                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message, messageType = apiResponse.MessageType });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API update return status (Not Approved) in module wise staging table.
        ///  Created Date   :- 25-Mar-2025
        ///  Change Date    :- Not yet modified.
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateReturnStatus([FromBody] ServiceRequestDTO request)
        {
            try
            {
                if (request == null || request.SelectedIds == null || !request.SelectedIds.Any())
                {
                    return Json(new { success = false, message = "Invalid data received" });
                }
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
                request.UpdatedBy = userId;
                // Single API Endpoint for all services
                string apiUrl = _apiSettings.BaseUrlPayrollDataMigrationServiceEndpoints.PutApprovalDataReturnStatus;

                var apiResponse = await _migrationServiceHelper.PostCommonAsync<ServiceRequestDTO, ApiResponseModel<object>>(apiUrl, request);

                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message, messageType = apiResponse.MessageType });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API retrieves all Returned Data service wise and Downloading in CSV.
        ///  Created Date   :- 03-APR-2025
        ///  Change Date    :- Not yet modified.
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> DownloadReturnedData(int serviceId, string serviceName)
        {
            if (serviceId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid Service ID" });
            }

            // Construct API URL from microservice settings
            string serviceDataApiUrl = _apiSettings.BaseUrlPayrollDataMigrationServiceEndpoints.GetReturnedData;
            string queryParams = $"?serviceId={serviceId}";

            // Call microservice API
            var responseData = await _migrationServiceHelper.GetServiceDataAsync($"{serviceDataApiUrl}{queryParams}");

            if (responseData == null || !responseData.Any())
            {
                return NotFound(new { success = false, message = "No returned data available." });
            }

            // Convert data to CSV using CsvHelper
            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            // Write headers
            foreach (var key in responseData.First().Keys)
            {
                csv.WriteField(key);
            }
            csv.NextRecord();

            // Write rows
            foreach (var row in responseData)
            {
                foreach (var key in row.Keys)
                {
                    csv.WriteField(row[key]?.ToString() ?? "");
                }
                csv.NextRecord();
            }

            csv.Flush();

            var byteArray = Encoding.UTF8.GetBytes(writer.ToString());
            var stream = new MemoryStream(byteArray);

            return File(stream, "text/csv", $"{serviceName}_Returned_Data.csv");

        }

    }
}
