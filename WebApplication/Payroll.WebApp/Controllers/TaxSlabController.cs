using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Extensions;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollTransactionService.BAL.Models;
using static Payroll.WebApp.Helpers.SessionHelper;

namespace Payroll.WebApp.Controllers
{
    public class TaxSlabController : Controller
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
        public TaxSlabController(RestApiUserServiceHelper userServiceHelper, RestApiMasterServiceHelper masterServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings, RestApiTransactionServiceHelper transactionServiceHelper)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _masterServiceHelper = masterServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _userServiceHelper = userServiceHelper;
        }
        
        #region Income Tax Slab Functionality

        public async Task<IActionResult> TaxSlab()
        {
            //await SetUserPermissions();
            var response = new ApiResponseModel<TaxSlabMasterDTO> { IsSuccess = false };
            try
            {
                ViewBag.SessionCompanyId = SessionCompanyId;
                ViewBag.SessionRoleId = SessionRoleId;
                return View(new List<TaxSlabMasterDTO>());
            }
            catch (Exception ex)
            {
                return new JsonResult(new ApiResponseModel<TaxSlabMasterDTO>
                {
                    IsSuccess = false,
                    Message = MessageConstants.TechnicalIssue,
                    StatusCode = ApiResponseStatusConstant.InternalServerError
                });
            }
        }

        public async Task<IActionResult> TaxSlabList(int id)
        {
            //await SetUserPermissions();
            var response = new ApiResponseModel<TaxSlabMasterDTO> { IsSuccess = false };
            try
            {
                var Company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                string apiUrl = $"{ _apiSettings.PayrollTransactionEndpoints.GetAllTaxSlabUrl}/{id}";
                var apiResponse = await _transactionServiceHelper.GetByIdCommonAsync<IEnumerable<TaxSlabMasterDTO>>(apiUrl, apikey);
                if (apiResponse?.Result == null)
                {
                    return Json(new
                    {
                        html = await this.RenderViewAsync("_TaxSlabListPartialView", new List<TaxSlabMasterDTO>(), true),
                        count = 0
                    });
                }

                var taxSlabList = apiResponse.Result.Select(p => new TaxSlabMasterDTO
                {
                    YearlyItTableDetail_Id = p.YearlyItTableDetail_Id,
                    YearlyItTable_Id = p.YearlyItTable_Id,
                    Company_Id = p.Company_Id,
                    SlabName = p.SlabName,
                    Income_From = p.Income_From,
                    Income_To = p.Income_To,
                    TaxPaybleInPercentage = p.TaxPaybleInPercentage,
                    TaxPaybleInAmount = p.TaxPaybleInAmount,
                    IsActive = p.IsActive
                }).ToList();
                ViewBag.TaxSlabConfigCount = taxSlabList.Count;
                var html = await this.RenderViewAsync("_TaxSlabListPartialView", taxSlabList, true);

                return Json(new
                {
                    html = html,
                    count = taxSlabList.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    html = await this.RenderViewAsync("_TaxSlabListPartialView", new List<TaxSlabMasterDTO>(), true),
                    count = 0
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTaxSlabDetailsById([FromRoute] int Id)
        {
            try
            {
                int company_Id = SessionCompanyId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                var getApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetByIdTaxSlabUrl}/{Id}";
                var response = await _transactionServiceHelper.GetByIdCommonAsync<TaxSlabMaster>(getApiUrl, apikey);

                if (!response.IsSuccess || response.Result == null)
                {
                    return Json(new { success = false, message = response.Message });
                }
                return Json(new { success = true, data = response.Result });
            }
            catch (Exception ex)
            {
                // Log error
                return Json(new { success = false, message = "An error occurred while fetching area details." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTaxSlab([FromBody] TaxSlabMasterDTO taxSlabMasterDTO)
        {
            //await SetUserPermissions();
            if (taxSlabMasterDTO == null)
            {
                return Json(new { success = false, message = "Invalid input. Please provide all required fields." });
            }
            try
            {
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                taxSlabMasterDTO.Company_Id = SessionCompanyId;
                taxSlabMasterDTO.CreatedBy = SessionUserId; // Assuming this is hardcoded or passed from session/user context
                taxSlabMasterDTO.CreatedDate = DateTime.Now;
                taxSlabMasterDTO.IsActive = true;

                var taxSlabRequest = _mapper.Map<TaxSlabMasterDTO>(taxSlabMasterDTO);
                // Define API URL
                var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostTaxSlabUrl;
                // Call the generic PostAsync method to post company data
                var apiResponse = await _transactionServiceHelper
                                    .PostCommonAsync<TaxSlabMasterDTO, TaxSlabMaster>(apiUrl, taxSlabRequest, apikey);
                // Handle response
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message ?? "Record created successfully." });
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
        public async Task<IActionResult> UpdateTaxSlab([FromBody] TaxSlabMasterDTO taxSlabMasterDTO)
        {
            //await SetUserPermissions();
            if (taxSlabMasterDTO == null)
            {
                return Json(new { success = false, message = "Invalid data. Please check your input." });
            }
            try
            {
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                taxSlabMasterDTO.UpdatedBy = SessionUserId;
                taxSlabMasterDTO.UpdatedDate = DateTime.Now;
                // Assuming you are updating the area using a service or repository
                var updateApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.PutTaxSlabUrl}/{taxSlabMasterDTO.YearlyItTableDetail_Id}";
                var updateResponse = await _transactionServiceHelper.PutCommonAsync<TaxSlabMasterDTO, TaxSlabMaster>(updateApiUrl, taxSlabMasterDTO, apikey);

                if (updateResponse.IsSuccess)
                {
                    return Json(new { success = true, message = updateResponse.Message });
                }
                else
                {
                    return Json(new { success = false, message = updateResponse.Message ?? "Failed to update the tax slab." });
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception details
                Console.WriteLine($"Exception in : {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the slab." });
            }
        }

        public async Task<IActionResult> DeleteTaxSlab([FromBody] TaxSlabMasterDTO model)
        {
            //await SetUserPermissions();
            var apikey = await _userServiceHelper.GenerateApiKeyAsync();
            // Set UpdatedBy from session
            model.UpdatedBy = SessionUserId;
            // Construct API URL
            var deleteApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.DeleteTaxSlabUrl}/{model.YearlyItTableDetail_Id}";
            // Call the common delete method (sending the request body)
            var deleteResponse = await _transactionServiceHelper.DeleteCommonAsync<TaxSlabMasterDTO, TaxSlabMaster>(deleteApiUrl, model, apikey);

            // Return appropriate response
            if (deleteResponse != null && deleteResponse.IsSuccess)
            {
                return Json(new { success = true, message = deleteResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = deleteResponse.Message ?? "Failed to delete the pay component." });
            }
        }
        #endregion

        public async Task<IActionResult> AssignIncomeTaxSlab()
        {
            //await SetUserPermissions();
            ViewBag.SessionCompanyId = SessionCompanyId; // Assuming it's string type like "2023-24"
            ViewBag.SessionFinancialYear = SessionFinancialYear.ToString(); // Assuming it's string type like "2023-24"

            return View();
            //var response = new ApiResponseModel<EntityFilterRequestDTO> { IsSuccess = false };
            //try
            //{
            //    return View(new List<EntityFilterRequestDTO>());
            //}
            //catch (Exception ex)
            //{
            //    return new JsonResult(new ApiResponseModel<EntityFilterRequestDTO>
            //    {
            //        IsSuccess = false,
            //        Message = MessageConstants.TechnicalIssue,
            //        StatusCode = ApiResponseStatusConstant.InternalServerError
            //    });
            //}
        }

        [HttpGet]
        public IActionResult EntityFilterResponseList()
        {
            ViewBag.SessionCompanyId = SessionCompanyId; // Assuming it's string type like "2023-24"
            ViewBag.SessionFinancialYear = SessionFinancialYear.ToString(); // Assuming it's string type like "2023-24"

            return PartialView("_EntityFilterPartial", new EntityFilterResponseDTO());
        }

        [HttpPost]
        public async Task<IActionResult> EntityFilterResponseList([FromBody] EntityFilterRequest entityFilterRequest)
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
        public async Task<IActionResult> BulkAssignTaxRegime([FromBody] MapEntityTaxRegimeDTO mapEntityTaxRegimeDTO)
        {
            if (mapEntityTaxRegimeDTO == null || mapEntityTaxRegimeDTO.EntityTaxRegime == null || !mapEntityTaxRegimeDTO.EntityTaxRegime.Any())
            {
                return Json(new { success = false, message = "Invalid input. Please provide valid tax regime data." });
            }
            try
            {
                mapEntityTaxRegimeDTO.CreatedBy = SessionUserId;
                mapEntityTaxRegimeDTO.UpdatedBy = SessionUserId;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                var assigntaxRequest = _mapper.Map<MapEntityTaxRegime>(mapEntityTaxRegimeDTO);
                // Define API URL
                var apiUrl = _apiSettings.PayrollTransactionEndpoints.PostAssignTaxRegimeRequestUrl;
                // Call the generic PostAsync method to post company data
                var apiResponse = await _transactionServiceHelper
                                    .PostCommonAsync<MapEntityTaxRegime, MapEntityTaxRegimeDTO>(apiUrl, assigntaxRequest, apikey);
                // Handle API result
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = apiResponse.Message ?? "Bulk tax regime assignment successful." });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message ?? "Failed to assign tax regimes." });
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

    }
}
