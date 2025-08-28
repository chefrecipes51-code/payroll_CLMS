/********************************************************************************************************
 *  Jira Task Ticket : PAYROLL-280                                                                      *
 *  Description:                                                                                        *
 *  This method fetches the company location map data based on the provided company ID. It retrieves    *
 *  data from an external API and caches the result for subsequent requests. The cache expiration is    *
 *  set to 5 minutes to avoid frequent API calls. If the data is not found in the cache, it makes an    *
 *  API call to fetch the data and store it in the cache.                                               *
 *                                                                                                      *
 *  Methods: 
 *  - BindCountriesDataAsync : Fetches country data and returns it in a cached response model. 
 *  - BindUserTypeDataAsync  : Fetches User Type data and returns it in a cached response model. 
 *  - BindCompaniesDataAsync : Fetches comapny data and returns it in a cached response model. 
 *  - BindCompanyLocationDataAsync : Fetches company location map data and returns it in a cached       *
 *  response model.                                                                              
 *                                                                                                      *
 *  Author: Priyanshi Jain                                                                              *
 *  Date  : 24-Sep-2024                                                                                 *
 *                                                                                                      *
 ********************************************************************************************************/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Payroll.Common.ApplicationModel;
using Payroll.Common.Repository.Interface;
using Payroll.WebApp.Models;
using PayrollMasterService.BAL.Models;
using RoleService.BAL.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using UserService.BAL.Models;

namespace Payroll.WebApp.Helpers
{
    public class BindDropdownDataHelper
    {
        //private readonly ICachingServiceRepository _cachingService;
        //private readonly ApiSettings _apiSettings;
        //private readonly CommonHelper _commonHelper;
        //private readonly IHttpClientFactory _httpClientFactory;
        //public BindDropdownDataHelper(
        //    ICachingServiceRepository cachingService,
        //     IOptions<ApiSettings> apiSettings,
        //    CommonHelper commonHelper,
        //    IHttpClientFactory httpClientFactory)
        //{
        //    _cachingService = cachingService;
        //    _apiSettings = apiSettings.Value; // Extract the ApiSettings value
        //    _commonHelper = commonHelper;
        //    _httpClientFactory = httpClientFactory;
        //}
        //public async Task<CountriesViewModel> BindCountriesDataAsync()
        //{
        //    CountriesViewModel obj = new CountriesViewModel();
        //    string countriesApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetAllCountriesUrl;

        //    // Directly fetching data without caching
        //    obj.CountriesDropdown = await _commonHelper.BindDropdownAsync<CountryMaster>(
        //        countriesApiUrl,
        //        item => item.Country_Id.ToString(),
        //        item => item.CountryName,
        //        "No Country Available"
        //    );

        //    return obj;
        //}
        //public async Task<LocationViewModel> BindCityWiseLocationsDataAsync(int? City_ID)
        //{
        //    LocationViewModel obj = new LocationViewModel();

        //    string locationApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetAllCityWiseLocationMasterDetailUrl}/{City_ID}";

        //    // Directly fetching data without caching
        //    obj.LocationDropdown = await _commonHelper.BindDropdownAsync<LocationMaster>(
        //        locationApiUrl,
        //        item => item.Location_Id.ToString(),
        //        item => item.LocationName,
        //        "No Company Available"
        //    );

        //    return obj;
        //}
        //public async Task<LocationViewModel> BindLocationsDataAsync()
        //{
        //    LocationViewModel obj = new LocationViewModel();

        //    string locationApiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllLocationMasterDetailUrl;

        //    // Directly fetching data without caching
        //    obj.LocationDropdown = await _commonHelper.BindDropdownAsync<LocationMaster>(
        //        locationApiUrl,
        //        item => item.Location_Id.ToString(),
        //        item => item.LocationName,
        //        "No Country Available"
        //    );

        //    return obj;
        //}
        //public async Task<CityViewModel> BindCityDataAsync(int? state_ID)
        //{
        //    CityViewModel obj = new CityViewModel();
        //    string citiesApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetAllCityMasterDetailUrl}/{state_ID}";

        //    // Directly fetching data without caching
        //    obj.CitysDropdown = await _commonHelper.BindDropdownAsync<CityMaster>(
        //        citiesApiUrl,
        //        item => item.City_ID.ToString(),
        //        item => item.City_Name,
        //        "No Country Available"
        //    );

