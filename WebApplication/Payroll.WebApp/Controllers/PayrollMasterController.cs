using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.Extensions.Options;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Extensions;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using System.Collections;
using System.Net;
using UserService.BAL.Requests;
using static Payroll.WebApp.Helpers.SessionHelper;

namespace Payroll.WebApp.Controllers
{
    [ServiceFilter(typeof(MenuAuthorizationFilter))]
    public class PayrollMasterController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        // Property to get UserId from Session
        private int SessionUserId
        {
            get
            {
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                return int.TryParse(sessionData?.UserId, out var parsedUserId) ? parsedUserId : 0;
            }
        }
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }
        public PayrollMasterController(RestApiMasterServiceHelper masterServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _masterServiceHelper = masterServiceHelper;
        }

        #region Area Master Crud Functionality
        [HttpGet]
        public async Task<IActionResult> OrgIndex()
        {
            await SetUserPermissions(); //Added By Chirag
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AreaList([FromBody] AreaDTO areaDTO)
        {
            try
            {
                await SetUserPermissions(); //Added By Chirag
                // Map UserDTO to UserRequest using AutoMapper
                areaDTO.CreatedBy = SessionUserId; // Assuming this is hardcoded or passed from session/user context
                areaDTO.IsActive = true;
                var areaRequest = _mapper.Map<AreaMaster>(areaDTO);
                // Define API URL
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostAreaMasterDetailUrl;
                // Call the generic PostAsync method to post company data
                var apiResponse = await _masterServiceHelper
                                    .PostCommonAsync<AreaMaster, AreaDTO>(apiUrl, areaRequest);
                //// Handle response
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
                // Pass the locationList directly to the PartialView
                //return PartialView("_AreaListPartial", apiResponse.Result);
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> FetchAreaList()
        {
            await SetUserPermissions();
            var response = new ApiResponseModel<AreaDTO> { IsSuccess = false };
            string apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllAreaMasterUrl;
            // Call the dynamic GetAsync method to fetch the data
            var apiResponse = await _masterServiceHelper.GetCommonAsync<List<AreaDTO>>(apiUrl);
            // Extract the data from the ApiResponseModel
            var areaList = apiResponse.Result; // assuming 'Data' contains the list of LocationMasterDTOs

            // Pass the locationList directly to the PartialView
            return PartialView("_AreaListPartial", areaList);
        }

        [Route("PayrollMaster/[action]/{areaId}")]
        [HttpGet]
        public async Task<IActionResult> GetAreaDetailsById([FromRoute] int areaId)
        {
            try
            {
                var getApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetAreaMasterDetailByIdUrl}/{areaId}";
                var response = await _masterServiceHelper.GetByIdListCommonAsync<AreaDTO>(getApiUrl, areaId);

                if (!response.IsSuccess || response.Result == null)
                {
                    return Json(new { success = false, message = response.Message });
                }
                return Json(new { success = true, data = response.Result });
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                // Log error
                return Json(new { success = false, message = "An error occurred while fetching area details." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateArea([FromBody] AreaDTO areaDTO)
        {
            await SetUserPermissions(); //Added By Chirag
            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
            if (areaDTO == null || string.IsNullOrWhiteSpace(areaDTO.AreaName) || areaDTO.Location_Id == 0)
            {
                return Json(new { success = false, message = "Invalid data. Please check your input." });
            }
            try
            {
                areaDTO.UpdatedBy = SessionUserId;
                areaDTO.UpdatedDate = DateTime.Now;
                // Assuming you are updating the area using a service or repository
                var updateApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.PutAreaMasterDetailUrl}/{areaDTO.Area_Id}";
                var updateResponse = await _masterServiceHelper.PutCommonAsync<AreaDTO, AreaMaster>(updateApiUrl, areaDTO);

                if (updateResponse.IsSuccess)
                {
                    return Json(new { success = true, message = updateResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = updateResponse.Message ?? "Failed to update the area." });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                // Optionally log the exception details
                Console.WriteLine($"Exception in UpdateArea: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the area." });
            }
        }
        public async Task<IActionResult> DeleteArea([FromBody] AreaDTO model)
        {
            await SetUserPermissions(); //Added By Chirag
            try
            {
                // Set UpdatedBy from session
                model.UpdatedBy = SessionUserId;

                // Construct API URL
                var deleteApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.DeleteAreaMasterDetailUrl}/{model.Area_Id}";

                // Call the common delete method (now sending the request body)
                var deleteResponse = await _masterServiceHelper.DeleteCommonAsync<AreaDTO, AreaMaster>(deleteApiUrl, model);

                if (deleteResponse != null && deleteResponse.IsSuccess)
                {
                    return Json(new { success = true, message = deleteResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = deleteResponse.Message ?? "Failed to delete the area." });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while deleting the area." });
            }
        }


        #endregion

        #region Location Master Crud Functionality
        public async Task<IActionResult> LoadLocationPartial()
        {
            await SetUserPermissions();
            var response = new ApiResponseModel<LocationMasterDTO> { IsSuccess = false };
            string apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllLocationMasterUrl;

            // Call the dynamic GetAsync method to fetch the data
            var apiResponse = await _masterServiceHelper.GetCommonAsync<List<LocationMasterDTO>>(apiUrl);

            // Extract the data from the ApiResponseModel
            var locationList = apiResponse.Result; // assuming 'Data' contains the list of LocationMasterDTOs

            // Pass the locationList directly to the PartialView
            return PartialView("_LocationListPartialView", locationList);
        }

        [HttpGet]
        public async Task<IActionResult> GetLocationDetails(int id)
        {
            try
            {
                var getApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetLocationMasterDetailByIdUrl}/{id}";
                var response = await _masterServiceHelper.GetByIdCommonAsync<LocationMasterDTO>(getApiUrl, id);

                if (!response.IsSuccess || response.Result == null)
                {
                    return Json(new { success = false, message = response.Message });
                }
                return Json(new { success = true, data = response.Result });
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while loading the loaction." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation([FromBody] LocationMasterDTO locationDTO)
        {
            await SetUserPermissions(); //Added By Chirag
            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
            if (locationDTO == null || string.IsNullOrWhiteSpace(locationDTO.LocationName))
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {

                locationDTO.CreatedBy = SessionUserId; // Assuming this is hardcoded or passed from session/user context
                locationDTO.CreatedDate = DateTime.Now;
                locationDTO.IsActive = true;

                var areaRequest = _mapper.Map<LocationMaster>(locationDTO);
                // Define API URL
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostLocationMasterDetailUrl;
                // Call the generic PostAsync method to post company data
                var apiResponse = await _masterServiceHelper
                                    .PostCommonAsync<LocationMaster, LocationMasterDTO>(apiUrl, areaRequest);
                // Handle response
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                // Log exception if necessary
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationMasterDTO locationMasterDTO)
        {
            await SetUserPermissions(); //Added By Chirag
            if (locationMasterDTO == null || string.IsNullOrWhiteSpace(locationMasterDTO.LocationName) || locationMasterDTO.Location_Id == 0)
            {
                return Json(new { success = false, message = "Invalid data. Please check your input." });
            }
            try
            {
                locationMasterDTO.UpdatedBy = SessionUserId;
                locationMasterDTO.UpdatedDate = DateTime.Now;
                // Assuming you are updating the area using a service or repository
                var updateApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.PutLocationMasterDetailUrl}/{locationMasterDTO.Location_Id}";
                var updateResponse = await _masterServiceHelper.PutCommonAsync<LocationMasterDTO, LocationMaster>(updateApiUrl, locationMasterDTO);

                if (updateResponse.IsSuccess)
                {
                    return Json(new { success = true, message = updateResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = updateResponse.Message ?? "Failed to update the location." });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the location." });
            }
        }

        public async Task<IActionResult> DeleteLocation([FromBody] LocationMasterDTO model)
        {
            await SetUserPermissions(); //Added By Chirag
            try
            {
                // Set UpdatedBy from session
                model.UpdatedBy = SessionUserId;

                // Construct API URL
                var deleteApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.DeleteLocationMasterDetailUrl}/{model.Location_Id}";

                // Call the common delete method (now sending the request body)
                var deleteResponse = await _masterServiceHelper.DeleteCommonAsync<LocationMasterDTO, LocationMaster>(deleteApiUrl, model);

                if (deleteResponse != null && deleteResponse.IsSuccess)
                {
                    return Json(new { success = true, message = deleteResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = deleteResponse.Message ?? "Failed to delete the location." });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while deleting the loaction." });
            }
        }

        #endregion

        #region Department Master Crud Functionality
        public async Task<IActionResult> FetchDepartmentList()
        {
            try
            {
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllDepartmentMasterUrl;
                var apiResponse = await _masterServiceHelper.GetCommonAsync<List<DepartmentDTO>>(apiUrl);
                var departmentList = apiResponse.Result;

                return PartialView("_DepartmentListPartial", departmentList);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error fetching department list: {ex.Message}");
                return PartialView("_DepartmentListPartial", new List<DepartmentDTO>());
            }
        }

        [HttpGet]
        [Route("PayrollMaster/[action]/{departmentId}")]
        public async Task<IActionResult> GetDepartmentDetailsById([FromRoute] int departmentId)
        {
            try
            {
                var getApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetDepartmentMasterDetailByIdUrl}/{departmentId}";
                var response = await _masterServiceHelper.GetByIdListCommonAsync<DepartmentDTO>(getApiUrl, departmentId);

                return PartialView("_AddUpdateDepartmentPartialView", response.Result);
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while loading the department." });
            }

        }
        [HttpPost]
        public async Task<IActionResult> SaveOrUpdateDepartment([FromBody] DepartmentMasterDTO departmentMasterDTO)
        {
            if (departmentMasterDTO == null ||
                string.IsNullOrWhiteSpace(departmentMasterDTO.DepartmentName) ||
                string.IsNullOrWhiteSpace(departmentMasterDTO.DepartmentCode))
            {
                return Json(new { success = false, message = "Invalid data. Please check your input." });
            }

            try
            {
                departmentMasterDTO.UpdatedBy = SessionUserId;
                departmentMasterDTO.CreatedBy = SessionUserId;
                departmentMasterDTO.UpdatedDate = DateTime.Now;
                //departmentMasterDTO.IsActive = true;

                string apiUrl;
                bool isNewEntry = departmentMasterDTO.Department_Id == 0;

                if (isNewEntry)
                {
                    apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostDepartmentMasterDetailUrl;
                    var createResponse = await _masterServiceHelper.PostCommonAsync<DepartmentMasterDTO, DepartmentMaster>(apiUrl, departmentMasterDTO);

                    if (createResponse.IsSuccess)
                    {
                        return Json(new { success = true, message = createResponse.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = createResponse.Message ?? "Failed to create department." });
                    }
                }
                else
                {
                    apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.PutDepartmentMasterDetailUrl}/{departmentMasterDTO.Department_Id}";
                    var updateResponse = await _masterServiceHelper.PutCommonAsync<DepartmentMasterDTO, DepartmentMaster>(apiUrl, departmentMasterDTO);

                    if (updateResponse.IsSuccess)
                    {
                        return Json(new { success = true, message = updateResponse.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = updateResponse.Message ?? "Failed to update department." });
                    }
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while processing the request." });
            }
        }
        public async Task<IActionResult> DeleteDepartment([FromBody] DepartmentMasterDTO model)
        {
            try
            {
                // Set UpdatedBy from session
                model.UpdatedBy = SessionUserId;

                // Construct API URL
                var deleteApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.DeleteDepartmentMasterDetailUrl}/{model.Department_Id}";

                // Call the common delete method (now sending the request body)
                var deleteResponse = await _masterServiceHelper.DeleteCommonAsync<DepartmentMasterDTO, DepartmentMaster>(deleteApiUrl, model);

                if (deleteResponse != null && deleteResponse.IsSuccess)
                {
                    return Json(new { success = true, message = deleteResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = deleteResponse.Message ?? "Failed to delete the department." });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while processing the request." });
            }
        }


        #endregion

        #region Map Department Location Master Crud Functionality
        public async Task<IActionResult> FetchMapDepartmentLocationList()
        {
            try
            {
                var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllMapDepartmentLocationUrl;
                var apiResponse = await _masterServiceHelper.GetCommonAsync<List<MapDepartmentLocationDTO>>(apiUrl);
                var departmentList = apiResponse.Result;

                return PartialView("_MapDepartmentLocationListPartial", departmentList);
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return PartialView("_DepartmentListPartial", new List<DepartmentDTO>());
            }
        }

        [HttpGet]
        [Route("PayrollMaster/[action]/{mapdepartmentId}")]
        public async Task<IActionResult> GetMapDepartmentLocationsById([FromRoute] int mapdepartmentId)
        {
            try
            {
                var getApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetMapDepartmentLocationByIdUrl}/{mapdepartmentId}";
                var response = await _masterServiceHelper.GetByIdListCommonAsync<MapDepartmentLocationDTO>(getApiUrl, mapdepartmentId);

                if (!response.IsSuccess || response.Result == null)
                {
                    return Json(new { success = false, message = response.Message });
                }

                // Return data in a structure suitable for populating dropdowns
                var mapDepartment = response.Result;

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        departmentLocationId = mapDepartment.Department_Location_Id,
                        CorrespondanceId = mapDepartment.Correspondance_ID,
                        companyId = mapDepartment.Company_Id,
                        countryId = mapDepartment.Country_ID,
                        stateId = mapDepartment.State_Id,
                        cityId = mapDepartment.City_ID,
                        locationId = mapDepartment.Location_Id,
                        areaId = mapDepartment.Area_Id,
                        FloorId = mapDepartment.Floor_Id,
                        departmentId = mapDepartment.Department_Id,
                        departmentCode = mapDepartment.Department_Code,
                        isActive = mapDepartment.IsActive,
                    }
                });
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while processing the request." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdateMapDepartment([FromBody] MapDepartmentLocationDTO mapDepartmentLocationDTO)
        {
            if (mapDepartmentLocationDTO == null)
            {
                return Json(new { success = false, message = "Invalid data. Please check your input." });
            }

            try
            {
                mapDepartmentLocationDTO.UpdatedBy = SessionUserId;
                mapDepartmentLocationDTO.CreatedBy = SessionUserId;
                mapDepartmentLocationDTO.UpdatedDate = DateTime.Now;
                mapDepartmentLocationDTO.Correspondance_ID = mapDepartmentLocationDTO.Location_Id;
                //departmentMasterDTO.IsActive = true;

                string apiUrl;
                bool isNewEntry = mapDepartmentLocationDTO.Department_Location_Id == 0;

                if (isNewEntry)
                {
                    apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostMapDepartmentLocationDetailUrl;
                    var createResponse = await _masterServiceHelper.PostCommonAsync<MapDepartmentLocationDTO, MapDepartmentLocation>(apiUrl, mapDepartmentLocationDTO);

                    if (createResponse.IsSuccess)
                    {
                        return Json(new { success = true, message = createResponse.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = createResponse.Message ?? "Failed to create department." });
                    }
                }
                else
                {
                    apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.PutMapDepartmentLocationDetailUrl}/{mapDepartmentLocationDTO.Department_Location_Id}";
                    var updateResponse = await _masterServiceHelper.PutCommonAsync<MapDepartmentLocationDTO, MapDepartmentLocation>(apiUrl, mapDepartmentLocationDTO);

                    if (updateResponse.IsSuccess)
                    {
                        return Json(new { success = true, message = updateResponse.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = updateResponse.Message ?? "Failed to update department." });
                    }
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while processing the request." });
            }
        }
        public async Task<IActionResult> DeleteMapDepartment([FromBody] MapDepartmentLocationDTO model)
        {
            try
            {
                // Set UpdatedBy from session
                model.UpdatedBy = SessionUserId;

                // Construct API URL
                var deleteApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.DeleteMapDepartmentMasterDetailUrl}/{model.Department_Location_Id}";

                // Call the common delete method (now sending the request body)
                var deleteResponse = await _masterServiceHelper.DeleteCommonAsync<MapDepartmentLocationDTO, MapDepartmentLocation>(deleteApiUrl, model);

                if (deleteResponse != null && deleteResponse.IsSuccess)
                {
                    return Json(new { success = true, message = deleteResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = deleteResponse.Message ?? "Failed to delete the department." });
                }
            }
            catch (SessionExpiredException ex)
            {
                return StatusCode(401, new { success = false, message = "Session Expired." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while processing the request." });
            }

        }

        #endregion

    }
}
