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

namespace Payroll.WebApp.Controllers
{
    public class DataVisualisationController : SharedUtilityController
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
        private readonly RestApiTransactionServiceHelper _transactionServiceHelper;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        // Property to get UserId from Session
        private int SessionCompanyId
        {
            get
            {
                //var sessionCompanyData = SessionHelper.GetSessionObject<UserCompanyDetails>(HttpContext, "UserSessionData");
                var sessionCompanyData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");

                // Extract companyId, roleId, and userId
                return sessionCompanyData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;

                //return sessionCompanyData?.Company_Id ?? 0;
            }
        }

        public DataVisualisationController(RestApiUserServiceHelper userServiceHelper, RestApiMasterServiceHelper masterServiceHelper, IMapper mapper, IOptions<ApiSettings> apiSettings, RestApiTransactionServiceHelper transactionServiceHelper)
        {
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _masterServiceHelper = masterServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
            _userServiceHelper = userServiceHelper;
        }

        public async Task<IActionResult> ContractorList()
        {
            var response = new ApiResponseModel<ContractorDetailsDTO> { IsSuccess = false };
            try
            {
                var Company_Id = SessionCompanyId;
                int correspondance_ID = 0;
                int contractor_ID = 0;
                var apikey = await _userServiceHelper.GenerateApiKeyAsync();
                string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllContractorUrl}/{Company_Id}";
                var apiResponse = await _transactionServiceHelper.GetByIdCommonAsync<IEnumerable<ContractorDetails>>(apiUrl, apikey);
                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    IEnumerable<ContractorDetailsDTO> ContractorDetailsDTO = _mapper.Map<IEnumerable<ContractorDetailsDTO>>(apiResponse.Result);
                    return View(ContractorDetailsDTO);
                }
                else
                {
                    return View(new List<ContractorDetailsDTO>());
                }

            }
            catch (Exception ex)
            {
                return new JsonResult(new ApiResponseModel<ContractorDetailsDTO>
                {
                    IsSuccess = false,
                    Message = MessageConstants.TechnicalIssue,
                    StatusCode = ApiResponseStatusConstant.InternalServerError
                });
            }
        }

        public async Task<IActionResult> ContractorProfile(string contractorId)
        {
            string decryptedIdStr = SingleEncryptionHelper.Decrypt(contractorId);
            int id = 0;
            if (int.TryParse(decryptedIdStr, out int parsedcId))
            {
                id = parsedcId;
            }
            int companyId = SessionCompanyId;
           
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetProfileContractorUrl}?contractorId={id}&id={companyId}";

            var response = await _transactionServiceHelper.GetByIdCommonAsync<List<ContractorProfileDTO>>(
                apiUrl, apiKey
            );

            var ContractorList = response?.Result;

            if (ContractorList == null || !ContractorList.Any())
            {
                TempData["ErrorMessage"] = "No Contractor found.";
                return RedirectToAction("Error", "Home");
            }

            // Send only the first entity to the view
            var entity = ContractorList.First();
            ViewBag.contractorId = id;
            return View(entity);
        }



        public IActionResult WorkOrderList()
        {
            return View();
        }
        public IActionResult WorkOrderProfile()
        {
            return View();
        }

    }
}
