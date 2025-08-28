using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using UserService.BAL.Requests;
using Payroll.WebApp.Common;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using Payroll.Common.ApplicationConstant;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Payroll.Common.Repository.Interface;
using Payroll.WebApp.Models;
using System.Net.Http;
using System.Text;
using PayrollMasterService.BAL.Requests;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Net;
using Payroll.Common.APIKeyManagement.Service;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace Payroll.WebApp.Helpers
{
    public class RestApiMasterServiceHelper
    {
        #region CTOR        
        private readonly HttpClient _httpClientMaster;
        private readonly ApiSettings _apiSettings;
        private readonly ICachingServiceRepository _cachingService;

        // ✅ Inject HttpClient Directly
        public RestApiMasterServiceHelper(ICachingServiceRepository cachingService, IOptions<ApiSettings> apiSettings, HttpClient httpClientMaster)
        {
            _httpClientMaster = httpClientMaster ?? throw new ArgumentNullException(nameof(httpClientMaster));
            _apiSettings = apiSettings.Value;
            _cachingService = cachingService;
        }
        #endregion
        #region Get All Record For Grid 
        /// <summary>
        /// Created By:- Harshida Parmar
        /// Created Date:- 18-12-'24
        /// Note:- Get All Master Table Details  
        /// </summary>
        public async Task<ApiResponseModel<IEnumerable<T>>> GetAllRecordsAsync<T>(string url)
        {
            var responseModel = new ApiResponseModel<IEnumerable<T>>();

            try
            {
                // Make the HTTP GET request
                HttpResponseMessage response = await _httpClientMaster.GetAsync(url);

                // Ensure the response status is successful
                response.EnsureSuccessStatusCode();

                // Read the response as a JSON string
                string responseJson = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into the generic response model
                if (Utility.NotEmptyNotNA(responseJson))
                {
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<T>>>(responseJson)
                                    ?? new ApiResponseModel<IEnumerable<T>>();
                }
                else
                {
                    responseModel.IsSuccess = false;
                    responseModel.Message = "No data found in response.";
                    responseModel.StatusCode = ApiResponseStatusConstant.NoContent;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllRecordsAsync: {ex.Message}");
                throw;
            }

            return responseModel;
        }

        #endregion

        #region GET ALL UserRoleMapping entity
        /// <summary>
        /// Created By:- Harshida Parmar
        /// Created Date:- 18-12-'24
        /// Note:- Get All Department Master Details  
        /// </summary>
        public async Task<ApiResponseModel<IEnumerable<DepartmentMaster>>> GetAllDepartmentMasterAsync(string url)
        {
            #region [Request]	
            var responseModel = new ApiResponseModel<IEnumerable<DepartmentMaster>>();
            try
            {
                // Server Uri: /MappingUserRoleApi/getalluserrole
                HttpResponseMessage response = await _httpClientMaster.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();

                #region [Response]
                if (Utility.NotEmptyNotNA(responseJson))
                {
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<DepartmentMaster>>>(responseJson);

                    if (responseModel == null)
                    {
                        responseModel = new ApiResponseModel<IEnumerable<DepartmentMaster>>();
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserRoleMappingsAsync: {ex.Message}");
                throw;
            }
            return responseModel;
            #endregion
        }
        #endregion
        #region Get Company based on ID
        public async Task<ApiResponseModel<T>> GetAllApiResponseByIdAsync<T>(string url) where T : class
        {
            var responseModel = new ApiResponseModel<T>();
            HttpResponseMessage response = await _httpClientMaster.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseJson = await response.Content.ReadAsStringAsync();
            if (Utility.NotEmptyNotNA(responseJson))    
            {
                responseModel = JsonConvert.DeserializeObject<ApiResponseModel<T>>(responseJson)
                                ?? new ApiResponseModel<T>();
            }
            else
            {
                responseModel.IsSuccess = false;
                responseModel.Message = "No valid data found in the response.";
                responseModel.StatusCode = ApiResponseStatusConstant.NoContent;
            }
            return responseModel;
        }

        public async Task<ApiResponseModel<CompanyProfile>> GetAllRecordsByCompanyIdAsync(string url)
        {
            var responseModel = new ApiResponseModel<CompanyProfile>();
            HttpResponseMessage response = await _httpClientMaster.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();

            if (Utility.NotEmptyNotNA(responseJson))
            {
                responseModel = JsonConvert.DeserializeObject<ApiResponseModel<CompanyProfile>>(responseJson)
                                ?? new ApiResponseModel<CompanyProfile>();
            }
            else
            {
                responseModel.IsSuccess = false;
                responseModel.Message = "No valid data found in the response.";
                responseModel.StatusCode = ApiResponseStatusConstant.NoContent;
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<CompanyCorrespondance>> GetCompanyCorrespondanceByIdAsync(string url)
        {
            var responseModel = new ApiResponseModel<CompanyCorrespondance>();
            HttpResponseMessage response = await _httpClientMaster.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();

            if (Utility.NotEmptyNotNA(responseJson))
            {
                responseModel = JsonConvert.DeserializeObject<ApiResponseModel<CompanyCorrespondance>>(responseJson)
                                ?? new ApiResponseModel<CompanyCorrespondance>();
            }
            else
            {
                responseModel.IsSuccess = false;
                responseModel.Message = "No valid data found in the response.";
                responseModel.StatusCode = ApiResponseStatusConstant.NoContent;
            }
            return responseModel;
        }
        #endregion
        #region Common Method For Call Web API Added By Priyanshi Jain 09 Jan 2025   
        public async Task<T> GetDataAndCountWithKeyAsync<T>(string apiUrl, string apiKey)
        {
            T resultData = default;

            try
            {
                _httpClientMaster.DefaultRequestHeaders.Clear();
                _httpClientMaster.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

                var response = await _httpClientMaster.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<T>>(responseJson);

                    if (responseModel != null && responseModel.Result != null)
                    {
                        resultData = responseModel.Result;
                    }
                }
                else
                {
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDataWithKeyAsync: {ex.Message}");
                throw;
            }

            return resultData;
        }
        public async Task<List<T>> GetListWithKeyAsync<T>(string apiUrl, string apiKey)
        {
            List<T> resultList = new List<T>();

            try
            {
                #region ADD KEY TO HEADER                   
                _httpClientMaster.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientMaster.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion

                var response = await _httpClientMaster.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    // Deserialize the API response into a generic type
                    var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<List<T>>>(responseJson);

                    if (responseModel != null && responseModel.Result != null)
                    {
                        resultList = responseModel.Result;
                    }
                    else
                    {
                        Console.WriteLine("No data in the API response.");
                    }
                }
                else
                {
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetListAsync: {ex.Message}");
                throw;
            }

            return resultList;
        }
        public async Task<ApiResponseModel<TResponse>> DeleteCommonWithKeyAsync<TRequest, TResponse>(string apiUrl, TRequest request, string apiKey)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                #region ADD KEY TO HEADER                   
                _httpClientMaster.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientMaster.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send DELETE request with body
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(apiUrl),
                    Content = content // Include JSON body
                };

                var response = await _httpClientMaster.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson)
                                       ?? new ApiResponseModel<TResponse> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<TResponse>> GetByIdCommonWithKeyAsync<TResponse>(string apiUrl, int id, string apiKey)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                #region ADD KEY TO HEADER                   
                _httpClientMaster.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientMaster.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                // Construct the URL with the ID
                var fullUrl = $"{apiUrl}";

                // Make GET request
                var response = await _httpClientMaster.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson)
                                        ?? new ApiResponseModel<TResponse> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdCommonAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }
        #region PUT/ UPDATE WITH AND WITHOUT DTO [USING API KEY]
        public async Task<ApiResponseModel<TResponse>> PutCommonWithKeyAsync<TRequest, TResponse>(string apiUrl, TRequest request, string apiKey)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                #region ADD KEY TO HEADER                   
                _httpClientMaster.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientMaster.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                // Make PUT request
                var response = await _httpClientMaster.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson)
                                        ?? new ApiResponseModel<TResponse> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PutAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<T>> PutSingleCommonWithKeyAsync<T>(string apiUrl, T request, string apiKey) //Added By Harshida
        {
            return await PutCommonWithKeyAsync<T, T>(apiUrl, request, apiKey);
        }
        #endregion

        #region POST/ INSERT WITH AND WITHOUT DTO [USING API KEY]
        public async Task<ApiResponseModel<TResponse>> PostCommonWithKeyAsync<TRequest, TResponse>(string apiUrl, TRequest request, string apiKey)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                #region ADD KEY TO HEADER                   
                _httpClientMaster.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientMaster.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                // Make POST request
                var response = await _httpClientMaster.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson)
                                        ?? new ApiResponseModel<TResponse> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }

        public async Task<SalaryStructureApiResponseModel<SalarySimulatorDTO>> PostSalarySimulatorWithKeyAsync<TRequest>(string apiUrl, TRequest request, string apiKey)
        {
            var responseModel = new SalaryStructureApiResponseModel<SalarySimulatorDTO> { IsSuccess = false };
            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                #region ADD KEY TO HEADER
                _httpClientMaster.DefaultRequestHeaders.Clear();
                _httpClientMaster.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion

                // Make POST request
                var response = await _httpClientMaster.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<SalaryStructureApiResponseModel<SalarySimulatorDTO>>(responseJson)
                                        ?? new SalaryStructureApiResponseModel<SalarySimulatorDTO> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<T>> PostSingleCommonWithKeyAsync<T>(string apiUrl, T request, string apiKey) //Added By Harshida
        {
            return await PostCommonWithKeyAsync<T, T>(apiUrl, request, apiKey);
        }
        #endregion
        public async Task<ApiResponseModel<TResponse>> PostCommonAsync<TRequest, TResponse>(string apiUrl, TRequest request)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request
                var response = await _httpClientMaster.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson)
                                        ?? new ApiResponseModel<TResponse> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<T>> GetCommonAsync<T>(string url)
        {
            var responseModel = new ApiResponseModel<T>();
            try
            {
                HttpResponseMessage response = await _httpClientMaster.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();

                if (Utility.NotEmptyNotNA(responseJson))
                {
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<T>>(responseJson) ?? new ApiResponseModel<T>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAsync: {ex.Message}");
                throw; // Optionally, add custom exception handling or logging here
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<T>> GetCommonKeyAsync<T>(string url, string apiKey)
        {
            var responseModel = new ApiResponseModel<T>();
            try
            {
                #region ADD KEY TO HEADER                   
                _httpClientMaster.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientMaster.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                HttpResponseMessage response = await _httpClientMaster.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();

                if (Utility.NotEmptyNotNA(responseJson))
                {
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<T>>(responseJson) ?? new ApiResponseModel<T>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAsync: {ex.Message}");
                throw; // Optionally, add custom exception handling or logging here
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<TResponse>> GetByIdCommonAsync<TResponse>(string apiUrl, int id)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Construct the URL with the ID
                var fullUrl = $"{apiUrl}";

                // Make GET request
                var response = await _httpClientMaster.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson)
                                        ?? new ApiResponseModel<TResponse> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdCommonAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<TResponse>> GetByIdListCommonAsync<TResponse>(string apiUrl, int id)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Construct the URL with the ID
                var fullUrl = $"{apiUrl}";

                // Make GET request
                var response = await _httpClientMaster.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        // Parse response as JToken to inspect structure
                        var jsonToken = JToken.Parse(responseJson);

                        if (jsonToken["result"] != null)
                        {
                            if (jsonToken["result"].Type == JTokenType.Array)
                            {
                                // If "result" is an array, take the first item
                                var listResponse = jsonToken.ToObject<ApiResponseModel<List<TResponse>>>();
                                if (listResponse?.Result != null && listResponse.Result.Any())
                                {
                                    responseModel = new ApiResponseModel<TResponse>
                                    {
                                        IsSuccess = listResponse.IsSuccess,
                                        Message = listResponse.Message,
                                        Result = listResponse.Result.FirstOrDefault()
                                    };
                                }
                            }
                            else
                            {
                                // If "result" is a single object, deserialize normally
                                responseModel = jsonToken.ToObject<ApiResponseModel<TResponse>>()
                                                ?? new ApiResponseModel<TResponse> { IsSuccess = false, Message = "No data returned from the API." };
                            }
                        }
                        else
                        {
                            responseModel.Message = "Invalid API response format.";
                        }
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdCommonAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<TResponse>> PutCommonAsync<TRequest, TResponse>(string apiUrl, TRequest request)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make PUT request
                var response = await _httpClientMaster.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson)
                                        ?? new ApiResponseModel<TResponse> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PutAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<TResponse>> DeleteCommonAsync<TRequest, TResponse>(string apiUrl, TRequest request)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send DELETE request with body
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(apiUrl),
                    Content = content // Include JSON body
                };

                var response = await _httpClientMaster.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson)
                                       ?? new ApiResponseModel<TResponse> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string apiUrl, TRequest requestData)
        {
            try
            {
                // Serialize the request object to JSON
                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Make the HTTP POST request
                using var response = await _httpClientMaster.PostAsync(apiUrl, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                // Read the response as a string
                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                // Deserialize the JSON response
                if (string.IsNullOrWhiteSpace(responseJson))
                    throw new HttpRequestException("API response is empty.");

                return JsonConvert.DeserializeObject<TResponse>(responseJson) ?? throw new InvalidOperationException("Deserialization returned null.");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Request timeout or task was canceled: {ex.Message}");
                throw new TimeoutException("The request took too long and was canceled.", ex);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request failed: {ex.Message}");
                throw;
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization failed: {ex.Message}");
                throw;
            }
        }
        public async Task<ApiResponseModel<TResponse>> SendRequestAsync<TRequest, TResponse>(HttpMethod method, string apiUrl, TRequest request = default)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };

            try
            {
                // Initialize HTTP request message
                var requestMessage = new HttpRequestMessage(method, apiUrl);

                // If the request method is POST or PUT, serialize the request body
                if (method == HttpMethod.Post || method == HttpMethod.Put)
                {
                    var jsonContent = JsonConvert.SerializeObject(request);
                    requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }

                // Send the request
                var response = await _httpClientMaster.SendAsync(requestMessage);

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson)
                                        ?? new ApiResponseModel<TResponse> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendRequestAsync: {ex.Message}");
                responseModel.Message = "An error occurred while calling the API.";
            }

            return responseModel;
        }

		#endregion


		#region Added By Krunali
        /// <summary>
        /// Create By :- Krunali
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
		public async Task<List<SubsidiaryMaster>> GetSubsidiaryMasterAllAsync(string url)
		{
			var responseModel = new List<SubsidiaryMaster>();

			try
			{
				HttpResponseMessage response = await _httpClientMaster.GetAsync(url);
				response.EnsureSuccessStatusCode();
				string responseJson = await response.Content.ReadAsStringAsync();

				Console.WriteLine("Response JSON: " + responseJson); // Debugging log

				if (!string.IsNullOrWhiteSpace(responseJson))
				{
					responseModel = JsonConvert.DeserializeObject<List<SubsidiaryMaster>>(responseJson)
									?? new List<SubsidiaryMaster>();
				}
			}

			catch (HttpRequestException ex)
			{
				Console.WriteLine($"HTTP Request Error: {ex.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetSubsidiaryMasterAsync: {ex.Message}");
			}

			return responseModel;
		}
        /// <summary>
        /// Created By:- Krunali
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
		public async Task<List<SelectListItem>> BindEnumToDropdownSubsidiaryTypeAsync<TEnum>() where TEnum : Enum
		{
			var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

			var dropdownItems = enumValues.Select(e => new SelectListItem
			{
				Value = Convert.ToInt32(e).ToString(), // Numeric value of the enum
				Text = e.ToString()
			}).ToList();

			return await Task.FromResult(dropdownItems);
		}

        #region SubsidiaryMaster Method
        public async Task<ApiResponseModel<SubsidiaryMasterDTO>> PostSubSidiaryMasterAsync(string apiUrl, SubsidiaryMaster subsidiaryMaster)
        {
            var responseModel = new ApiResponseModel<SubsidiaryMasterDTO> { IsSuccess = false };
            try
            {
                // Serialize the entity to JSON
                var jsonContent = JsonConvert.SerializeObject(subsidiaryMaster);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request
                var response = await _httpClientMaster.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<SubsidiaryMasterDTO>>(responseJson)
                                        ?? new ApiResponseModel<SubsidiaryMasterDTO> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"Record Already Exists!: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostUserAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<SubsidiaryMaster>> GetSubsidiaryMasterByIdAsync(string url)
        {
            var responseModel = new ApiResponseModel<SubsidiaryMaster>();
            HttpResponseMessage response = await _httpClientMaster.GetAsync(url);

            //response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();

            if (Utility.NotEmptyNotNA(responseJson))
            {
                responseModel = JsonConvert.DeserializeObject<ApiResponseModel<SubsidiaryMaster>>(responseJson)
                                ?? new ApiResponseModel<SubsidiaryMaster>();
            }
            else
            {
                responseModel.IsSuccess = false;
                responseModel.Message = "No valid data found in the response.";
                responseModel.StatusCode = ApiResponseStatusConstant.NoContent;
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<IEnumerable<SubsidiaryMaster>>> GetSubsidiaryMasterAsync(string url)
        {
            var responseModel = new ApiResponseModel<IEnumerable<SubsidiaryMaster>>();
            try
            {
                HttpResponseMessage response = await _httpClientMaster.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                if (Utility.NotEmptyNotNA(responseJson))
                {
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<SubsidiaryMaster>>>(responseJson);

                    if (responseModel == null)
                    {
                        responseModel = new ApiResponseModel<IEnumerable<SubsidiaryMaster>>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }
        public async Task<ApiResponseModel<SubsidiaryMasterDTO>> PutSubsidiaryMasterAsync(string apiUrl, SubsidiaryMaster subsidiaryMaster)
        {
            var responseModel = new ApiResponseModel<SubsidiaryMasterDTO> { IsSuccess = false };
            try
            {
                // Serialize the entity to JSON
                var jsonContent = JsonConvert.SerializeObject(subsidiaryMaster);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request
                var response = await _httpClientMaster.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<SubsidiaryMasterDTO>>(responseJson)
                                        ?? new ApiResponseModel<SubsidiaryMasterDTO> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostUserAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }

		//public async Task<HttpResponseMessage> DeleteSubsidiaryMasterAsync(string apiUrl, SubsidiaryMaster ui)
		//{
		//    if (ui == null)
		//        throw new ArgumentNullException(nameof(ui));

		//    var uri = new Uri(apiUrl);
		//    var json = JsonConvert.SerializeObject(ui);
		//    var content = new StringContent(json, Encoding.UTF8, "application/json");

		//    using (var request = new HttpRequestMessage(HttpMethod.Delete, uri))
		//    {
		//        request.Content = content;  // Attach the JSON body to the DELETE request
		//        return await _httpClientMaster.SendAsync(request);
		//    }
		//}
		public async Task<ApiResponseModel<SubsidiaryMaster>> DeleteSubsidiaryMasterAsync(string apiUrl, SubsidiaryMaster ui)
		{
			if (ui == null)
				throw new ArgumentNullException(nameof(ui));

			var uri = new Uri(apiUrl);
			var json = JsonConvert.SerializeObject(ui);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			using (var request = new HttpRequestMessage(HttpMethod.Delete, uri))
			{
				request.Content = content;  // Attach the JSON body to the DELETE request

				using (var response = await _httpClientMaster.SendAsync(request))
				{
					var responseString = await response.Content.ReadAsStringAsync();

					if (!response.IsSuccessStatusCode)
					{
						return new ApiResponseModel<SubsidiaryMaster>
						{
							IsSuccess = false,
							Message = "Failed to delete subsidiary",
							StatusCode = (int)response.StatusCode
						};
					}

					return JsonConvert.DeserializeObject<ApiResponseModel<SubsidiaryMaster>>(responseString);
				}
			}
		}
		#endregion
		#endregion

		#region Update/Insert Company Financial
		public async Task<ApiResponseModel<TResponse>> PostCompanyFinancialDetailsAsync<TRequest, TResponse>(string apiUrl, TRequest request)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request
                var response = await _httpClientMaster.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson)
                                        ?? new ApiResponseModel<TResponse> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error Response: {errorResponse}");

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorResponseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(errorResponse);
                        if (errorResponseModel != null && errorResponseModel.MessageType != 1)
                        {
                            //responseModel.Message = $"Invalid MessageType: {errorResponseModel.MessageType}. Response: {errorResponseModel.Message}";
                            responseModel.Message = $"Response:- {errorResponseModel.Message}";
                            responseModel.StatusCode = (int)response.StatusCode;
                        }
                        else
                        {
                            responseModel.Message = $"Bad request: {errorResponse}";
                        }
                    }
                    else
                    {
                        responseModel.Message = $"API call failed with status code: {response.StatusCode}. Response: {errorResponse}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }

        #endregion
        #region Code from Other Service Class
        public async Task<LocationViewModel> BindCityWiseLocationsDataAsync(int? City_ID)
        {
            LocationViewModel obj = new LocationViewModel();

            string locationApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetAllCityWiseLocationMasterDetailUrl}/{City_ID}";

            // Directly fetching data without caching
            obj.LocationDropdown = await BindDropdownAsync<LocationMaster>(
                locationApiUrl,
                item => item.Location_Id.ToString(),
                item => item.LocationName,
                "No Company Available"
            );

            return obj;
        }

        public async Task<LocationViewModel> BindLocationsDataAsync()
        {
            LocationViewModel obj = new LocationViewModel();

            string locationApiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllLocationMasterDetailUrl;

            // Directly fetching data without caching
            obj.LocationDropdown = await BindDropdownAsync<LocationMaster>(
                locationApiUrl,
                item => item.Location_Id.ToString(),
                item => item.LocationName,
                "No Country Available"
            );

            return obj;
        }

        public async Task<CityViewModel> BindCityDataAsync(int? state_ID)
        {
            CityViewModel obj = new CityViewModel();
            string citiesApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetAllCityMasterDetailUrl}/{state_ID}";

            // Directly fetching data without caching
            obj.CitysDropdown = await BindDropdownAsync<CityMaster>(
                citiesApiUrl,
                item => item.City_ID.ToString(),
                item => item.City_Name,
                "No Country Available"
            );

            return obj;
        }

        public async Task<StateViewModel> BindStateDataAsync(int? Country_Id)
        {
            StateViewModel obj = new StateViewModel();
            string statesApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetAllStateMasterDetailUrl}/{Country_Id}";

            // Directly fetching data without caching
            obj.StatesDropdown = await BindDropdownAsync<StateMaster>(
                statesApiUrl,
                item => item.State_Id.ToString(),
                item => item.StateName,
                "No Country Available"
            );

            return obj;
        }

        public async Task<DepartmentViewModel> BindDepartmentDataAsync(int id)
        {
            DepartmentViewModel obj = new DepartmentViewModel();

            string departmentApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetDepartmentMasterDetailByIdUrl}/{id}";

            // Directly fetching data without caching
            obj.DepartmentDropdown = await BindDropdownAsync<DepartmentMaster>(
                departmentApiUrl,
                item => item.Department_Id.ToString(),
                item => item.DepartmentName,
                "No Departments Available"
            );

            return obj;
        }

        public async Task<CompanyViewModel> BindCompaniesDataAsync()
        {
            CompanyViewModel obj = new CompanyViewModel();
            string companyApiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllCompanyMasterDetailUrl;

            // Directly fetching data without caching
            obj.CompanyDropdown = await BindDropdownAsync<CompanyMaster>(
                companyApiUrl,
                item => item.Company_Id.ToString(),
                item => item.CompanyName,
                "No Company Available"
            );

            return obj;
        }

        /// <summary>
        /// Created By :- HArshida Parmar
        /// Created Date:- 07-02-25
        /// </summary>
        /// <returns></returns>
        public async Task<CompanyTypeViewModel> BindCompanyTypeAsync()
        {
            CompanyTypeViewModel obj = new CompanyTypeViewModel();

            string companyTypeApiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllCompanyTypeUrl;
         
            obj.CompanyTypeDropdown = await BindDropdownAsync<CompanyTypeRequest>(
                companyTypeApiUrl,
                item => item.CompanyType_ID.ToString(),
                item => item.CompanyType_Name,
                "No Company Type Available"
            );

            return obj;
        }

        /// <summary>
        /// Created By :- HArshida Parmar
        /// Created Date:- 13-02-25
        /// </summary>
        /// <returns></returns>
        public async Task<CompanyCurrencyViewModel> BindCurrencyAsync()
        {
            CompanyCurrencyViewModel obj = new CompanyCurrencyViewModel();

            string companyTypeApiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllCompanyCurrencyUrl;
          
            obj.CompanyCurrencyDropdown = await BindDropdownAsync<CurrencyRequest>(
                companyTypeApiUrl,
                item => item.Currency_Id.ToString(),
                item => item.Currency_Name,
                "No Company Currency Available"
            );

            return obj;
        }
        /// <summary>
        /// Created By :- HArshida Parmar
        /// Created Date:- 26-06-25
        /// </summary>
        /// <returns></returns>
        public async Task<AccountHeadViewModel> BindAccountHeadAsync()
        {
            AccountHeadViewModel obj = new AccountHeadViewModel();
            string accountHeadApiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllAccountingTypeUrl;
            obj.AccountHeadDropdown = await BindDropdownAsync<AccountType>(
                accountHeadApiUrl,
                item => item.AccountType_ID.ToString(),
                item => item.AccountType_Name,
                "No Account Head Available"
            );
            return obj;
        }

        /// <summary>
        /// Created By :- HArshida Parmar
        /// Created Date:- 26-06-25
        /// </summary>
        /// <returns></returns>
        public async Task<GLViewModel> BindGLAsync(int parentId)
        {
            GLViewModel obj = new GLViewModel();

            string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetParentGroupUrl}/{parentId}";

            obj.GLDropdown = await BindDropdownAsync<GLGroup>(
                apiUrl,
                item => item.GL_Group_Id.ToString(),
                item => item.Group_Name,
                "No GL Group Available"
            );

            return obj;
        }

        /// <summary>
        /// Created By :- HArshida Parmar
        /// Created Date:- 30-02-25
        /// </summary>
        /// <returns></returns>
        public async Task<FormulaViewModel> BindFormulaTypeAsync(string apiKey)
        {
            FormulaViewModel obj = new FormulaViewModel();

            string companyTypeApiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllFormulaUrl;

            obj.FormulaDropdown = await BindDropdownWithKeyAsync<FormulaMasterDTO>(
                companyTypeApiUrl,
                item => item.Formula_Id.ToString(),
                item => item.FormulaName,
                apiKey, // pass it further
                "No Formula Type Available"               
            );
            return obj;
        }
        public async Task<ApiResponseModel<CompanyLocationMapDto>> BindCompanyLocationDataAsync(int? companyId,int? userId)
        {
            // Directly fetching data without caching
            var queryString = $"{_apiSettings.PayrollMasterServiceEndpoints.GetCompanylocationmapDetailUrl}?companyId={companyId}&userId={userId}";

            try
            {
                var response = await _httpClientMaster.GetAsync(queryString);

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponseModel<CompanyLocationMapDto>
                    {
                        IsSuccess = false,
                        Message = $"Failed to fetch data. Status Code: {response.StatusCode}"
                    };
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var resultData = System.Text.Json.JsonSerializer.Deserialize<ApiResponseModel<CompanyLocationMapDto>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return resultData ?? new ApiResponseModel<CompanyLocationMapDto> { IsSuccess = false, Message = "Invalid response data" };
            }
            catch (Exception ex)
            {
                return new ApiResponseModel<CompanyLocationMapDto>
                {
                    IsSuccess = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }
        public async Task<AreaViewModel> BindAreaLocationDataAsync(int id)
        {
            AreaViewModel obj = new AreaViewModel();
            string statesApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetAreaLocationMasterDetailByIdUrl}/{id}";

            // Directly fetching data without caching
            obj.AreasDropdown = await BindDropdownAsync<AreaMaster>(
                statesApiUrl,
                item => item.Area_Id.ToString(),
                item => item.AreaName,
                "No Country Available"
            );

            return obj;
        }
        public async Task<DepartmentViewModel> BindAllDepartmentDataAsync()
        {
            DepartmentViewModel obj = new DepartmentViewModel();

            string departmentApiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllDepartmentMasterUrl;


            // Directly fetching data without caching
            obj.DepartmentDropdown = await BindDropdownAsync<DepartmentMaster>(
                departmentApiUrl,
                item => item.Department_Id.ToString(),
                item => item.DepartmentName,
                "No Departments Available"
            );

            return obj;
        }

        public async Task<FloorViewModel> BindFloorDataAsync()
        {
            FloorViewModel obj = new FloorViewModel();
            string floorApiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllFloorMasterUrl;
           
            // Directly fetching data without caching
            obj.FloorsDropdown = await BindDropdownAsync<FloorMaster>(
                floorApiUrl,
                item => item.Floor_Id.ToString(),
                item => item.Floor_No,
                "No Departments Available"
            );

            return obj;
        }

        public async Task<SalaryStractureViewModel> BindSalaryStructureAsync(int salaryId)
        {
            SalaryStractureViewModel obj = new SalaryStractureViewModel();
            salaryId = 0;
            string salaryApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetAllSalaryStructureByIdUrl}/{salaryId}";

            obj.SalaryStructureDropdown = await BindDropdownAsync<SalaryStructureDropdownDTO>(
                salaryApiUrl,
                item => item.SalaryStructure_Hdr_Id.ToString(),
                item => item.SalaryStructureName,
                "No Salary Stracture Available"
            );
            return obj;
        }


        #endregion
            #region Common Helper Class Bind Dropdown with KEY 
            /// <summary>
            /// A generic method to bind data fetched from an API to a dropdown list.
            /// </summary>
            /// <typeparam name="T">The type of data to be used for binding the dropdown.</typeparam>
            /// <param name="apiUrl">The URL of the API to fetch data from.</param>
            /// <param name="valueSelector">A function to select the value for each dropdown item.</param>
            /// <param name="textSelector">A function to select the display text for each dropdown item.</param>
            /// <param name="noDataMessage">The message to display if no data is available (default is "No data available").</param>
            /// <returns>A list of <see cref="SelectListItem"/> to populate the dropdown.</returns>
        public async Task<List<SelectListItem>> BindDropdownWithKeyAsync<T>(
            string apiUrl,
            Func<T, string> valueSelector,
            Func<T, string> textSelector,
            string apiKey,
            string noDataMessage = "No data available"            
        )
        {
            var resultList = await GetListWithKeyAsync<T>(apiUrl, apiKey);

            if (resultList != null && resultList.Any())
            {
                return resultList.Select(item => new SelectListItem
                {
                    Value = valueSelector(item),
                    Text = textSelector(item)
                }).ToList();
            }
            else
            {
                return new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = noDataMessage }
            };
            }
        }
       
        #endregion

        #region CommonHelper Class HEre 
        /// <summary>
        /// A generic method to fetch data from an API and return it as a list of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the data to be returned.</typeparam>
        /// <param name="apiUrl">The URL of the API to fetch data from.</param>
        /// <returns>A list of type T containing the API response data.</returns>
        /// <exception cref="Exception">Thrown if an error occurs during the API request or deserialization.</exception>
        public async Task<List<T>> GetListAsync<T>(string apiUrl)
        {
            List<T> resultList = new List<T>();

            try
            {
                var response = await _httpClientMaster.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    // Deserialize the API response into a generic type
                    var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<List<T>>>(responseJson);

                    if (responseModel != null && responseModel.Result != null)
                    {
                        resultList = responseModel.Result;
                    }
                    else
                    {
                        Console.WriteLine("No data in the API response.");
                    }
                }
                else
                {
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetListAsync: {ex.Message}");
                throw;
            }

            return resultList;
        }

        /// <summary>
        /// A generic method to bind data fetched from an API to a dropdown list.
        /// </summary>
        /// <typeparam name="T">The type of data to be used for binding the dropdown.</typeparam>
        /// <param name="apiUrl">The URL of the API to fetch data from.</param>
        /// <param name="valueSelector">A function to select the value for each dropdown item.</param>
        /// <param name="textSelector">A function to select the display text for each dropdown item.</param>
        /// <param name="noDataMessage">The message to display if no data is available (default is "No data available").</param>
        /// <returns>A list of <see cref="SelectListItem"/> to populate the dropdown.</returns>
        public async Task<List<SelectListItem>> BindDropdownAsync<T>(
            string apiUrl,
            Func<T, string> valueSelector,
            Func<T, string> textSelector,
            string noDataMessage = "No data available"
        )
        {
            var resultList = await GetListAsync<T>(apiUrl);

            if (resultList != null && resultList.Any())
            {
                return resultList.Select(item => new SelectListItem
                {
                    Value = valueSelector(item),
                    Text = textSelector(item)
                }).ToList();
            }
            else
            {
                return new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = noDataMessage }
            };
            }
        }

        /// <summary>
        /// A generic method to fetch data from an API and return it as a single object of type T.
        /// It tries to handle both array-based and object-based responses.
        /// </summary>
        /// <typeparam name="T">The type of data to be returned.</typeparam>
        /// <param name="apiUrl">The URL of the API to fetch data from.</param>
        /// <returns>A single object of type T representing the API response.</returns>
        /// <exception cref="Exception">Thrown if an error occurs during the API request or deserialization.</exception>
        public async Task<T> GetDataAsync<T>(string apiUrl)
        {
            try
            {
                var response = await _httpClientMaster.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    // Try to deserialize as a List<T> (array-based response)
                    try
                    {
                        var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<List<T>>>(responseJson);
                        if (responseModel != null && responseModel.Result != null)
                        {
                            return (T)(object)responseModel.Result; // Return as List<T> wrapped in T
                        }
                    }
                    catch (System.Text.Json.JsonException ex)
                    {
                        // If deserialization to List<T> fails, try deserializing to a single object (object-based response)
                        var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<T>>(responseJson);
                        if (responseModel != null && responseModel.Result != null)
                        {
                            return responseModel.Result; // Return as single object of type T
                        }
                        else
                        {
                            Console.WriteLine("No data in the API response.");
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
                Console.WriteLine($"Error in GetDataAsync: {ex.Message}");
                throw;
            }

            return default(T);
        }

        /// <summary>
        /// A method to bind enum values to a dropdown list.
        /// </summary>
        /// <typeparam name="TEnum">The enum type to be used for binding the dropdown.</typeparam>
        /// <returns>A list of <see cref="SelectListItem"/> to populate the dropdown with enum values.</returns>
        public async Task<List<SelectListItem>> BindEnumToDropdownAsync<TEnum>() where TEnum : Enum
        {
            var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            var dropdownItems = enumValues.Select(e => new SelectListItem
            {
                Value = e.ToString(),
                Text = e.ToString()
            }).ToList();

            return await Task.FromResult(dropdownItems);
        }
        #endregion

    }
}
