/// Developer By:- Harshida Parmar
/// Note:- Using these class we can make call to TransactionService
///        All helper class has common method. 
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Payroll.Common.ApplicationModel;
using Payroll.Common.Repository.Interface;
using Payroll.WebApp.Models;
using PayrollMasterService.BAL.Models;
using System.Text;
using System.Text.Json;
using UserService.BAL.Models;
using Payroll.WebApp.Common;
using Humanizer;
using RabbitMQ.Client;
using Payroll.Common.APIKeyManagement.Service;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Net.Http;
using Payroll.WebApp.Models.DTOs;
using System.Net.Http.Headers;
using PayrollTransactionService.BAL.Models;
using System.Net;

namespace Payroll.WebApp.Helpers
{
    public class RestApiTransactionServiceHelper
    {
        private readonly HttpClient _httpClientTransaction;
        private readonly ApiSettings _apiSettings;
        private readonly ICachingServiceRepository _cachingService;

        public RestApiTransactionServiceHelper(ICachingServiceRepository cachingService, IOptions<ApiSettings> apiSettings, HttpClient httpClient
               )
        {
            _httpClientTransaction = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this._apiSettings = apiSettings.Value;
            _cachingService = cachingService;
            if (_httpClientTransaction.Timeout == TimeSpan.FromSeconds(100)) // default .NET timeout
            {
                _httpClientTransaction.Timeout = TimeSpan.FromMinutes(10); // or your desired timeout
            }

        }
        #region Common Method For Call Web API Added By Priyanshi Jain 09 Jan 2025 
        public async Task<List<T>> GetListWithKeyAsync<T>(string apiUrl, string apiKey)
        {
            List<T> resultList = new List<T>();

            try
            {
                var fullUrl = $"{apiUrl}";
                #region ADD KEY TO HEADER                   
                _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion

                var response = await _httpClientTransaction.GetAsync(fullUrl);

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
        public async Task<(List<T> Result, string Message)> GetListDataAndMessageWithKeyAsync<T>(string apiUrl, string apiKey)
        {
            List<T> resultList = new List<T>();
            string message = null;

            try
            {
                var fullUrl = $"{apiUrl}";
                _httpClientTransaction.DefaultRequestHeaders.Clear();
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

                var response = await _httpClientTransaction.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<List<T>>>(responseJson);

                    if (responseModel != null)
                    {
                        resultList = responseModel.Result ?? new List<T>();
                        message = responseModel.Message; // capture any error/info message
                    }
                }
                else
                {
                    message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                message = $"Error in GetListWithKeyAsync: {ex.Message}";
            }

            return (resultList, message);
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
                _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                // Make PUT request
                var response = await _httpClientTransaction.PutAsync(apiUrl, content);
               
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
                _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                // Make POST request
                var response = await _httpClientTransaction.PostAsync(apiUrl, content);

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

        //    public async Task<ApiResponseModel<TResponse>> PostPayrollProcessCommonWithKeyAsync<TRequest, TResponse>(
        //string apiUrl,
        //TRequest request,
        //string apiKey)
        //    {
        //        var responseModel = new ApiResponseModel<TResponse>
        //        {
        //            IsSuccess = false,
        //            StatusCode = (int)HttpStatusCode.InternalServerError
        //        };

        //        try
        //        {
        //            // Serialize the request object to JSON
        //            var jsonContent = JsonConvert.SerializeObject(request);
        //            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //            // Clear previous headers and add API Key
        //            _httpClientTransaction.DefaultRequestHeaders.Clear();
        //            _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

        //            // Perform POST request
        //            using var response = await _httpClientTransaction.PostAsync(apiUrl, content);

        //            var responseJson = await response.Content.ReadAsStringAsync();

        //            if (response.IsSuccessStatusCode)
        //            {
        //                if (!string.IsNullOrWhiteSpace(responseJson))
        //                {
        //                    var apiResult = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson);
        //                    if (apiResult != null)
        //                    {
        //                        responseModel = apiResult;
        //                    }
        //                    else
        //                    {
        //                        responseModel.Message = "Unable to deserialize API response.";
        //                    }
        //                }
        //                else
        //                {
        //                    responseModel.Message = "Empty response received from API.";
        //                }
        //            }
        //            else
        //            {
        //                responseModel.Message = $"API call failed with status code: {response.StatusCode}";
        //                responseModel.StatusCode = (int)response.StatusCode;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            responseModel.Message = $"Exception occurred: {ex.Message}";
        //        }

        //        return responseModel;
        //    }

        public async Task<ApiResponseModel<TResponse>> PostPayrollProcessCommonWithKeyAsync<TRequest, TResponse>(
 string apiUrl,
 TRequest request,
 string apiKey)
        {
            var responseModel = new ApiResponseModel<TResponse>
            {
                IsSuccess = false,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Clear previous headers and add API Key
                _httpClientTransaction.DefaultRequestHeaders.Clear();
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

                // Perform POST request
                using var response = await _httpClientTransaction.PostAsync(apiUrl, content);

                var responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrWhiteSpace(responseJson))
                    {
                        var apiResult = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(responseJson);
                        if (apiResult != null)
                        {
                            responseModel = apiResult;
                        }
                        else
                        {
                            responseModel.Message = "Unable to deserialize API response.";
                        }
                    }
                    else
                    {
                        responseModel.Message = "Empty response received from API.";
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                    responseModel.StatusCode = (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                responseModel.Message = $"Exception occurred: {ex.Message}";
            }

            return responseModel;
        }
        public async Task<ApiResponseModel<T>> PostSingleCommonWithKeyAsync<T>(string apiUrl, T request, string apiKey) //Added By Harshida
        {
            return await PostCommonWithKeyAsync<T, T>(apiUrl, request, apiKey);
        }
        #endregion
        public async Task<ApiResponseModel<T>> GetWithKeyAsync<T>(string url, string apiKey)
        {
            var responseModel = new ApiResponseModel<T> { IsSuccess = false };

            try
            {
                _httpClientTransaction.DefaultRequestHeaders.Clear();
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

                var response = await _httpClientTransaction.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<T>>(responseJson)
                                        ?? new ApiResponseModel<T> { IsSuccess = false, Message = "Invalid response." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                responseModel.Message = $"Exception: {ex.Message}";
            }

            return responseModel;
        }

        public async Task<ApiResponseModel<T>> GetCommonAsync<T>(string url)
        {
            var responseModel = new ApiResponseModel<T>();
            try
            {
                HttpResponseMessage response = await _httpClientTransaction.GetAsync(url);
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
                _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                HttpResponseMessage response = await _httpClientTransaction.GetAsync(url);
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
        public async Task<ApiResponseModel<T>> GetPayrollCommonKeyAsync<T>(string url, T request, string apiKey)
        {
            var responseModel = new ApiResponseModel<T>();
            try
            { 
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                #region ADD KEY TO HEADER                   
                _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                HttpResponseMessage response = await _httpClientTransaction.GetAsync(url);
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


        public async Task<ApiResponseModel<TResponse>> GetByIdCommonAsync<TResponse>(string apiUrl, string apiKey)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Construct the URL with the ID
                var fullUrl = $"{apiUrl}";
                #region ADD KEY TO HEADER                       
                _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                // Make GET request
                var response = await _httpClientTransaction.GetAsync(fullUrl);

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
        public async Task<ApiResponseModel<TResponse>> GetByIdCommonForContractorAsync<TResponse>(string apiUrl, int id, string apiKey)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Construct the URL with the ID
                var fullUrl = $"{apiUrl}";
                #region ADD KEY TO HEADER                       
                _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                // Make GET request
                var response = await _httpClientTransaction.GetAsync(fullUrl);

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

        public async Task<ApiResponseModel<TResponse>> GetByIdListCommonAsync<TResponse>(string apiUrl, int id, string apiKey)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Construct the URL with the ID
                var fullUrl = $"{apiUrl}";
                #region ADD KEY TO HEADER                   
                _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                // Make GET request
                var response = await _httpClientTransaction.GetAsync(fullUrl);

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
        public async Task<ApiResponseModel<TResponse>> PutCommonAsync<TRequest, TResponse>(string apiUrl, TRequest request, string apiKey)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                #region ADD KEY TO HEADER                   
                _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                // Make PUT request
                var response = await _httpClientTransaction.PutAsync(apiUrl, content);

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
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string apiUrl, TRequest requestData)
        {
            try
            {
                // Serialize the request object to JSON
                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Make the HTTP POST request
                using var response = await _httpClientTransaction.PostAsync(apiUrl, content).ConfigureAwait(false);
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
        public async Task<ApiResponseModel<T>> FetchApiResponseAsync<T>(string url)
        {
            var responseModel = new ApiResponseModel<T> { IsSuccess = false };

            try
            {
                HttpResponseMessage response = await _httpClientTransaction.GetAsync(url);
                string responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<T>>(responseJson) ?? new ApiResponseModel<T>();
                        responseModel.IsSuccess = true;
                    }
                }
                else
                {
                    // responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                responseModel.Message = $"Error in API call: {ex.Message}";
                Console.WriteLine($"Error in FetchApiResponseAsync: {ex.Message}");
            }
            return responseModel;
        }

        public async Task<ApiResponseModel<TResponse>> PostCommonAsync<TRequest, TResponse>(string apiUrl, TRequest request, string apiKey)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                #region ADD KEY TO HEADER                   
                _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                // Make POST request
                var response = await _httpClientTransaction.PostAsync(apiUrl, content);


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

        public async Task<ApiResponseModel<TResponse>> PostEntityFilterAsync<TRequest, TResponse>(string url, TRequest data, string apiKey)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };

            // Serialize the request object to JSON
            var jsonContent = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            #region ADD KEY TO HEADER                   
            _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
            _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            #endregion
            // Make POST request
            var response = await _httpClientTransaction.PostAsync(url, content);



            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                responseModel = JsonConvert.DeserializeObject<ApiResponseModel<TResponse>>(json);
                //return new ApiResponseModel<TResponse>
                //{
                //    IsSuccess = true,
                //    Data = result
                //};
            }
            return responseModel;
            //return new ApiResponseModel<TResponse>
            //{
            //    IsSuccess = false,
            //    Data = default
            //};
        }

        public async Task<ApiResponseModel<TResponse>> DeleteCommonAsync<TRequest, TResponse>(
        string apiUrl, TRequest request, string apiKey)
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
                #region ADD KEY TO HEADER                   
                _httpClientTransaction.DefaultRequestHeaders.Clear(); // Clear default headers just to be safe
                _httpClientTransaction.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
                #endregion
                var response = await _httpClientTransaction.SendAsync(requestMessage);

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
                responseModel.Message = $"Exception: {ex.Message}";
            }

            return responseModel;
        }


        public async Task<EntityTypeViewModel> BindEntityTypeAsync()
        {
            EntityTypeViewModel obj = new EntityTypeViewModel();

            string companyTypeApiUrl = _apiSettings.PayrollTransactionEndpoints.GetAllEntityTypeUrl;

            obj.EntityTypeDropdown = await BindDropdownAsync<EntityTypeDTO>(
                companyTypeApiUrl,
                item => item.ExternalUnique_Id.ToString(),
                item => item.EntityType_Name,
                "No Entity Type Available"
            );

            return obj;
        }
        /// <summary>
        /// Created By :- Priyanshi Jain
        /// Created Date:- 01-05-25
        /// </summary>
        /// <returns></returns>
        public async Task<PayGradeViewModel> BindActivePayGradeTypeAsync(string apiKey)
        {
            PayGradeViewModel obj = new PayGradeViewModel();

            string payGradeTypeApiUrl = _apiSettings.PayrollTransactionEndpoints.GetAllActivePayGreadsUrl;

            obj.PayGradeDropdown = await BindDropdownWithKeyAsync<PayGradeMasterDTO>(
                payGradeTypeApiUrl,
                item => item.PayGrade_Id.ToString(),
                item => item.PayGradeName,
                apiKey, // pass it further
                "No PayGrade Type Available"
            );
            return obj;
        }

        public async Task<DistinctLocationViewModel> BindDistinctLocationAsync(string apiKey, int companyId)
        {
            DistinctLocationViewModel obj = new DistinctLocationViewModel();

            string distinctLocationApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetDistinctLocationUrl}?company_ID={companyId}";

            obj.DistinctLocationDropdown = await BindDropdownWithKeyAsync<DistinctLocationDTO>(
                distinctLocationApiUrl,
                item => item.Location_ID.ToString(),
                item => item.LocationName,
                apiKey, // pass it further
                "No Location Available"
            );
            return obj;
        }
        public async Task<PayComponentViewModel> BindPayComponentAsync(string apiKey, int Company_Id)
        {

            PayComponentViewModel obj = new PayComponentViewModel();

            string distinctLocationApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayComponentsUrl}/{Company_Id}";

            obj.PayComponentDropdown = await BindDropdownWithKeyAsync<PayComponentDTO>(
                distinctLocationApiUrl,
                item => item.EarningDeduction_Id.ToString(),
                item => item.EarningDeductionName,
                apiKey, // pass it further
                "No Pay Component Available"
            );
            return obj;
        }
        public async Task<PayComponentViewModel> BindPayGradeAsync(string apiKey)
        {

            PayComponentViewModel obj = new PayComponentViewModel();

            string distinctLocationApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayGradeUrl}";

            obj.PayComponentDropdown = await BindDropdownWithKeyAsync<PayGradeMaster>(
                distinctLocationApiUrl,
                item => item.PayGrade_Id.ToString(),
                item => item.PayGradeName,
                apiKey, // pass it further
                "No Pay Component Available"
            );
            return obj;
        }

        public async Task<IsParentComponentViewModel> BindIsParentComponentAsync(string apiKey, int companyId)
        {
            IsParentComponentViewModel obj = new IsParentComponentViewModel();
            string selectType = "P";
            string distinctLocationApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayComponentsUrl}/{companyId}?selectType={selectType}";

            obj.ParentComponentDropdown = await BindDropdownWithKeyAsync<PayComponentDTO>(
                distinctLocationApiUrl,
                item => item.EarningDeduction_Id.ToString(),
                item => item.EarningDeductionName,
                apiKey, // pass it further
                "No Parent Component Available"
            );
            return obj;
        }

