/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-201                                                                  *
 *  Description:                                                                                    *
     *  This Helper class will used use this class to make API call and perform operaiton like      *                                   
     *  SerializeObject and DeserializeObject.                                                      *                                   
 *                                                                                                  *
 *  Author: Harshida Parmar                                                                         *
 *  Date  : 21-Nov-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Configuration;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.CommonDto;
using Payroll.Common.Repository.Interface;
using Payroll.WebApp.Common;
using Payroll.WebApp.Models;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UserService.BAL.Models;
using UserService.BAL.Requests;

namespace Payroll.WebApp.Helpers
{
    public class RestApiUserServiceHelper
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly ICachingServiceRepository _cachingService;
        private readonly IConfiguration _configuration;
        // ✅ Inject HttpClient Directly
        public RestApiUserServiceHelper(IConfiguration configuration,ICachingServiceRepository cachingService, IOptions<ApiSettings> apiSettings, HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this._apiSettings = apiSettings.Value;
            _cachingService = cachingService;
            _configuration = configuration;
        }
        #region Generate KEY For All Call
        public async Task<string> GenerateApiKeyAsync()
        {
            Console.WriteLine("Starting API key generation...");
            var username = _configuration["GenerateKeyUserDetails:Username"];
            var password = _configuration["GenerateKeyUserDetails:Password"];
            var apiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.PostGetGenerateAPIUrl;

            var apiKeyRequest = new ApiRquestForm
            {
                userId = username,
                Password = password,
                validityMinutes = 20, // No USE BUT copied from Delta, over there these field so I have place it, BUT OUR KEY IS USED ONCE ONYL
                maxUsage = 10 // No USE BUT copied from Delta, over there these field so I have place it, BUT OUR KEY IS USED ONCE ONYL
            };

            var apiKey = await GetPostAPIKeyAsync<ApiRquestForm>(apiUrl, apiKeyRequest);
            Console.WriteLine($"API key generated: {apiKey}");
            return apiKey;          
        }
        public async Task<string> GetPostAPIKeyAsync<TRequest>(string apiUrl, TRequest request)
        {
            string apiKey = null;

            try
            {
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        // Anonymous type to parse only apiKey from JSON
                        var parsed = JsonConvert.DeserializeAnonymousType(responseJson, new { apiKey = "", expiryDate = DateTime.MinValue });

                        if (!string.IsNullOrEmpty(parsed.apiKey))
                        {
                            return parsed.apiKey;
                        }
                    }
                }
                else
                {
                    // Log or handle error status code if needed
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostCommonAsync: {ex.Message}");
                throw;
            }
            return apiKey;  // Return the apiKey directly
        }

        #endregion
        #region GET ALL UserRoleMapping entity
        /// <summary>
        /// Created By:- Harshida Parmar
        /// Created Date:- 21-11-'24
        /// Note:- Common method to get UserRole Details        
        /// Updated By:- ...
        /// Updated Date:-...
        /// Updated Note:-...
        /// </summary>
        public async Task<ApiResponseModel<IEnumerable<UserRoleMappingRequest>>> GetUserRoleMappingsAsync(string url)
        {
            #region [Request]	
            var responseModel = new ApiResponseModel<IEnumerable<UserRoleMappingRequest>>();
            try
            {
                // Server Uri: /MappingUserRoleApi/getalluserrole
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();

                #region [Response]
                if (Utility.NotEmptyNotNA(responseJson))
                {
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<UserRoleMappingRequest>>>(responseJson);

                    if (responseModel == null)
                    {
                        responseModel = new ApiResponseModel<IEnumerable<UserRoleMappingRequest>>();
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

        #region Post UserRoleMapping entity
        /// <summary>
        /// Created By:- Harshida Parmar
        /// Created Date:- 22-11-'24
        /// Note:- Common method to Insert/Post/Add UserRole Details        
        /// Updated By:- ...
        /// Updated Date:-...
        /// Updated Note:-...
        /// </summary>
        public async Task<ApiResponseModel<UserRoleMappingDTO>> PostUserRoleMappingAsync(string apiUrl, UserRoleMapping userRoleMappingEntity)
        {
            #region [Request]
            var responseModel = new ApiResponseModel<UserRoleMappingDTO> { IsSuccess = false };

            try
            {
                // Serialize the entity to JSON
                var jsonContent = JsonConvert.SerializeObject(userRoleMappingEntity);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    #region [Response]
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<UserRoleMappingDTO>>(responseJson)
                                                ?? new ApiResponseModel<UserRoleMappingDTO> { IsSuccess = false, Message = "No data returned from the API." };

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
                Console.WriteLine($"Error in AddUserRoleMappingsAsync: {ex.Message}");
                throw;
            }
            return responseModel;
            #endregion
        }
        #endregion

        #region Delete UserRoleMapping Entity
        //public async Task<ApiResponseModel<UserRoleMapping>> DeleteUserRoleMappingAsync(string apiUrl)
        //{
        //    ApiResponseModel<UserRoleMapping> responseModel = new ApiResponseModel<UserRoleMapping>();

        //    try
        //    {
        //        var response = await _httpClient.DeleteAsync(apiUrl);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            responseModel = await response.Content.ReadFromJsonAsync<ApiResponseModel<UserRoleMapping>>();
        //        }
        //        else
        //        {
        //            responseModel.IsSuccess = false;
        //            responseModel.Message = "Error deleting the user role mapping.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        responseModel.IsSuccess = false;
        //        responseModel.Message = "Exception occurred: " + ex.Message;
        //    }

        //    return responseModel;
        //}
        #endregion

        #region User Crud Activity

        /// <summary>
        /// Created By  :- Rohit Tiwari
        /// Created Date:- 29-11-2024
        /// Updated By  :- NOT YET
        /// Updated Date:- NULL
        /// Updated Note:- NULL
        /// </summary>
        public async Task<ApiResponseModel<IEnumerable<UserRequest>>> GetUsersAsync(string url)
        {
            var responseModel = new ApiResponseModel<IEnumerable<UserRequest>>();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                if (Utility.NotEmptyNotNA(responseJson))
                {
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<UserRequest>>>(responseJson);

                    if (responseModel == null)
                    {
                        responseModel = new ApiResponseModel<IEnumerable<UserRequest>>();
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

        
        public async Task<UserListViewModel> GetUserListAsync()
        {
            UserListViewModel obj = new UserListViewModel();
            string userApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetUserListUrl;
            string cacheKey = "userListDropdownCache";
            // fetching data using caching
            obj.UserDropdown = await _cachingService.GetOrCreate(
                cacheKey,
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);  // Ensure cache expiration time is set correctly.
                    var userList = await BindDropdownAsync<UserListModel>(
                        userApiUrl,
                        item => item.UserId.ToString(),
                        item => item.UserName,
                        "No User Data Available"
                    );
                    return userList;
                }
            );
            return obj;
        }

        //chirag gurjar. 2 may 2025
        public async Task<UserListViewModel> GetLocationwiseUserListAsync(int? Company_Id, int? Location_Id)
        {
            UserListViewModel obj = new UserListViewModel();
            //string userApiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetLocationwiseUserListUrl}/{Company_Id}/{Location_Id}";
            string userApiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetLocationwiseUserListUrl}?Company_Id={Company_Id}&Location_Id={Location_Id}";


            //string cacheKey = "locationwiseUserListDropdownCache";
            // fetching data using caching
            //obj.UserDropdown = await _cachingService.GetOrCreate(
            //    cacheKey,
            //    async cacheEntry =>
            //    {
            //        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);  // Ensure cache expiration time is set correctly.
            //        var userList = await BindDropdownAsync<UserListModel>(
            //            userApiUrl,
            //            item => item.UserId.ToString(),
            //            item => item.UserName,
            //            "No User Data Available"
            //        );
            //        return userList;
            //    }
            //);

            obj.UserDropdown = await BindDropdownAsync<UserListModel>(
                      userApiUrl,
                      item => item.UserId.ToString(),
                      item => item.UserName,
                      "No User Data Available"
                  );

            return obj;
        }

        /// <summary>
        /// Created By  :- Rohit Tiwari
        /// Created Date:- 29-11-2024
        /// Updated By  :- Krunali gohil
        /// Updated Date:- NULL
        /// Updated Note:- NULL
        /// </summary>
        /// 
        public async Task<ApiResponseModel<UserInfoDTO>> GetUserByIdAsync(string url)
        {
            var responseModel = new ApiResponseModel<UserInfoDTO>();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                if (Utility.NotEmptyNotNA(responseJson))
                {
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<UserInfoDTO>>(responseJson);

                    if (responseModel == null)
                    {
                        responseModel = new ApiResponseModel<UserInfoDTO>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserByIdAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }

        public async Task<ApiResponseModel<UserInfoDTO>> GetUserRoleByIdAsync(string url)
        {
            var responseModel = new ApiResponseModel<UserInfoDTO>();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                if (Utility.NotEmptyNotNA(responseJson))
                {
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<UserInfoDTO>>(responseJson);

                    if (responseModel == null)
                    {
                        responseModel = new ApiResponseModel<UserInfoDTO>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserRoleByIdAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }
        /// <summary>
        /// Created By  :- Rohit Tiwari
        /// Created Date:- 29-11-2024
        /// Updated By  :- NOT YET
        /// Updated Date:- NULL
        /// Updated Note:- NULL
        /// </summary>
        public async Task<ApiResponseModel<UserDTO>> PostUserAsync(string apiUrl, UserRequest userRequest)
        {
            var responseModel = new ApiResponseModel<UserDTO> { IsSuccess = false };
            try
            {
                // Serialize the entity to JSON
                var jsonContent = JsonConvert.SerializeObject(userRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<UserDTO>>(responseJson)
                                        ?? new ApiResponseModel<UserDTO> { IsSuccess = false, Message = "No data returned from the API." };
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

        /// <summary>
        /// Created By  :- Rohit Tiwari
        /// Created Date:- 29-11-2024
        /// Updated By  :- NOT YET
        /// Updated Date:- NULL
        /// Updated Note:- NULL
        /// </summary>
        public async Task<ApiResponseModel<UserDTO>> PutUserAsync(string apiUrl, UserRequest userRequest)
        {
            var responseModel = new ApiResponseModel<UserDTO> { IsSuccess = false };
            try
            {
                // Serialize the entity to JSON
                var jsonContent = JsonConvert.SerializeObject(userRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<UserDTO>>(responseJson)
                                        ?? new ApiResponseModel<UserDTO> { IsSuccess = false, Message = "No data returned from the API." };
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

        /// <summary>
        /// Created By  :- Harshida Parmar
        /// Created Date:- 01-01-'25
        /// Updated By  :- NOT YET
        /// Updated Date:- NULL
        /// Updated Note:- NULL
        /// </summary>
        public async Task<HttpResponseMessage> DeleteUserAsync(string apiUrl, UserRequest ui)
        {
            HttpResponseMessage response = null;

            if (ui != null)
            {
                #region [Request]
                var uri = new Uri(apiUrl);
                var json = JsonConvert.SerializeObject(ui);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await _httpClient.PutAsync(uri, content);
                #endregion
            }
            return response;
        }
        public async Task<RoleOrLocationDTO> PutDefaultLocationRole(string apiUrl, RoleOrLocationRequest userRequest)
        {
            var responseModel = new RoleOrLocationDTO();
            try
            {
                // Serialize the entity to JSON
                var jsonContent = JsonConvert.SerializeObject(userRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request
                var response = await _httpClient.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonResponse);
                    if (jsonObject["data"] != null)
                    {
                        responseModel = jsonObject["data"].ToObject<RoleOrLocationDTO>();
                    }
                }
                else
                {
                    // responseModel.Message = "Failed to update the role or location.";
                }
            }
            catch (Exception ex)
            {
                //responseModel.Message = $"Error: {ex.Message}";
            }

            return responseModel;
        }
        #endregion
      
        #region 

        /// <summary>
        /// Created By  :- Rohit Tiwari
        /// Created Date:- 29-11-2024
        /// Updated By  :- NOT YET
        /// Updated Date:- Chirag Gurjar
        /// Updated Note:- return type model changed, Payroll-384
        /// </summary>
        public async Task<ApiResponseModel<UserActivationDTO>> VerifyResetPasswordLinkAsync(string url)
        {
            var responseModel = new ApiResponseModel<UserActivationDTO>();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseJson = await response.Content.ReadAsStringAsync();
                if (Utility.NotEmptyNotNA(responseJson))
                {
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<UserActivationDTO>>(responseJson);

                    if (responseModel == null)
                    {
                        responseModel = new ApiResponseModel<UserActivationDTO>();
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

        public async Task<ApiResponseModel<SendEmailDTO>> PostUpdateUserPasswordAsync(string apiUrl, SendEmailModel sendEmail)
        {
            var responseModel = new ApiResponseModel<SendEmailDTO>();
            try
            {
                var jsonContent = JsonConvert.SerializeObject(sendEmail);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<SendEmailDTO>>(responseJson)
                                        ?? new ApiResponseModel<SendEmailDTO> { IsSuccess = false, Message = "No data returned from the API." };
                    }
                }
                else
                {
                    responseModel.Message = $"API call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostUpdateUserPasswordAsync: {ex.Message}");
                throw;
            }
            return responseModel;
        }

        #endregion

        #region Common Method For Call Web API Added By Priyanshi Jain 09 Jan 2025
        public async Task<ApiResponseModel<TResponse>> PostCommonAsync<TRequest, TResponse>(string apiUrl, TRequest request)
        {
            var responseModel = new ApiResponseModel<TResponse> { IsSuccess = false };
            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request
                var response = await _httpClient.PostAsync(apiUrl, content);

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
                HttpResponseMessage response = await _httpClient.GetAsync(url);
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
                var response = await _httpClient.GetAsync(fullUrl);

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
                var response = await _httpClient.GetAsync(fullUrl);

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
                var response = await _httpClient.PutAsync(apiUrl, content);

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
                using var response = await _httpClient.PostAsync(apiUrl, content).ConfigureAwait(false);
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
                HttpResponseMessage response = await _httpClient.GetAsync(url);
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

        #endregion
        public async Task<ApiResponseModel<IEnumerable<UserRoleBasedMenuRequest>>> GetUserRoleMenuAsync(string url)
        {
            #region [Request]	
            var responseModel = new ApiResponseModel<IEnumerable<UserRoleBasedMenuRequest>>();
            try
            {
                // Server Uri: /MappingUserRoleApi/getalluserrole
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();

                #region [Response]
                if (Utility.NotEmptyNotNA(responseJson))
                {
                    responseModel = JsonConvert.DeserializeObject<ApiResponseModel<IEnumerable<UserRoleBasedMenuRequest>>>(responseJson);

                    if (responseModel == null)
                    {
                        responseModel = new ApiResponseModel<IEnumerable<UserRoleBasedMenuRequest>>();
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
        public async Task<ApiResponseModel<T>> GetUserLocationWiseRoleAsync<T>(string url)
        {
            var response = new ApiResponseModel<T> { IsSuccess = false };

            try
            {
                var httpResponse = await _httpClient.GetAsync(url);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var content = await httpResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<ApiResponseModel<T>>(content);

                    // Ensure IsSuccess is set correctly
                    response.IsSuccess = true;
                }
                else
                {
                    response.Message = $"Error: {httpResponse.StatusCode}";
                    response.StatusCode = (int)httpResponse.StatusCode;
                }
            }
            catch (Exception ex)
            {
                response.Message = "API request failed.";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return response;
        }
        public async Task<ApiResponseModel<RoleMenuPermissionsModelDTO>> PostUserRoleMenuMappingAsync(string apiUrl, AddUpdateUserRoleMenuRequest apiRequest)
        {
            #region [Request]
            var responseModel = new ApiResponseModel<RoleMenuPermissionsModelDTO> { IsSuccess = false };

            try
            {
                // Serialize the request object to JSON
                var jsonContent = JsonConvert.SerializeObject(apiRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Make POST request
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();

                    #region [Response]
                    if (Utility.NotEmptyNotNA(responseJson))
                    {
                        responseModel = JsonConvert.DeserializeObject<ApiResponseModel<RoleMenuPermissionsModelDTO>>(responseJson)
                                        ?? new ApiResponseModel<RoleMenuPermissionsModelDTO> { IsSuccess = false, Message = "No data returned from the API." };
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
                Console.WriteLine($"Error in PostUserRoleMenuMappingAsync: {ex.Message}");
                throw;
            }

            return responseModel;
            #endregion
        }
        public async Task<ApiResponseModel<T>> PostSignleAsync<T>(string apiUrl, T requestData)
        {
            try
            {
                // Serialize request object to JSON
                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Make the POST call
                using var response = await _httpClient.PostAsync(apiUrl, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                // Read the response
                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(responseJson))
                    throw new HttpRequestException("API response is empty.");

                // Deserialize into ApiResponseModel<T>
                return JsonConvert.DeserializeObject<ApiResponseModel<T>>(responseJson)
                       ?? throw new InvalidOperationException("Deserialization returned null.");
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
        public async Task<ApiResponseModel<T>> PutSingleAsync<T>(string apiUrl, T requestData)
        {
            try
            {
                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _httpClient.PutAsync(apiUrl, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(responseJson))
                    throw new HttpRequestException("API response is empty.");

                return JsonConvert.DeserializeObject<ApiResponseModel<T>>(responseJson)
                       ?? throw new InvalidOperationException("Deserialization returned null.");
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
           
        }

        public async Task<string> GetCheckUserEmailAsync(string url)
        {
            string resultMessage = string.Empty;

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(responseJson))
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<string>>(responseJson);

                    if (apiResponse.IsSuccess)
                    {
                        resultMessage = ApiResponseMessageConstant.NoIssues;
                    }
                    else
                    {
                        resultMessage = ApiResponseMessageConstant.EmailalreadyConflict;
                    }
                }
                else
                {
                    resultMessage = ApiResponseMessageConstant.SomethingWrong;
                }
            }
            else
            {
                // Handle HTTP error
                resultMessage = ApiResponseMessageConstant.SomethingWrong;
            }
            return resultMessage;
        }
        //public async Task<T> GetAsync<T>(string apiUrl)
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetAsync(apiUrl);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            throw new HttpRequestException($"API call failed: {response.StatusCode}");
        //        }

        //        var responseJson = await response.Content.ReadAsStringAsync();
        //        return JsonConvert.DeserializeObject<T>(responseJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in GetAsync: {ex.Message}");
        //        throw; // Optionally, add custom exception handling or logging here
        //    }
        //}
        public async Task<T> GetAsync<T>(string apiUrl)
        {
            try
            {
                using var response = await _httpClient.GetAsync(apiUrl).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(responseJson))
                    throw new HttpRequestException("API response is empty.");

                return JsonConvert.DeserializeObject<T>(responseJson) ?? throw new InvalidOperationException("Deserialization returned null.");
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

        }

        #region Common Helper Move HEre 
        public async Task<CountriesViewModel> BindCountriesDataAsync()
        {
            CountriesViewModel obj = new CountriesViewModel();
            string countriesApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetAllCountriesUrl;

            // Directly fetching data without caching
            obj.CountriesDropdown = await BindDropdownAsync<CountryMaster>(
                countriesApiUrl,
                item => item.Country_Id.ToString(),
                item => item.CountryName,
                "No Country Available"
            );
            return obj;
        }
        public async Task<CountriesViewModel> BindCommonCountriesDataAsync()
        {
            CountriesViewModel obj = new CountriesViewModel();
            string countriesApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetAllCountriesUrl;

            // Directly fetching data without caching
            obj.CountriesDropdown = await BindDropdownAsync<CountryMaster>(
                countriesApiUrl,
                item => item.Country_Id.ToString(),
                item => item.CountryName,
                "No Country Available"
            );

            return obj;
        }
        //public async Task<LocationViewModel> BindCityWiseLocationsDataAsync(int? City_ID)
        //{
        //    LocationViewModel obj = new LocationViewModel();

        //    string locationApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetAllCityWiseLocationMasterDetailUrl}/{City_ID}";

        //    // Directly fetching data without caching
        //    obj.LocationDropdown = await BindDropdownAsync<LocationMaster>(
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
        //    obj.LocationDropdown = await BindDropdownAsync<LocationMaster>(
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
        //    obj.CitysDropdown = await BindDropdownAsync<CityMaster>(
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
        //    obj.StatesDropdown = await BindDropdownAsync<StateMaster>(
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
        //    obj.DepartmentDropdown = await BindDropdownAsync<DepartmentMaster>(
        //        departmentApiUrl,
        //        item => item.Department_Id.ToString(),
        //        item => item.DepartmentName,
        //        "No Departments Available"
        //    );

        //    return obj;
        //}
        public async Task<UserTypeViewModel> BindUserTypeDataAsync()
        {
            UserTypeViewModel obj = new UserTypeViewModel();
            string userTypeApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetAllUserTypeUrl;
            string cacheKey = "userTypeDropdownCache";
            // fetching data using caching
            obj.UserTypeDropdown = await _cachingService.GetOrCreate(
                cacheKey,
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);  // Ensure cache expiration time is set correctly.
                    var userTypeList = await BindDropdownAsync<UserTypeMaster>(
                        userTypeApiUrl,
                        item => item.UserType_Id.ToString(),
                        item => item.UserTypeName,
                        "No User Type Available"
                    );
                    return userTypeList;
                }
            );
            return obj;
        }
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
        //            var companyList = await BindDropdownAsync<CompanyMaster>(
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
        public async Task<ApiResponseModel<CompanyLocationMapDto>> BindCompanyLocationDataAsync(int? companyId)
        {
            // Define a cache key that is unique per companyId
            string cacheKey = $"companyLocationMap_{companyId}";

            // Fetch data using caching
            var data = await _cachingService.GetOrCreate(
                cacheKey,
                async cacheEntry =>
                {
                    // Set cache expiration time (adjust as needed)
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5); // Cache for 5 minutes or as needed

                    // ✅ Use `_httpClient` directly instead of `_httpClientFactory`
                    var queryString = $"{_apiSettings.PayrollMasterServiceEndpoints.GetCompanylocationmapDetailUrl}?companyId={companyId}";

                    var response = await _httpClient.GetAsync(queryString); // ✅ Use `_httpClient`
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error fetching data. Status Code: {response.StatusCode}");
                    }

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var resultData = System.Text.Json.JsonSerializer.Deserialize<ApiResponseModel<CompanyLocationMapDto>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return resultData; // Return the fetched data to be cached
                }
            );
            return data;
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
                var response = await _httpClient.GetAsync(apiUrl);

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
                var response = await _httpClient.GetAsync(apiUrl);

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
        #region RestApiLoginServiceHelper
        public async Task<ApiResponseModel<UserRequest>> GetUsersRecordAsync(string url)
        {
            var apiResponse = new ApiResponseModel<UserRequest>();

            try
            {
                var response = await _httpClient.GetAsync(url); //Calling UserApi/GetByIdAuth Controller
                var responseJson = await response.Content.ReadAsStringAsync();


                if (Utility.NotEmptyNotNA(responseJson))
                {
                    apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<UserRequest>>(responseJson)
                                    ?? new ApiResponseModel<UserRequest>();
                }
            }
            catch (Exception ex)
            {
                // Log the exception using a logger instead of Console.WriteLine for production use.
                var errorMessage = $"Error in{ex.Message}";
            }

            return apiResponse;
        }
        public async Task<ApiResponseModel<UserRequest>> UpdateLoginActivityAsync(string url, UserRequest model)
        {
            // Send HTTP POST request
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                // Deserialize and return successful response
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseModel<UserRequest>>();
                return apiResponse;
            }
            else
            {
                // Handle errors
                var errorResponse = new ApiResponseModel<UserRequest>
                {
                    IsSuccess = false,
                    Message = $"Error: {response.ReasonPhrase}",
                    StatusCode = (int)response.StatusCode
                };
                return errorResponse;
            }
        }
        #endregion

        #region approverProcess
        public async Task<ModulesViewModel> BindModulesDataAsync()
        {
            ModulesViewModel obj = new ModulesViewModel();
            string modulesApiUrl = _apiSettings.BaseUrlPayrollUserServiceEndpoints.GetAllModuleUrl;

            // Directly fetching data without caching
            obj.ModulesDropdown = await BindDropdownAsync<ModuleMaster>(
                modulesApiUrl,
                item => item.Module_Id.ToString(),
                item => item.ModuleName,
                "No Module Available"
            );
            return obj;
        }

        public async Task<ServicesViewModel> BindServicesDataAsync(int? Module_Id)
        {
            ServicesViewModel obj = new ServicesViewModel();
            string modulesApiUrl = $"{_apiSettings.BaseUrlPayrollUserServiceEndpoints.GetAllServicesByModuleIdUrl}/{Module_Id}";

            // Directly fetching data without caching
            obj.ServicesDropdown = await BindDropdownAsync<ServiceMaster>(
                modulesApiUrl,
                item => item.ServiceID.ToString(),
                item => item.ServiceName,
                "No Service Available"
            );
            return obj;
        }
        #endregion

    }


}
