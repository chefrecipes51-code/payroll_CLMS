/****************************************************************************************************
 *  Jira Task Ticket :  Payroll-594                                                                 *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for SalaryStructure.                                     *
 *  It includes APIs to retrieve, create, update, and delete SalaryStructure                         *
 *  Author: Chirag Gurjar                                                                           *
 *  Date  : 18-Mar-2025                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payroll.Common.Helpers;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Extensions;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
//using PayrollTransactionService.BAL.Models;
using System.Net.Http;
using UserService.BAL.Models;

namespace Payroll.WebApp.Controllers
{
    public class SalaryStructureController : SharedUtilityController
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        //private readonly BindDropdownDataHelper _dropdownHelper;
        private readonly RestApiUserServiceHelper _userServiceHelper;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
        //private readonly RestApiTransactionServiceHelper _transactionServiceHelper;
        private readonly HttpClient _httpClient;

        private int SessionCompanyId
        {
            get
            {
                var sessionCompanyData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");
                return sessionCompanyData.CompanyDetails.FirstOrDefault()?.Company_Id ?? 0;
            }
        }
        public SalaryStructureController(RestApiMasterServiceHelper masterServiceHelper
            , RestApiUserServiceHelper userServiceHelper

            , IMapper mapper, IOptions<ApiSettings> apiSettings
            , HttpClient httpClient)
        {
            this._mapper = mapper;
            this._apiSettings = apiSettings.Value;
            //_dropdownHelper = dropdownHelper;
            _userServiceHelper = userServiceHelper;
            _masterServiceHelper = masterServiceHelper;

            _httpClient = httpClient;
        }

        [HttpGet("SalaryStructure/SalaryStructureGrid")]
        public async Task<IActionResult> SalaryStructureGrid(int Id = 0)
        {
            try
            {
                string queryParams = $"/{Id}"; // Used these to allow these parameter for CLMS parameter

                string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetSalaryStructureGridUrl}";
                var apiResponse = await _masterServiceHelper.GetAllRecordsAsync<SalaryStructureGrid>($"{apiUrl}{queryParams}");

                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    var approvalList = apiResponse.Result;
                    return View("SalaryStructureGrid", approvalList);  // 👈 returns View with model
                }

                // Return empty list with error message in TempData if needed
                TempData["Error"] = apiResponse?.Message ?? "Failed to load data.";
                return View("SalaryStructureGrid", new List<SalaryStructureGrid>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Internal Server Error: " + ex.Message;
                return View("SalaryStructureGrid", new List<SalaryStructureGrid>());
            }
        }



        [HttpGet("SalaryStructure/GetSalaryStructure")]
        public async Task<IActionResult> GetSalaryStructure(string salarystructure_hdr_id)
        {
            try
            {

                // Decrypt the SalaryStructure_Hdr_Id
                int? salaryStructureId = null;
                if (!string.IsNullOrEmpty(salarystructure_hdr_id))
                {
                    try
                    {
                        string decryptedIdStr = SingleEncryptionHelper.Decrypt(salarystructure_hdr_id);
                        if (int.TryParse(decryptedIdStr, out int parsedId))
                        {
                            salaryStructureId = parsedId;
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Invalid encrypted Salary Structure ID.";
                        return View("SalaryStructure", new SalaryStructureDTO());
                    }
                }

                // Construct API URL
                string queryParams = $"/{salaryStructureId ?? 0}";
                string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetSalaryStructureByIdUrl}{queryParams}";

                // Call the API to fetch the Salary Structure
                var apiResponse = await _masterServiceHelper.GetAllApiResponseByIdAsync<SalaryStructureDTO>(apiUrl);



                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    Console.WriteLine($"SalaryFrequency: {apiResponse.Result.SalaryFrequency}");
                    return View("GetSalaryStructure", apiResponse.Result);
                }


                // Handle case where no data is returned
                TempData["Error"] = apiResponse?.Message ?? "Failed to load Salary Structure data.";
                return View("GetSalaryStructure", new SalaryStructureDTO());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Internal Server Error: " + ex.Message;
                return View("GetSalaryStructure", new SalaryStructureDTO());
            }
        }

        [HttpGet("SalaryStructure/GetSalaryStructureSimulatorOld")]
        public async Task<IActionResult> GetSalaryStructureSimulatorOld(string salarystructure_hdr_id)
        {
            try
            {
                // Decrypt the SalaryStructure_Hdr_Id
                int? salaryStructureId = null;
                if (!string.IsNullOrEmpty(salarystructure_hdr_id))
                {
                    try
                    {
                        string decryptedIdStr = SingleEncryptionHelper.Decrypt(salarystructure_hdr_id);
                        if (int.TryParse(decryptedIdStr, out int parsedId))
                        {
                            salaryStructureId = parsedId;
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Invalid encrypted Salary Structure ID.";
                        return View("SalaryStructureSimulator", new SalaryStructureDTO());
                    }
                }

                // Construct API URL
                string queryParams = $"/{salaryStructureId ?? 0}";
                string apiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.GetSalaryStructureByIdUrl}{queryParams}";

                // Call the API to fetch the Salary Structure
                var apiResponse = await _masterServiceHelper.GetAllApiResponseByIdAsync<SalaryStructureDTO>(apiUrl);



                if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
                {
                    Console.WriteLine($"SalaryFrequency: {apiResponse.Result.SalaryFrequency}");
                    return View("SalaryStructureSimulator", apiResponse.Result);
                }


                // Handle case where no data is returned
                TempData["Error"] = apiResponse?.Message ?? "Failed to load Salary Structure data.";
                return View("SalaryStructureSimulator", new SalaryStructureDTO());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Internal Server Error: " + ex.Message;
                return View("SalaryStructureSimulator", new SalaryStructureDTO());
            }
        }

        [HttpGet("SalaryStructure/GetSalaryStructureSession")]
        public IActionResult GetSalaryStructureSession()
        {
            // Load from session
            var salaryStructure = SessionHelper.GetSessionObject<SalaryStructureDTO>(HttpContext, "SalaryStructureSession") ?? new SalaryStructureDTO();
            return View("GetSalaryStructure", salaryStructure);
        }

        [HttpGet("SalaryStructure/GetSalaryStructureSimulator")]
        public IActionResult GetSalaryStructureSimulator()
        {
            // Load from session
            var salaryStructure = SessionHelper.GetSessionObject<SalaryStructureDTO>(HttpContext, "SalaryStructureSession") ?? new SalaryStructureDTO();
            return View("SalaryStructureSimulator", salaryStructure);
        }


        [HttpPost]
        public IActionResult SaveSalaryStructureSession([FromBody] SalaryStructureDTO salaryStructure)
        {
            if (salaryStructure == null)
                return Json(new { success = false, message = "Invalid data." });

            // Save to session
            SessionHelper.SetSessionObject(HttpContext, "SalaryStructureSession", salaryStructure);
            return Json(new { success = true });
        }


        [HttpPost("SalaryStructure/AddSalaryStructure")]
        public async Task<IActionResult> AddSalaryStructure([FromBody] SalaryStructureDTO salaryStructure)
        {
            try
            {

                // Validate input
                if (salaryStructure == null || salaryStructure.SalaryStructureDetails == null || !salaryStructure.SalaryStructureDetails.Any())
                {
                    return Json(new { success = false, message = "Missing required fields." });
                }

                // Set CreatedBy and UpdatedBy fields from session
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                int userId = int.TryParse(sessionData?.UserId, out var parsedUserId) ? parsedUserId : 0;
                salaryStructure.CreatedBy = userId;
                salaryStructure.UpdatedBy = userId;

                var Company_Id = SessionCompanyId;
                salaryStructure.Company_Id = Company_Id;

                // Define API URL
                string apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostSalaryStructureUrl;

                // Generate API Key
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                // Call the API to add the Salary Structure
                var apiResponse = await _masterServiceHelper.PostCommonWithKeyAsync<SalaryStructureDTO, SalaryStructureDTO>(apiUrl, salaryStructure, apiKey);

                // Handle response
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = "Salary Structure added successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost("SalaryStructure/CalculateSalaryStructure1")]
        public async Task<IActionResult> CalculateSalaryStructure1([FromBody] SalarySimulatorDTO salaryStructure)
        {
            try
            {

                // Validate input
                if (salaryStructure == null || salaryStructure.SalarySimulatorDetails == null || !salaryStructure.SalarySimulatorDetails.Any())
                {
                    return Json(new { success = false, message = "Missing required fields." });
                }

                // Set CreatedBy and UpdatedBy fields from session
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                int userId = int.TryParse(sessionData?.UserId, out var parsedUserId) ? parsedUserId : 0;
                salaryStructure.CreatedBy = userId;
                salaryStructure.UpdatedBy = userId;

                var Company_Id = SessionCompanyId;

                // Define API URL
                string apiUrl = _apiSettings.PayrollMasterServiceEndpoints.CalculateSalaryStructureUrl;

                // Generate API Key
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                // Call the API to add the Salary Structure
                var apiResponse = await _masterServiceHelper.PostCommonWithKeyAsync<SalarySimulatorDTO, SalarySimulatorDTO>(apiUrl, salaryStructure, apiKey);

                // Handle response
                if (apiResponse.IsSuccess)
                {
                    return Json(new { success = true, message = "Salary Structure added successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost("SalaryStructure/CalculateSalaryStructure")]
        public async Task<IActionResult> CalculateSalaryStructure([FromBody] SalaryStructureDTO salaryStructure)
        {
            try
            {
                // Map SalaryStructureDTO to SalarySimulatorDTO
                SalarySimulatorDTO salarySimulator = new SalarySimulatorDTO
                {

                    ctc = salaryStructure.SimulatedAmount,
                    payrollMonth = 1,
                    SalarySimulatorDetails = salaryStructure.SalaryStructureDetails?
                        .Select(d => new SalarySimulatorDetailDTO
                        {
                            SalaryComponentId = d.EarningDeductionID,
                            PayComponentType = d.EarningDeductionType,
                            PayComponentName = d.EarningDeductionName,
                            CalculationType = d.CalculationType,
                            ComponentSequence = d.ComponentSequence,
                            Formula_ID = d.Formula_ID,
                            Formula = d.Formula_Computation

                        }).ToList()
                };

                if (salarySimulator == null || salarySimulator.SalarySimulatorDetails == null || !salarySimulator.SalarySimulatorDetails.Any())
                {
                    return Json(new { success = false, message = "Missing required fields." });
                }

                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                int userId = int.TryParse(sessionData?.UserId, out var parsedUserId) ? parsedUserId : 0;
                salarySimulator.CreatedBy = userId;
                salarySimulator.UpdatedBy = userId;

                string apiUrl = _apiSettings.PayrollMasterServiceEndpoints.CalculateSalaryStructureUrl;
                var apiKey = await _userServiceHelper.GenerateApiKeyAsync();

                var apiResponse = await _masterServiceHelper.PostSalarySimulatorWithKeyAsync<SalarySimulatorDTO>(apiUrl, salarySimulator, apiKey);

                if (apiResponse.IsSuccess)
                {
                    // return Json(new { success = true, message = "Salary Structure calculated successfully!", data = apiResponse.Data });
                    // Replace this line in CalculateSalaryStructure action:

                    // With this for better debugging and correct JSON output:
                    return Json(new
                    {
                        success = apiResponse.IsSuccess,
                        message = apiResponse.Message ?? "Salary Structure calculated successfully!",
                        data = apiResponse.Result ?? apiResponse.JsonResponse,
                        returnCount = apiResponse.returnCount
                    });
                }
                else
                {
                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public async Task<IActionResult> DeleteSalaryStructure([FromBody] SalaryStructure model)
        {


            // Set UpdatedBy from session
            model.UpdatedBy = SessionUserId;
            model.MinSalary = model.MinSalary ?? 0;
            model.MaxSalary = model.MaxSalary ?? 0;

            model.SalaryStructure_Hdr_Id = model.SalaryStructure_Hdr_Id;
            // Construct API URL
            var deleteApiUrl = $"{_apiSettings.PayrollMasterServiceEndpoints.DeleteSalaryStructureUrl}/{model.SalaryStructure_Hdr_Id}";

            var apikey = await _userServiceHelper.GenerateApiKeyAsync();
            // Call the common delete method (now sending the request body)
            var deleteResponse = await _masterServiceHelper.DeleteCommonAsync<SalaryStructure, SalaryStructure>(deleteApiUrl, model);

            if (deleteResponse != null && deleteResponse.IsSuccess)
            {
                return Json(new { success = true, message = deleteResponse.Message });
            }
            else
            {
                return Json(new { success = false, message = deleteResponse.Message ?? "Failed to delete the salary structure." });
            }


        }


        //[HttpGet("SalaryStructure/GetSalaryStructure1")]
        //public async Task<IActionResult> GetSalaryStructure1(string SalaryStructure_Hdr_Id)
        //{
        //    int? decryptedSalaryStructure_Hdr_Id = null;
        //    if (!string.IsNullOrEmpty(SalaryStructure_Hdr_Id))
        //    {
        //        try
        //        {
        //            string decryptedIdStr = SingleEncryptionHelper.Decrypt(SalaryStructure_Hdr_Id);
        //            if (int.TryParse(decryptedIdStr, out int parsedSalaryStructure_Hdr_Id))
        //            {
        //                decryptedSalaryStructure_Hdr_Id = parsedSalaryStructure_Hdr_Id;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // return BadRequest("Invalid encrypted company ID");
        //        }
        //    }

        //    try
        //    {

        //        string queryParams = $"?SalaryStructure_Hdr_Id={decryptedSalaryStructure_Hdr_Id ?? 0}"; // Used these to allow these parameter for CLMS parameter

        //        string apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetSalaryStructureUrl}";///{companyId}/{locationId}/{serviceId}";
        //       // var apiResponse = await _masterServiceHelper.GetAllRecordsAsync<SalaryStructure>($"{apiUrl}{ queryParams}");
        //        var apiResponse = await _masterServiceHelper.GetAllApiResponseByIdAsync<SalaryStructureDTO>($"{apiUrl}{queryParams}");

        //        //var acJson = JsonConvert.SerializeObject(apiResponse.Result);
        //        //Console.WriteLine(acJson);


        //        if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
        //        {
        //            return View("GetSalaryStructure", apiResponse.Result); //  returns full view
        //        }

        //        TempData["Error"] = apiResponse?.Message ?? "Failed to load data.";
        //        return View("GetSalaryStructure", new SalaryStructureDTO
        //        {
        //            Config = new SalaryStructure(),// <-- initialize the nested object
        //            Levels = new List<ApprovalLevel>(),
        //            Details = new List<ApprovalDetail>()
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = "Internal Server Error: " + ex.Message;
        //        return View("GetSalaryStructure", new SalaryStructureDTO
        //        {
        //            Config = new SalaryStructure(),// <-- initialize the nested object
        //            Levels = new List<ApprovalLevel>(),
        //            Details = new List<ApprovalDetail>()
        //        });
        //    }
        //}

        //[HttpPost("SalaryStructure/AddSalaryStructure")]
        //public async Task<IActionResult> AddSalaryStructure([FromBody] SalaryStructureDTO SalaryStructure)
        //{

        //    // return Json(new { success = false, message = "" });
        //    var apiKey = await _userServiceHelper.GenerateApiKeyAsync(); // Get API Key
        //    try
        //    {
        //        // Set CreatedBy field
        //        var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
        //        int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
        //        SalaryStructure.Config.CreatedBy = userId;
        //        SalaryStructure.Config.UpdatedBy = userId;


        //        if (SalaryStructure == null ||
        //     SalaryStructure.Config == null ||
        //      !SalaryStructure.Levels.Any() ||
        //         !SalaryStructure.Details.Any())
        //        {
        //            return Json(new { success = false, message = "Missing required fields." });
        //        }


        //        // Define API URL
        //        var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostSalaryStructureUrl;

        //// apiUrl = "http://localhost:5265/api/SalaryStructureApi/postsalarystructure";
        //// Call the generic PostAsync method to post SalaryStructure data
        //var apiResponse = await _masterServiceHelper
        //                            .PostCommonWithKeyAsync<SalaryStructureDTO, SalaryStructureDTO>(apiUrl, SalaryStructure, apiKey);

        //        // Handle response
        //        if (apiResponse.IsSuccess)
        //        {
        //            return Json(new { success = true, message = "SalaryStructure details added successfully!" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = apiResponse.Message });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception if necessary
        //        return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
        //    }
        //}

    }
}

