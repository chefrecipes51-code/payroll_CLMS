using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using Payroll.Common.ApplicationModel;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollTransactionService.BAL.Models;
using static Payroll.Common.EnumUtility.EnumUtility;

namespace Payroll.WebApp.Controllers
{
    public class DropDownController : Controller
    {
        //private readonly BindDropdownDataHelper _dropdownHelper;
        //private readonly CommonHelper _commonHelper;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
        private readonly RestApiTransactionServiceHelper _transactionServiceHelper;
        //public DropDownController(RestApiUserServiceHelper userServiceHelper, BindDropdownDataHelper dropdownHelper, CommonHelper commonHelper)
        //public DropDownController(RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, BindDropdownDataHelper dropdownHelper)
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
                var sessionCompanyData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
                return sessionCompanyData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
            }
        }
        public DropDownController(RestApiMasterServiceHelper masterServiceHelper, RestApiUserServiceHelper userServiceHelper, RestApiTransactionServiceHelper transactionServiceHelper)
        {
            //_dropdownHelper = dropdownHelper;
            //_commonHelper = commonHelper;
            _userServiceHelper = userServiceHelper;
            _masterServiceHelper = masterServiceHelper;
            _transactionServiceHelper = transactionServiceHelper;
        }
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all Country details from the database 
        ///                    using a stored procedure. It checks if records exist and
        ///                    returns the appropriate response.
        ///  Created Date   :- 25-Dec-2024
        ///  Change Date    :- 25-Dec-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a JSOn response with the result of the operation.</returns>

        [HttpGet]
        public async Task<IActionResult> FetchCountriesDropdown()
        {
            var viewModel = await _userServiceHelper.BindCountriesDataAsync();
            return Json(viewModel.CountriesDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchCommonCountriesDropdown() /// Created By:- Harshida Parmar
        {
            var viewModel = await _userServiceHelper.BindCommonCountriesDataAsync();
            return Json(viewModel.CountriesDropdown);
        }
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all User Type details from the database 
        ///                    using a stored procedure. It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 25-Dec-2024
        ///  Change Date    :- 25-Dec-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>

        [HttpGet]
        public async Task<IActionResult> FetchUserTypeDropdown()
        {
            var viewModel = await _userServiceHelper.BindUserTypeDataAsync();
            return Json(viewModel.UserTypeDropdown);
        }

        public async Task<IActionResult> GetUsersDropdown()
        {
            var viewModel = await _userServiceHelper.GetUserListAsync();
            return Json(viewModel.UserDropdown);
        }

        public async Task<IActionResult> GetLocationwiseUsersDropdown(int? Company_Id, int? Location_Id)
        {
            var viewModel = await _userServiceHelper.GetLocationwiseUserListAsync(Company_Id, Location_Id);
            return Json(viewModel.UserDropdown);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all Company details from the database 
        ///                    using a stored procedure. It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 25-Dec-2024
        ///  Change Date    :- 25-Dec-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>

        [HttpGet]
        public async Task<IActionResult> FetchCompaniesDropdown()
        {
            var viewModel = await _masterServiceHelper.BindCompaniesDataAsync();
            return Json(viewModel.CompanyDropdown);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all Company->Country->State->City->Location->Role from the database based on companyId
        ///                    using a stored procedure. It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 25-Dec-2024
        ///  Change Date    :- 25-Dec-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> GetCompanyLocationData(int? companyId, int? userId)
        {
            try
            {
                var data = await _masterServiceHelper.BindCompanyLocationDataAsync(companyId, userId);

                if (data == null || !data.IsSuccess || data.Result == null)
                {
                    return Json(new { isSuccess = false, message = data?.Message ?? "No data found" });
                }

                var companyLocationData = new CompanyLocationMapDto
                {
                    Countries = data.Result.Countries,
                    States = data.Result.States,
                    Cities = data.Result.Cities,
                    Locations = data.Result.Locations,
                    Roles = data.Result.Roles,
                    AreaLocations = data.Result.AreaLocations,
                    Areas = data.Result.Areas
                };

                return Json(new { isSuccess = true, result = companyLocationData });
            }
            catch (Exception ex)
            {
                return Ok($"Error: {ex.Message}");
            }
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all Salutaions from the based on
        ///                    using a enum. It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 25-Dec-2024
        ///  Change Date    :- 25-Dec-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> FetchSalutationsDropdown()
        {
            var dropdownItems = await _userServiceHelper.BindEnumToDropdownAsync<SalutationEnum>();
            return Json(dropdownItems);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all City details from the database 
        ///                    using a stored procedure. It checks if records exist and
        ///                    returns the appropriate response.
        ///  Created Date   :- 20-01-2025
        ///  Change Date    :- 20-01-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a JSOn response with the result of the operation.</returns>

        [HttpGet]
        [Route("DropDown/[action]/{state_ID}")]
        public async Task<IActionResult> FetchCityDropdown(int? state_ID)
        {
            var viewModel = await _masterServiceHelper.BindCityDataAsync(state_ID);
            return Json(viewModel.CitysDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchCommonCityDropdown(int? state_ID) /// Created By:- Harshida Parmar
        {
            var viewModel = await _masterServiceHelper.BindCityDataAsync(state_ID);
            return Json(viewModel.CitysDropdown);
        }
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all State details from the database 
        ///                    using a stored procedure. It checks if records exist and
        ///                    returns the appropriate response.
        ///  Created Date   :- 20-01-2025
        ///  Change Date    :- 20-01-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a JSOn response with the result of the operation.</returns>

        [HttpGet]
        [Route("DropDown/[action]/{Country_Id}")]
        public async Task<IActionResult> FetchStateDropdown(int? Country_Id)
        {
            var viewModel = await _masterServiceHelper.BindStateDataAsync(Country_Id);
            return Json(viewModel.StatesDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchCommonStateDropdown(int? Country_Id) /// Created By:- Harshida Parmar
        {
            var viewModel = await _masterServiceHelper.BindStateDataAsync(Country_Id);
            return Json(viewModel.StatesDropdown);
        }
        [HttpGet]
        [Route("DropDown/[action]/{City_ID}")]
        public async Task<IActionResult> FetchLocationsDropdown(int? City_ID)
        {
            var viewModel = await _masterServiceHelper.BindCityWiseLocationsDataAsync(City_ID);
            return Json(viewModel.LocationDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchCommonLocationsDropdown(int? City_ID) // /// Created By:- Harshida Parmar
        {
            var viewModel = await _masterServiceHelper.BindCityWiseLocationsDataAsync(City_ID);
            return Json(viewModel.LocationDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchAreaLocationDropdown(int id)
        {
            var viewModel = await _masterServiceHelper.BindAreaLocationDataAsync(id);
            return Json(viewModel.AreasDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchDepartmentDropdown(int id)
        {
            var viewModel = await _masterServiceHelper.BindDepartmentDataAsync(id);
            return Json(viewModel.DepartmentDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchAllDepartmentDropdown()
        {
            var viewModel = await _masterServiceHelper.BindAllDepartmentDataAsync();
            return Json(viewModel.DepartmentDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchFloorMasterDropdown()
        {
            var viewModel = await _masterServiceHelper.BindFloorDataAsync();
            return Json(viewModel?.FloorsDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchSalaryFrequencyDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<SalaryFrequencyEnum>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchMonthlySalaryDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<MonthlySalary>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchEffectivePayrollStartMonthDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<EffectivePayrollStartMonth>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchAllowAdhocComponentsDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchLockSalaryEditsPostPayrollDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchEnablePayDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchSlipGenerationDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchPayslipFormatDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<PayslipFormat>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchAutoNumberingDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchDigitalSignatureDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchAutoEmailPayslipsDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchIsPayslipNoDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchDigitalSignatureRequiredDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchDataSyncTypeDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<DataSyncType>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchSyncFrequencyDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<SyncFrequency>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchPaymentFormatDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<PaymentFormat>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchCommongYesNoDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchEntityTypeDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEntityTypeAsync();
            return Json(dropdownItems.EntityTypeDropdown);
        }
        /// <summary>
        /// Created By:- Harshida Parmar
        /// Created Date:- 07-02-25
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FetchCompanyTypeDropdown()
        {
            var viewModel = await _masterServiceHelper.BindCompanyTypeAsync();
            return Json(viewModel.CompanyTypeDropdown);
        }

        /// <summary>
        /// Created By:- Harshida Parmar
        /// Created Date:- 13-02-25
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FetchCurrencyDropdown()
        {
            var viewModel = await _masterServiceHelper.BindCurrencyAsync();
            return Json(viewModel.CompanyCurrencyDropdown);
        }

        /// <summary>
        /// Created By:- Harshida Parmar
        /// Created Date:- 25-06-25
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FetchAccountHeadDropdown()
        {
            var viewModel = await _masterServiceHelper.BindAccountHeadAsync();
            return Json(viewModel.AccountHeadDropdown);
        }

        /// <summary>
        /// Created By:- Harshida Parmar
        /// Created Date:- 26-06-25
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FetchGLDropdown(int parentId = 0)
        {
            var viewModel = await _masterServiceHelper.BindGLAsync(parentId);
            return Json(viewModel.GLDropdown);
        }
        /// <summary>
        ///  Developer Name :-Harshida
        ///  Message detail :- This Method retrieves all PTaxSlab based on using a enum. 
        ///                     It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 08-05-2025      
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> FetchPayTaxFrequencyDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<PTax>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchTranTypeDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<TranType>();
            return Json(dropdownItems);
        }
        /// <summary>
        ///  Developer Name :-Harshida
        ///  Message detail :- This Method retrieves all Gender based on using a enum. 
        ///                     It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 08-05-2025      
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> FetchGenderDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<Gender>();
            return Json(dropdownItems);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all Correspondance Type Enum from the based on
        ///                    using a enum. It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 02-Jan-2025
        ///  Change Date    :- 02-Jan-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> FetchCorrespondanceTypeDropdown()
        {
            var dropdownItems = await _userServiceHelper.BindEnumToDropdownAsync<CorrespondanceTypeEnum>();
            return Json(dropdownItems);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all Calculation Type from the based on
        ///                    using a enum. It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 15-04-2025
        ///  Change Date    :- 15-04-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> FetchCalculationTypeDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CalculationType>();
            return Json(dropdownItems);
        }

        #region Global Parameter Complinace Setting 
        [HttpGet]
        public async Task<IActionResult> FetchPFApplicableDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchPFEmployerShareDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<PFEmployerShare>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchPFBasedOnDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<PFBasedOn>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchVoluntaryPFDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchVPFModeDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<VPFMode>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchESICApplicabilityDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchProfessionalTaxDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<ProfessionalTax>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchPTRegistrationModeDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<PTRegistrationMode>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchLabourWelfareFundDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<LabourWelfareFund>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchLWFCycleDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<LWFCycle>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchTDSDeductedOnActualDateDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchESICBasedOnDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<ESICBasedOn>();
            return Json(dropdownItems);
        }
        #endregion


        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all Calculation Type from the based on
        ///                    using a enum. It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 15-04-2025
        ///  Change Date    :- 15-04-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> FetchIsChildPayComponentDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<CommonToggleYesNo>();
            return Json(dropdownItems);
        }

        [HttpGet]
        public async Task<IActionResult> FetchIsParentPaycomponentDropdown()
        {
            int companyId = SessionCompanyId;
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindIsParentComponentAsync(apiKey, companyId);
            return Json(dropdownItems.ParentComponentDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchPaycomponentChildDropdown([FromQuery] string selectType, int EarningDeduction_Id = 0)
        {
            int companyId = SessionCompanyId;
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindIsParentComponentChildAsync(apiKey, companyId, selectType, EarningDeduction_Id);
            return Json(dropdownItems.ParentComponentDropdown);
        }

        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- This API retrieves all Formula Type from the based on
        ///                    using a enum. It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 30-04-2025     
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> FetchFormulaTypeDropdown()
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _masterServiceHelper.BindFormulaTypeAsync(apiKey);
            return Json(dropdownItems.FormulaDropdown);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all PayrollHeads Type from the based on
        ///                    using a enum. It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 15-04-2025
        ///  Change Date    :- 15-04-2025
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> FetchPayrollHeadsDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<PayrollHeads>();
            return Json(dropdownItems);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all Active Pay Grade Type from the based on
        ///                    using a api. It checks if records exist and returns the appropriate response.
        ///  Created Date   :- 02-05-2025     
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a model response with the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> FetchActivePayGradeTypeDropdown()
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindActivePayGradeTypeAsync(apiKey);
            return Json(dropdownItems.PayGradeDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchDistinctLocationDropdown(int companyId)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindDistinctLocationAsync(apiKey, companyId);
            return Json(dropdownItems.DistinctLocationDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchCompanyMonthYearDropdown(int companyId)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindDistinctLocationAsync(apiKey, companyId);
            return Json(dropdownItems.DistinctLocationDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchSkillCategoryDropdown(int correspondance_ID,
         int? skillcategory_Id,
         bool isActive)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindSkillCategoryAsync(apiKey, correspondance_ID, skillcategory_Id, isActive);
            return Json(dropdownItems.SkillCategoryDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchTradeTypeDropdown(int companyLocationID,
         int? trade_mst_Id,
         bool isActive)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindTradeAsync(apiKey, companyLocationID,
         trade_mst_Id,
         isActive);
            return Json(dropdownItems.TradeDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchTaxRegimeDropdown()
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindTaxRegimeAsync(apiKey);
            return Json(dropdownItems.TaxRegimeDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchSalaryStructureDropdown(int salaryId)
        {
            var dropdownItems = await _masterServiceHelper.BindSalaryStructureAsync(salaryId);
            return Json(dropdownItems.SalaryStructureDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchContractorDropdown(int? contractor_ID,
             int company_ID
             )
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindContractorAsync(apiKey, contractor_ID,
         company_ID
         );
            return Json(dropdownItems.ContractorDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchContractorWorkOrderDropdown(int company_ID)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindContractorWorkOrderAsync(apiKey, company_ID);
            return Json(dropdownItems.ContractorWorkOrderDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchContractorDropdownWithCode(int? contractor_ID,
            int company_ID
            )
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindContractorwithCodeAsync(apiKey, contractor_ID,
         company_ID
         );
            return Json(dropdownItems.ContractorDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchFinancialYearDropdown(int companyId)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindFinancialYearAsync(apiKey, companyId);
            return Json(dropdownItems.FinYearDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchCompanyPayrollValidationDropdown(string month_Yr)
        {
            var dropdownItems = await _transactionServiceHelper.BindCompanyPayrollValidationAsync(month_Yr);
            return Json(dropdownItems.CompanyPayrollValidationDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchCompanyLocationPayrollValidationDropdown(string month_Yr, int company_ID, bool? isActive)
        {
            var dropdownItems = await _transactionServiceHelper.BindCompanyLocationPayrollValidationAsync(month_Yr, company_ID, isActive);
            return Json(dropdownItems.CompanyLocationPayrollValidationDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchContractorPayrollValidationDropdown(int company_ID, string month_Yr, bool? isActive)
        {
            var dropdownItems = await _transactionServiceHelper.BindContractorPayrollValidationAsync(company_ID,month_Yr, isActive);
            return Json(dropdownItems.ContractorPayrollValidationDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchWorkOrderPayrollValidationDropdown(string month_Yr, int company_ID, bool? isActive)
        {
            var dropdownItems = await _transactionServiceHelper.BindWorkOrderPayrollValidationAsync(month_Yr, company_ID,  isActive);
            return Json(dropdownItems.WorkOrderPayrollValidationDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> FetchPreviousMonthYearPeriodByCmpIdDropdown(int company_ID)
        {
            var dropdownItems = await _transactionServiceHelper.BindPreviousMonthYearPeriodAsync(company_ID);
            return Json(dropdownItems.PreviousMonthYearPeriodPayrollValidationDropdown);
        }



        #region Bind Work Order Dropdown added by Abhishek
        [HttpGet]
        public async Task<IActionResult> FetchWorkOrderByContractor(string contractorCode)
        {
            var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindWorkOrderDataAsync(apiKey, contractorCode);
            return Json(dropdownItems.WorkOrderDropdown);
        }
        #endregion
        #region Krunali Added code 
        //addded by Krunali 03-03-2025
        [HttpGet]
        public async Task<IActionResult> FetchSubsidiaryTypesDropdown()
        {
            var dropdownItems = await _masterServiceHelper.BindEnumToDropdownSubsidiaryTypeAsync<SubsidiaryTypeEnum>();
            return Json(dropdownItems);
        }
        //addded by Krunali 03-03-2025
        [HttpGet]
        [Route("DropDown/[action]/{Location_Id}")]
        public async Task<IActionResult> FetchAreaDropdown(int Location_Id)
        {
            var viewModel = await _masterServiceHelper.BindAreaLocationDataAsync(Location_Id);
            return Json(viewModel.AreasDropdown);
        }
        #endregion

        #region Module
        [HttpGet]
        public async Task<IActionResult> FetchModulesDropdown()
        {
            var viewModel = await _userServiceHelper.BindModulesDataAsync();
            return Json(viewModel.ModulesDropdown);
        }
        [HttpGet]
        [Route("DropDown/[action]/{moduleId}")]
        public async Task<IActionResult> FetchServicesDropdown(int moduleId)
        {
            var viewModel = await _userServiceHelper.BindServicesDataAsync(moduleId);
            return Json(viewModel.ServicesDropdown);
        }

        /// <summary>
        ///  Developer Name :- [Your Name]
        ///  Message detail :- This API retrieves all PayGrade details from the database 
        ///                    using a stored procedure. It checks if records exist and
        ///                    returns the appropriate response.
        ///  Created Date   :- [Today's Date]
        ///  Change Date    :- [Today's Date]
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        //[HttpGet]
        //public async Task<IActionResult> FetchPayGradeDropdown1(int companyId)
        //{
        //    try
        //    {
        //        var apiKey = await _userServiceHelper.GenerateApiKeyAsync();
        //        string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayGradeUrl}";
        //        var apiResponse = await _transactionServiceHelper.GetByIdCommonAsync<IEnumerable<PayGradeMaster>>(apiUrl, companyId, apiKey);

        //        if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
        //        {
        //            var dropdownItems = apiResponse.Result.Select(pg => new SelectListItem
        //            {
        //                Value = pg.PayGrade_Id.ToString(),
        //                Text = pg.PayGradeName
        //            }).ToList();

        //            return Json(dropdownItems);
        //        }

        //        return Json(new List<SelectListItem>
        //{
        //    new SelectListItem { Value = "", Text = apiResponse?.Message ?? "No PayGrades available" }
        //});
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in FetchPayGradeDropdown: {ex.Message}");
        //        return Json(new List<SelectListItem>
        //{
        //    new SelectListItem { Value = "", Text = "An error occurred while fetching PayGrades." }
        //});
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> FetchSalaryBasicsDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<SalaryBasicsEnum>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchComponentValueTypeDropdown()
        {
            var dropdownItems = await _transactionServiceHelper.BindEnumListToDropdownAsync<ComponentValueTypeEnum>();
            return Json(dropdownItems);
        }
        [HttpGet]
        public async Task<IActionResult> FetchPayComponentListDropdown(string apiKey)
        {
            var Company_Id = SessionCompanyId;
            //var apiKey = "";//await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindPayComponentAsync(apiKey, Company_Id);
            return Json(dropdownItems.PayComponentDropdown);
        }
        [HttpGet]
        public async Task<IActionResult> FetchPayGradeDropdown(string apiKey)
        {
            var Company_Id = SessionCompanyId;
            //var apiKey = "";// await _userServiceHelper.GenerateApiKeyAsync();
            var dropdownItems = await _transactionServiceHelper.BindPayGradeAsync(apiKey);
            return Json(dropdownItems.PayComponentDropdown);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateApiKey()
        {
            try
            {
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                if (string.IsNullOrEmpty(apiKey))
                    return BadRequest(new { success = false, message = "API Key generation failed." });

                return Ok(new { success = true, apiKey });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}