        public async Task<IsParentComponentViewModel> BindIsParentComponentChildAsync(string apiKey, int companyId, string selectType, int EarningDeduction_Id)
        {
            IsParentComponentViewModel obj = new IsParentComponentViewModel();
            string distinctLocationApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetPayComponentsChildUrl}/{companyId}?selectType={selectType}&EarningDeduction_Id={EarningDeduction_Id}";

            obj.ParentComponentDropdown = await BindDropdownWithKeyAsync<PayComponentDTO>(
                distinctLocationApiUrl,
                item => item.EarningDeduction_Id.ToString(),
                item => item.EarningDeductionName,
                apiKey, // pass it further
                "No Parent Component Available"
            );
            return obj;
        }

        public async Task<SkillCategoryViewModel> BindSkillCategoryAsync(string apiKey, int correspondance_ID,
         int? skillcategory_Id,
         bool isActive)
        {
            SkillCategoryViewModel obj = new SkillCategoryViewModel();

            string skillCategoryApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetSkillCategoryUrl}/?correspondance_ID={correspondance_ID}&isActive={true}";

            obj.SkillCategoryDropdown = await BindDropdownWithKeyAsync<SkillCategoryDTO>(
                skillCategoryApiUrl,
                item => item.Skillcategory_Id.ToString(),
                item => item.Skillcategory_Name,
                apiKey, // pass it further
                "No Skill Available"
            );
            return obj;
        }

        public async Task<TradeViewModel> BindTradeAsync(string apiKey, int companyLocationID,
         int? trade_mst_Id,
         bool isActive)
        {
            TradeViewModel obj = new TradeViewModel();

            string tradeApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetTradeMasterUrl}/?companyLocationID={companyLocationID}&isActive={true}";

            obj.TradeDropdown = await BindDropdownWithKeyAsync<TradeMasterDTO>(
                tradeApiUrl,
                item => item.Trade_mst_Id.ToString(),
                item => item.Trade_Name,
                apiKey, // pass it further
                "No Trade Available"
            );
            return obj;
        }

        public async Task<TaxRegimeViewModel> BindTaxRegimeAsync(string apiKey)
        {
            TaxRegimeViewModel obj = new TaxRegimeViewModel();

            string regimeApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllTaxregimeUrl}";

            obj.TaxRegimeDropdown = await BindDropdownWithKeyAsync<TaxRegimeMasterDTO>(
                regimeApiUrl,
                item => item.YearlyItTable_Id.ToString(),
                item => item.Regimename,
                apiKey, // pass it further
                "No Regime Available"
            );
            return obj;
        }

        public async Task<ContractorViewModel> BindContractorAsync(string apiKey, int? contractor_ID,
             int company_ID)
        {
            ContractorViewModel obj = new ContractorViewModel();
            int correspondance_ID = 0;
            string tradeApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllContractorDetailUrl}/?contractor_ID={contractor_ID}&company_ID={company_ID}&correspondance_ID={correspondance_ID}";

            obj.ContractorDropdown = await BindDropdownWithKeyAsync<ContractorMasterDTO>(
                tradeApiUrl,
                item => item.Contractor_ID.ToString(),
                item => item.Contractor_Name,
                apiKey, // pass it further
                "No Contractor Available"
            );
            return obj;
        }
        public async Task<ContractorWorkOrderViewModel> BindContractorWorkOrderAsync(string apiKey, 
            int company_ID)
        {
            ContractorWorkOrderViewModel obj = new ContractorWorkOrderViewModel();            
            string tradeApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetContractorWithWorkOrderUrl}/?company_ID={company_ID}";

            obj.ContractorWorkOrderDropdown = await BindDropdownWithKeyAsync<ContractorWorkOrderRequest>(
                tradeApiUrl,
                item => item.WorkOrder_Code.ToString(),
                item => item.ContractorWiseWorkOrder,
                apiKey, // pass it further
                "No Contractor -Work Order Available"
            );
            return obj;
        }

        public async Task<ContractorViewModel> BindContractorwithCodeAsync(string apiKey, int? contractor_ID,
           int company_ID)
        {
            ContractorViewModel obj = new ContractorViewModel();
            int correspondance_ID = 0;
            string tradeApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllContractorDetailUrl}/?contractor_ID={contractor_ID}&company_ID={company_ID}&correspondance_ID={correspondance_ID}";

            obj.ContractorDropdown = await BindDropdownWithKeyAsync<ContractorMasterDTO>(
                tradeApiUrl,
                item => item.Contractor_Code.ToString(),
                item => item.Contractor_Name,
                apiKey, // pass it further
                "No Contractor Available"
            );
            return obj;
        }
        public async Task<FinYearViewModel> BindFinancialYearAsync(string apiKey, int companyId)
        {
            FinYearViewModel obj = new FinYearViewModel();

            string regimeApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllFinancialYearRequestUrl}/?companyId={companyId}";

            obj.FinYearDropdown = await BindDropdownWithKeyAsync<FinancialYearMasterDTO>(
                regimeApiUrl,
                item => item.FinancialYearID.ToString(),
                item => item.FinYear,
                apiKey, // pass it further
                "No Financial Year Available"
            );
            return obj;
        }
        public async Task<WorkOrderViewModel> BindWorkOrderDataAsync(string apiKey, string ContractorCode)
        {
            WorkOrderViewModel obj = new WorkOrderViewModel();
            string tradeApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllWorkOrderDetailUrl}/?ContractorCode={ContractorCode}";

            obj.WorkOrderDropdown = await BindDropdownWithKeyAsync<WorkorderMasterDTO>(
                tradeApiUrl,
                item => item.WorkOrder_Code.ToString(),
                item => item.WorkOrder_No.ToString(),
                apiKey, // pass it further
                "No Workorder Available"
            );
            return obj;
        }
        public async Task<CompanyPayrollValidationViewModel> BindCompanyPayrollValidationAsync(string month_Yr)
        {
            CompanyPayrollValidationViewModel obj = new CompanyPayrollValidationViewModel();

            string distinctLocationApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllCompanyPayrollValidationDetailUrl}?month_Yr={month_Yr}";

            obj.CompanyPayrollValidationDropdown = await BindDropdownAsync<CompanyPayrollValidationDTO>(
                distinctLocationApiUrl,
                item => item.Company_Id.ToString(),
                item => item.CompanyName,
                "No company Available"
            );
            return obj;
        }
        public async Task<CompanyLocationPayrollValidationViewModel> BindCompanyLocationPayrollValidationAsync(string month_Yr, int company_ID, bool? isActive)
        {
            CompanyLocationPayrollValidationViewModel obj = new CompanyLocationPayrollValidationViewModel();

            string distinctLocationApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllCompanyLocationPayrollValidationDetailUrl}?month_Yr={month_Yr}&company_ID={company_ID}&isActive={isActive}";

            obj.CompanyLocationPayrollValidationDropdown = await BindDropdownAsync<CompanyLocationPayrollValidationDTO>(
                distinctLocationApiUrl,
                item => item.Location_ID.ToString(),
                item => item.LocationName,
                "No company location Available"
            );
            return obj;
        }

        public async Task<ContractorPayrollValidationViewModel> BindContractorPayrollValidationAsync(int company_ID, string month_Yr, bool? isActive)
        {
            ContractorPayrollValidationViewModel obj = new ContractorPayrollValidationViewModel();

            string distinctLocationApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllContractorPayrollValidationDetailUrl}?company_ID={company_ID}&month_Yr={month_Yr}&isActive={isActive}";

            obj.ContractorPayrollValidationDropdown = await BindDropdownAsync<ContractorPayrollValidationDTO>(
                distinctLocationApiUrl,
                item => item.Contractor_ID.ToString(),
                item => item.Contractor_Name,
                "No contractor Available"
            );
            return obj;
        }

        public async Task<WorkOrderPayrollValidationViewModel> BindWorkOrderPayrollValidationAsync(string month_Yr, int company_ID, bool? isActive)
        {
            WorkOrderPayrollValidationViewModel obj = new WorkOrderPayrollValidationViewModel();

            string distinctLocationApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllWorkOrderPayrollValidationDetailUrl}?month_Yr={month_Yr}&company_ID={company_ID}&isActive={isActive}";

            obj.WorkOrderPayrollValidationDropdown = await BindDropdownAsync<WorkOrderPayrollValidationDTO>(
                distinctLocationApiUrl,
                item => item.WorkOrder_Id.ToString(),
                item => item.WorkOrder_No,
                "No work order no Available"
            );
            return obj;
        }

        public async Task<PreviousMonthYearPeriodPayrollValidationViewModel> BindPreviousMonthYearPeriodAsync(int company_ID)
        {
            PreviousMonthYearPeriodPayrollValidationViewModel obj = new PreviousMonthYearPeriodPayrollValidationViewModel();

            string distinctLocationApiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPreviousMonthYearPeriod_ByCmpIdDetailUrl}?company_ID={company_ID}";

            obj.PreviousMonthYearPeriodPayrollValidationDropdown = await BindDropdownAsync<PreviousMonthYearPeriodDTO>(
                distinctLocationApiUrl,
                item => item.month_Id.ToString(),
                item => item.year,
                "No month-year Available"
            );
            return obj;
        }

        /// <summary>
        /// A method to bind enum values to a dropdown list.
        /// </summary>
        /// <typeparam name="TEnum">The enum type to be used for binding the dropdown.</typeparam>
        /// <returns>A list of <see cref="SelectListItem"/> to populate the dropdown with enum values.</returns>
        public async Task<List<SelectListItem>> BindEnumListToDropdownAsync<TEnum>() where TEnum : Enum
        {
            var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            var dropdownItems = enumValues.Select(e => new SelectListItem
            {
                Value = Convert.ToInt32(e).ToString(),
                Text = e.ToString()
            }).ToList();

            return await Task.FromResult(dropdownItems);
        }
        #endregion
        #region [COPY FROM Common Helper Class]
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
                var response = await _httpClientTransaction.GetAsync(apiUrl);

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
                var response = await _httpClientTransaction.GetAsync(apiUrl);

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
        public async Task<ApiResponseModel<T>> GetPayAsync<T>(string requestUri)
        {
            var response = await _httpClientTransaction.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponseModel<T>>(jsonString);
            }

            return new ApiResponseModel<T>
            {
                IsSuccess = false,
                Message = $"Error: {response.StatusCode}",
                StatusCode = (int)response.StatusCode
            };
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
    }
}
