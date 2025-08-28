using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using Payroll.Common.CommonRequest;
using Payroll.WebApp.Common;
using System.Text;
using UserService.BAL.Requests;

namespace Payroll.WebApp.Helpers
{
    public class RestApiLoginServiceHelper
    {
        private static HttpClient _httpClient123;
        public static RestApiLoginServiceHelper Instance { get; } = new RestApiLoginServiceHelper();
        static RestApiLoginServiceHelper()
        {
            if (_httpClient123 == null)
            {
                _httpClient123 = new HttpClient();
            }
        }

        //public async Task<ApiResponseModel<UserRequest>> AuthUsersAsync(string url, LoginRequest login)
        //{
        //    var responseModel = new ApiResponseModel<UserRequest>();

        //    try
        //    {
        //        var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
        //        var response = await _httpClient123.PostAsync(url, content);

        //        var responseJson = await response.Content.ReadAsStringAsync();
        //        if (Utility.NotEmptyNotNA(responseJson))
        //        {
        //            responseModel = JsonConvert.DeserializeObject<ApiResponseModel<UserRequest>>(responseJson)
        //                            ?? new ApiResponseModel<UserRequest>();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception using a logger instead of Console.WriteLine for production use.
        //        var errorMessage = $"Error in {nameof(AuthUsersAsync)}: {ex.Message}";
        //    }

        //    return responseModel;
        //}

        //public async Task<ApiResponseModel<UserRequest>> GetUsersAsync(string url)
        //{
        //    var apiResponse = new ApiResponseModel<UserRequest>();

        //    try
        //    {
        //        var response = await _httpClient123.GetAsync(url); //Calling UserApi/GetByIdAuth Controller
        //        var responseJson = await response.Content.ReadAsStringAsync();


        //        if (Utility.NotEmptyNotNA(responseJson))
        //        {
        //            apiResponse = JsonConvert.DeserializeObject<ApiResponseModel<UserRequest>>(responseJson)
        //                            ?? new ApiResponseModel<UserRequest>();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception using a logger instead of Console.WriteLine for production use.
        //        var errorMessage = $"Error in {nameof(AuthUsersAsync)}: {ex.Message}";
        //    }

        //    return apiResponse;
        //}

        // Method to call UpdateLoginActivity endpoint
        //public async Task<ApiResponseModel<UserRequest>> UpdateLoginActivityAsync(string url, UserRequest model)
        //{
        //    // Send HTTP POST request
        //    HttpResponseMessage response = await _httpClient123.PostAsJsonAsync(url, model);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Deserialize and return successful response
        //        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseModel<UserRequest>>();
        //        return apiResponse;
        //    }
        //    else
        //    {
        //        // Handle errors
        //        var errorResponse = new ApiResponseModel<UserRequest>
        //        {
        //            IsSuccess = false,
        //            Message = $"Error: {response.ReasonPhrase}",
        //            StatusCode = (int)response.StatusCode
        //        };
        //        return errorResponse;
        //    }
        //}

    }
}