        //    return obj;
        //}
        //public async Task<StateViewModel> BindStateDataAsync(int? Country_Id)
        //{
        //    StateViewModel obj = new StateViewModel();
        //    string statesApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetAllStateMasterDetailUrl}/{Country_Id}";

        //    // Directly fetching data without caching
        //    obj.StatesDropdown = await _commonHelper.BindDropdownAsync<StateMaster>(
        //        statesApiUrl,
        //        item => item.State_Id.ToString(),
        //        item => item.StateName,
        //        "No Country Available"
        //    );

        //    return obj;
        //}
        //public async Task<DepartmentViewModel> BindDepartmentDataAsync()
        //{
        //    DepartmentViewModel obj = new DepartmentViewModel();

        //    string departmentApiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllDepartmentMasterUrl;

        //    // Directly fetching data without caching
        //    obj.DepartmentDropdown = await _commonHelper.BindDropdownAsync<DepartmentMaster>(
        //        departmentApiUrl,
        //        item => item.Department_Id.ToString(),
        //        item => item.DepartmentName,
        //        "No Departments Available"
        //    );

        //    return obj;
        //}
        //public async Task<UserTypeViewModel> BindUserTypeDataAsync()
        //{
        //    UserTypeViewModel obj = new UserTypeViewModel();
        //    string userTypeApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetAllUserTypeUrl;
        //    string cacheKey = "userTypeDropdownCache";
        //    // fetching data using caching
        //    obj.UserTypeDropdown = await _cachingService.GetOrCreate(
        //        cacheKey,
        //        async cacheEntry =>
        //        {
        //            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);  // Ensure cache expiration time is set correctly.
        //            var userTypeList = await _commonHelper.BindDropdownAsync<UserTypeMaster>(
        //                userTypeApiUrl,
        //                item => item.UserType_Id.ToString(),
        //                item => item.UserTypeName,
        //                "No User Type Available"
        //            );
        //            return userTypeList;
        //        }
        //    );
        //    return obj;
        //}
        //public async Task<CompanyViewModel> BindCompaniesDataAsync()
        //{
        //    CompanyViewModel obj = new CompanyViewModel();
        //    string companyApiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllCompanyMasterDetailUrl;
        //    string cacheKey = "companyDropdownCache";
        //    // fetching data using caching
        //    obj.CompanyDropdown = await _cachingService.GetOrCreate(
        //        cacheKey,
        //        async cacheEntry =>
        //        {
        //            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);  // Ensure cache expiration time is set correctly.
        //            var companyList = await _commonHelper.BindDropdownAsync<CompanyMaster>(
        //             companyApiUrl,
        //             item => item.Company_Id.ToString(),
        //             item => item.CompanyName,
        //             "No Company Available"
        //            );
        //            return companyList;
        //        }
        //    );
        //    return obj;
        //} 
        //public async Task<ApiResponseModel<CompanyLocationMapDto>> BindCompanyLocationDataAsync(int? companyId)
        //{
        //    // Define a cache key that is unique per companyId
        //    string cacheKey = $"companyLocationMap_{companyId}";

        //    // Fetch data using caching
        //    var data = await _cachingService.GetOrCreate(
        //        cacheKey,
        //        async cacheEntry =>
        //        {
        //            // Set cache expiration time (adjust as needed)
        //            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5); // Cache for 5 minutes or as needed

        //            // Fetch the data from the API if not in the cache
        //            var client = _httpClientFactory.CreateClient("ApiClient");
        //            var queryString = $"{_apiSettings.PayrollMasterServiceEndpoints.GetCompanylocationmapDetailUrl}?companyId={companyId}";

        //            var response = await client.GetAsync(queryString);
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                throw new Exception("Error fetching data");
        //            }

        //            var jsonString = await response.Content.ReadAsStringAsync();
        //            var resultData = System.Text.Json.JsonSerializer.Deserialize<ApiResponseModel<CompanyLocationMapDto>>(jsonString, new JsonSerializerOptions
        //            {
        //                PropertyNameCaseInsensitive = true
        //            });

        //            return resultData; // Return the fetched data to be cached
        //        }
        //    );

        //    return data;
        //}
    }
}
