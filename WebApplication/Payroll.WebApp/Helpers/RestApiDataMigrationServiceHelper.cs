/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-201                                                                  *
 *  Description:                                                                                    *
     *  This Helper class will used use this class to make API call and perform operaiton like      *                                   
     *  SerializeObject and DeserializeObject (Bulk Upload).                                        *                                   
 *                                                                                                  *
 *  Author: Harshida Parmar                                                                         *
 *  Date  : 02-Dec-'24                                                                              *
 *                                                                                                  *
 ****************************************************************************************************/
using DataMigrationService.BAL.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using Payroll.Common.Repository.Interface;
using Payroll.WebApp.Common;
using System.Net.Http;
using System.Text;

namespace Payroll.WebApp.Helpers
{
    public class RestApiDataMigrationServiceHelper
    {
        #region CTOR        
        private readonly HttpClient _httpClientMigration;
        private readonly ApiSettings _apiSettings;
        private readonly ICachingServiceRepository _cachingService;
        public RestApiDataMigrationServiceHelper(ICachingServiceRepository cachingService, IOptions<ApiSettings> apiSettings, HttpClient httpClient)
        {
            _httpClientMigration = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this._apiSettings = apiSettings.Value;
            _cachingService = cachingService;
        }

        #endregion
        #region BulkUploadDepartment
        public async Task<ApiResponseModel<DepartmentStage>> PostDepartmentStageAsync(string apiUrl, DepartmentStage departmentStage)
        {
            #region [Request]
            // var responseModel = new ApiResponseModel<DepartmentStage> { IsSuccess = false };
            ApiResponseModel<DepartmentStage> responseModel = new ApiResponseModel<DepartmentStage>();
            try
            {
                // Serialize the DepartmentStage entity to JSON
                var jsonContent = JsonConvert.SerializeObject(departmentStage);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request to the API
                var response = await _httpClientMigration.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    #region [Response]
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        // Deserialize the response JSON to ApiResponseModel
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<DepartmentStage>>(responseJson)
                                         ?? new ApiResponseModel<DepartmentStage> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                    #endregion
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostDepartmentStageAsync: {ex.Message}");
                throw;
            }
            return responseModel;
            #endregion
        }
        #endregion
        #region BulkUploadContractorDocument
        public async Task<ApiResponseModel<ContractorDocumentMaster>> PostContractorDepartmentStageAsync(string apiUrl, ContractorDocumentMaster departmentStage)
        {
            #region [Request]
            ApiResponseModel<ContractorDocumentMaster> responseModel = new ApiResponseModel<ContractorDocumentMaster>();
            try
            {
                // Serialize the DepartmentStage entity to JSON
                var jsonContent = JsonConvert.SerializeObject(departmentStage);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request to the API
                var response = await _httpClientMigration.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    #region [Response]
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        // Deserialize the response JSON to ApiResponseModel
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<ContractorDocumentMaster>>(responseJson)
                                         ?? new ApiResponseModel<ContractorDocumentMaster> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                    #endregion
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostDepartmentStageAsync: {ex.Message}");
                throw;
            }
            return responseModel;
            #endregion
        }
        public async Task<ApiResponseModel<ContractDocumentFTP>> UpdateContractorDocumentAsync(string apiUrl, ContractDocumentFTP departmentStage)
        {
            #region [Request]
            ApiResponseModel<ContractDocumentFTP> responseModel = new ApiResponseModel<ContractDocumentFTP>();
            try
            {
                // Serialize the DepartmentStage entity to JSON
                var jsonContent = JsonConvert.SerializeObject(departmentStage);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request to the API
                var response = await _httpClientMigration.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    #region [Response]
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        // Deserialize the response JSON to ApiResponseModel
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<ContractDocumentFTP>>(responseJson)
                                         ?? new ApiResponseModel<ContractDocumentFTP> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                    #endregion
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostDepartmentStageAsync: {ex.Message}");
                throw;
            }
            return responseModel;
            #endregion
        }
        #endregion
        #region BindDPForServiceName
        public async Task<List<ServiceImportMaster>> GetServiceNameAsync(string apiUrl)
        {
            List<ServiceImportMaster> serviceList = new List<ServiceImportMaster>();

            try
            {
                var response = await _httpClientMigration.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    #region [Response]
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        // Deserialize the response JSON to ApiResponseModel
                        var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<ServiceImportMaster>>(responseJson);

                        if (responseModel != null && responseModel.Data != null)
                        {
                            // Deserialize the Data into a List<ServiceImportMaster>
                            serviceList = JsonConvert.DeserializeObject<List<ServiceImportMaster>>(responseModel.Data.ToString());
                        }
                        else
                        {
                            // Handle case where Data is null or empty
                            Console.WriteLine("No data in the API response.");
                        }
                    }
                    #endregion
                }
                else
                {
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetServiceNameAsync: {ex.Message}");
                throw;
            }

