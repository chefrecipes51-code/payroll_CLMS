/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-280                                                                  *
 *  Description:                                                                                   *
 *  This helper class contains generic methods for interacting with APIs,                            *
 *  handling data retrieval, and dynamically binding dropdown lists.                                 *
 *  It is responsible for the following functionalities:                                            *
 *                                                                                                 *
 *  - GetListAsync<T>        : Retrieves a list of data from the API and returns it as a list of    *
 *                            the specified type T.                                                  *
 *  - BindDropdownAsync<T>   : Binds data retrieved from the API to a dropdown list, dynamically    *
 *                            populating the dropdown with value and text based on custom selectors.  *
 *  - GetDataAsync<T>        : Fetches data from an API and returns it as a single object or list   *
 *                            based on the API response format.                                      *
 *  - BindEnumToDropdownAsync: Binds an enum to a dropdown list, where each enum value is displayed *
 *                            as a dropdown item.                                                  *
 *                                                                                                 *
 *  Author: Priyanshi Jain                                                                           *
 *  Date  : 30-Dec-2024                                                                              *
 *                                                                                                 *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using Payroll.WebApp.Models;
using static Payroll.Common.EnumUtility.EnumUtility;

/// <summary>
/// Class is deprected 
/// </summary>
public class CommonHelper
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonHelper"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to make API requests.</param>
    public CommonHelper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    ///// <summary>
    ///// A generic method to fetch data from an API and return it as a list of the specified type.
    ///// </summary>
    ///// <typeparam name="T">The type of the data to be returned.</typeparam>
    ///// <param name="apiUrl">The URL of the API to fetch data from.</param>
    ///// <returns>A list of type T containing the API response data.</returns>
    ///// <exception cref="Exception">Thrown if an error occurs during the API request or deserialization.</exception>
    //public async Task<List<T>> GetListAsync<T>(string apiUrl)
    //{
    //    List<T> resultList = new List<T>();

    //    try
    //    {
    //        var response = await _httpClient.GetAsync(apiUrl);

    //        if (response.IsSuccessStatusCode)
    //        {
    //            var responseJson = await response.Content.ReadAsStringAsync();

    //            // Deserialize the API response into a generic type
    //            var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<List<T>>>(responseJson);

    //            if (responseModel != null && responseModel.Result != null)
    //            {
    //                resultList = responseModel.Result;
    //            }
    //            else
    //            {
    //                Console.WriteLine("No data in the API response.");
    //            }
    //        }
    //        else
    //        {
    //            Console.WriteLine($"API call failed with status code: {response.StatusCode}");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error in GetListAsync: {ex.Message}");
    //        throw;
    //    }

    //    return resultList;
    //}

    ///// <summary>
    ///// A generic method to bind data fetched from an API to a dropdown list.
    ///// </summary>
    ///// <typeparam name="T">The type of data to be used for binding the dropdown.</typeparam>
    ///// <param name="apiUrl">The URL of the API to fetch data from.</param>
    ///// <param name="valueSelector">A function to select the value for each dropdown item.</param>
    ///// <param name="textSelector">A function to select the display text for each dropdown item.</param>
    ///// <param name="noDataMessage">The message to display if no data is available (default is "No data available").</param>
    ///// <returns>A list of <see cref="SelectListItem"/> to populate the dropdown.</returns>
    //public async Task<List<SelectListItem>> BindDropdownAsync<T>(
    //    string apiUrl,
    //    Func<T, string> valueSelector,
    //    Func<T, string> textSelector,
    //    string noDataMessage = "No data available"
    //)
    //{
    //    var resultList = await GetListAsync<T>(apiUrl);

    //    if (resultList != null && resultList.Any())
    //    {
    //        return resultList.Select(item => new SelectListItem
    //        {
    //            Value = valueSelector(item),
    //            Text = textSelector(item)
    //        }).ToList();
    //    }
    //    else
    //    {
    //        return new List<SelectListItem>
    //        {
    //            new SelectListItem { Value = "", Text = noDataMessage }
    //        };
    //    }
    //}

    ///// <summary>
    ///// A generic method to fetch data from an API and return it as a single object of type T.
    ///// It tries to handle both array-based and object-based responses.
    ///// </summary>
    ///// <typeparam name="T">The type of data to be returned.</typeparam>
    ///// <param name="apiUrl">The URL of the API to fetch data from.</param>
    ///// <returns>A single object of type T representing the API response.</returns>
    ///// <exception cref="Exception">Thrown if an error occurs during the API request or deserialization.</exception>
    //public async Task<T> GetDataAsync<T>(string apiUrl)
    //{
    //    try
    //    {
    //        var response = await _httpClient.GetAsync(apiUrl);

    //        if (response.IsSuccessStatusCode)
    //        {
    //            var responseJson = await response.Content.ReadAsStringAsync();

    //            // Try to deserialize as a List<T> (array-based response)
    //            try
    //            {
    //                var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<List<T>>>(responseJson);
    //                if (responseModel != null && responseModel.Result != null)
    //                {
    //                    return (T)(object)responseModel.Result; // Return as List<T> wrapped in T
    //                }
    //            }
    //            catch (JsonException ex)
    //            {
    //                // If deserialization to List<T> fails, try deserializing to a single object (object-based response)
    //                var responseModel = JsonConvert.DeserializeObject<ApiResponseModel<T>>(responseJson);
    //                if (responseModel != null && responseModel.Result != null)
    //                {
    //                    return responseModel.Result; // Return as single object of type T
    //                }
    //                else
    //                {
    //                    Console.WriteLine("No data in the API response.");
    //                }
    //            }
    //        }
    //        else
    //        {
    //            Console.WriteLine($"API call failed with status code: {response.StatusCode}");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error in GetDataAsync: {ex.Message}");
    //        throw;
    //    }

    //    return default(T);
    //}

    ///// <summary>
    ///// A method to bind enum values to a dropdown list.
    ///// </summary>
    ///// <typeparam name="TEnum">The enum type to be used for binding the dropdown.</typeparam>
    ///// <returns>A list of <see cref="SelectListItem"/> to populate the dropdown with enum values.</returns>
    //public async Task<List<SelectListItem>> BindEnumToDropdownAsync<TEnum>() where TEnum : Enum
    //{
    //    var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

    //    var dropdownItems = enumValues.Select(e => new SelectListItem
    //    {
    //        Value = e.ToString(),
    //        Text = e.ToString()
    //    }).ToList();

    //    return await Task.FromResult(dropdownItems);
    //}
}


