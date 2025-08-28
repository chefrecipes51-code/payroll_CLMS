using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.Helpers;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollTransactionService.BAL.Models;
using static Payroll.WebApp.Helpers.SessionHelper;

namespace Payroll.WebApp.Controllers
{
    public class EntityMasterController : SharedUtilityController
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
        private readonly RestApiTransactionServiceHelper _transactionServiceHelper;
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
        private int SessionCompanyId
        {
            get
            {
                //var sessionCompanyData = SessionHelper.GetSessionObject<UserCompanyDetails>(HttpContext, "UserSessionData");
                var sessionCompanyData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
                // Extract companyId, roleId, and userId
                return sessionCompanyData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
            }
        }
        private string SessionFinancialYear
        {
            get
            {
                var sessionFinancial = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
                return sessionFinancial?.CompanyDetails?.FirstOrDefault()?.FinYear ?? string.Empty;
            }
        }
        private int SessionRoleId
        {
            get
            {
                var sessionCompanyData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
                return sessionCompanyData.RoleDetails.FirstOrDefault()?.Role_Id ?? 0;
            }
        }
        private async Task SetUserPermissions()
        {
            var menuItems = await MenuHelper.GetUserMenus(HttpContext, _userServiceHelper, _mapper, _apiSettings);
            string controllerName = RouteData.Values["controller"]?.ToString().ToLower();
            var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));
            ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }
        public EntityMasterController(RestApiUserServiceHelper userServiceHelper, RestApiMasterServiceHelper masterServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings, RestApiTransactionServiceHelper transactionServiceHelper)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _masterServiceHelper = masterServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _userServiceHelper = userServiceHelper;
        }

        public IActionResult EntityList()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            ViewBag.SessionRoleId = SessionRoleId;
            return View();
        }
        public async Task<IActionResult> EntityProfile(string id)
        {
            int? decrypteduserId = null;
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    string decryptedIdStr = SingleEncryptionHelper.Decrypt(id);

                    if (int.TryParse(decryptedIdStr, out int parseduserId))
                    {
                        decrypteduserId = parseduserId;
                    }
                    else
                    {
                        //return BadRequest("Invalid decrypted company ID format");
                    }
                }
                catch (Exception ex)
                {
                    // return BadRequest("Invalid encrypted company ID");
                }
            }
            int companyId = SessionCompanyId;
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllEntityDetailUrl}?entityId={decrypteduserId}";

            var response = await _transactionServiceHelper.GetByIdCommonAsync<List<EntityMasterDTO>>(
                apiUrl, apiKey
            );

            var entityList = response?.Result;

            if (entityList == null || !entityList.Any())
            {
                TempData["ErrorMessage"] = "No entity found.";
                return RedirectToAction("Error", "Home");
            }

            // Send only the first entity to the view
            var entity = entityList.First();
            ViewBag.EntityId = decrypteduserId;
            return View(entity);
        }
        public IActionResult GradeEntityMapping()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            ViewBag.SessionRoleId = SessionRoleId;
            return View();
        }
        [HttpGet]
        public IActionResult MapEntityGradeResponse()
        {
            ViewBag.SessionCompanyId = SessionCompanyId;
            ViewBag.SessionRoleId = SessionRoleId;
            return PartialView("_MapEntityGradePartial");
        }
        [HttpPost]
        public async Task<IActionResult> MapEntityGradeResponse([FromBody] EntityFilterRequest entityFilterRequest)
        {
            try
            {
                if (entityFilterRequest == null)
                {
                    // GET call to load partial for first time
                    return PartialView("_EntityFilterPartial", new EntityFilterResponseDTO());
                }
                entityFilterRequest.CompanyId = SessionCompanyId;
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
                string apiUrl = _apiSettings.PayrollTransactionEndpoints.GetAllEntityFilterRequestUrl;

                var apiResponse = await _transactionServiceHelper.PostCommonAsync<EntityFilterRequest, EntityFilterResponseDTO>(apiUrl, entityFilterRequest, apiKey);
                // Check for null response or null result
                if (apiResponse == null && apiResponse.Result.ContractorEntities == null && apiResponse.Result.EntityCodes == null && apiResponse.Result.EntityNames == null && apiResponse.Result.ContractorEntities == null && apiResponse.Result.GradeMapEntities == null)
                {
                    return Json(new { message = "No records found" });
                }
                var model = apiResponse?.Result ?? new EntityFilterResponseDTO();
                
                switch (entityFilterRequest.SelectType)
                {
                    case "C":
                        return Json(new { contractors = model.Contractors });
                    case "ED":
                        return Json(new { entityCodes = model.EntityCodes });
                    case "EM":
                        return Json(new { entityNames = model.EntityNames });
                    case "CO":
                        return Json(new { contractorEntities = model.ContractorEntities });
                    case "ET":
                        var allEntities = model.GradeMapEntities;

                        int pageSize = entityFilterRequest.PageSize > 0 ? entityFilterRequest.PageSize : 10;
                        int pageNumber = entityFilterRequest.PageNumber > 0 ? entityFilterRequest.PageNumber : 1;

                        int totalCount = model.TotalCount;
                        var paginatedData = allEntities
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
                        return Json(new
                        {
                            gradeMapEntities = model.GradeMapEntities,
                            totalRecords = totalCount
                        });

                    //return Json(new { gradeMapEntities = model.GradeMapEntities });
                    default:
                        return Json(new { message = "Invalid SelectType" });
                }
            }
            catch (Exception ex)
            {
                //return Json(new EntityFilterResponseDTO()); // or appropriate error response
                return PartialView("_EntityFilterPartial", new EntityFilterResponseDTO());
            }
        }
        [HttpPost]
        public async Task<IActionResult> BulkAssignMapGrdaeEntity([FromBody] MapEntityGradeMasterDTO mapGradeEntityDTO)
        {
            if (mapGradeEntityDTO == null || mapGradeEntityDTO.MapEntityGrade == null || !mapGradeEntityDTO.MapEntityGrade.Any())
            {
                return Json(new { success = false, message = "Invalid input. Please provide valid grade entity data." });
            }
            try
            {
                mapGradeEntityDTO.UpdatedBy = SessionUserId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                var assigntaxRequest = _mapper.Map<MapEntityGradeMaster>(mapGradeEntityDTO);
                // Define API URL
                var apiUrl = _apiSettings.PayrollTransactionEndpoints.PutMapGradeEntityUrl;
                // Call the generic PostAsync method to post company data
                var apiResponse = await _transactionServiceHelper
                                    .PostCommonAsync<MapEntityGradeMaster, MapEntityGradeMasterDTO>(apiUrl, assigntaxRequest, apikey);
                // Handle API result
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message ?? "Bulk map grade entity successful." });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message ?? "Failed to map grade entity." });
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
        public async Task<IActionResult> UpdateEntityCompliance([FromBody] EntityComplianceDTO entityComplianceDTO)
        {
            await SetUserPermissions();
            if (entityComplianceDTO == null)
            {
                return Json(new { success = false, message = "Invalid data. Please check your input." });
            }
            try
            {
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                entityComplianceDTO.UpdatedBy = SessionUserId;
                entityComplianceDTO.UpdatedDate = DateTime.Now;
                // Assuming you are updating the area using a service or repository
                var updateApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.UpdateEntityComplianceDetailUrl}/{entityComplianceDTO.Entity_ID}";
                var updateResponse = await _transactionServiceHelper.PutCommonAsync<EntityComplianceDTO, EntityCompliance>(updateApiUrl, entityComplianceDTO, apikey);

                if (updateResponse.IsSuccess)
                {
                    return Json(new { success = true, message = updateResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = updateResponse.Message ?? "Failed to update the area." });
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception details
                Console.WriteLine($"Exception in UpdateArea: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the area." });
            }
        }
        public async Task<IActionResult> EntityDataValidation()
        {
            int module_Id = 51;
            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllEntityDataValidationDetailUrl}?moduleId={module_Id}";

            var response = await _transactionServiceHelper.GetCommonAsync<List<EntityDataValidationDTO>>(
                apiUrl
            );

            var entityList = response?.Result;

            if (entityList == null || !entityList.Any())
            {
                return Json(new { success = false, message = "No data found." });

            }
            return Json(new { success = true, data = response.Result });

        }
    }
}