            return serviceList;
        }
        #endregion

        #region BindDPFForServiceData
        public async Task<List<Dictionary<string, object>>> GetServiceDataAsync(string apiUrl)
        {
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
            int returnCount = 0; // Initialize returnCount
            try
            {
                var response = await _httpClientMigration.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    #region [Response]
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        // Deserialize response JSON to a generic API response model
                        var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<object>>(responseJson);

                        if (responseModel != null && responseModel.Data != null)
                        {
                            // Deserialize the Data into a List<Dictionary<string, object>>
                            dataList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseModel.Data.ToString());
                        }
                        else
                        {
                            Console.WriteLine("No data in the API response.");
                        }

                    }
                    #endregion
                }
                else
                {
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetServiceDataAsync: {ex.Message}");
                throw;
            }

            return dataList;
        }

        #endregion

        #region BindReturnServiceData
        public async Task<(List<Dictionary<string, object>> dataList, int returnCount)> GetReturnServiceDataAsync(string apiUrl)
        {
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
            int returnCount = 0; // Initialize returnCount

            try
            {
                var response = await _httpClientMigration.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        // Deserialize response JSON to a generic API response model
                        var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<object>>(responseJson);

                        if (responseModel != null && responseModel.Data != null)
                        {
                            // Deserialize the Data into a List<Dictionary<string, object>>
                            dataList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseModel.Data.ToString());
                        }

                        // Extract returnCount if available in the response
                        if (responseModel != null && responseModel.returnCount != null)
                        {
                            returnCount = Convert.ToInt32(responseModel.returnCount);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetServiceDataAsync: {ex.Message}");
                throw;
            }

            return (dataList, returnCount);
        }

        #endregion

        #region DownloadSampleAndHelpFile
        public async Task<List<UploadTemplateMaster>> GetSampleAndHelpFileAsync(string apiUrl)
        {
            List<UploadTemplateMaster> serviceList = new List<UploadTemplateMaster>();

            try
            {
                var response = await _httpClientMigration.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    #region [Response]
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        // Deserialize the response JSON to ApiResponseModel
                        var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<UploadTemplateMaster>>(responseJson);

                        if (responseModel != null && responseModel.Data != null)
                        {
                            // Deserialize the Data into a List<ServiceImportMaster>
                            serviceList = JsonConvert.DeserializeObject<List<UploadTemplateMaster>>(responseModel.Data.ToString());
                        }
                        else
                        {
                            // Handle case where Data is null or empty
                            Console.WriteLine("No data in the API response.");
                        }
                    }
                    #endregion
                }
                else
                {
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetServiceNameAsync: {ex.Message}");
                throw;
            }

            return serviceList;
        }
        #endregion


        #region Post Common Data service wise
        public async Task<ApiResponseModel<TResponse>> PostCommonAsync<TRequest, TResponse>(string apiUrl, TRequest request)
        {
            var responseModel = new ApiResponseModel<TResponse>();

            try
            {
                var jsonContent = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClientMigration.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(jsonResponse);
                    responseModel.IsSuccess = true;
                    responseModel.Message = responseModel.Message;
                    responseModel.MessageType = responseModel.MessageType;
                }
                else
                {
                    responseModel.IsSuccess = false;
                    responseModel.Message = $"API call failed with status code {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Error in PostCommonAsync: {ex.Message}");
                responseModel.IsSuccess = false;
                responseModel.Message = "An unexpected error occurred.";
            }

            return responseModel;
        }
        #endregion
    }
}
