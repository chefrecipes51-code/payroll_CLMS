using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.DAL.Interface;
using System.Net;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.APIKeyManagement.Service;


namespace PayrollTransactionService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ContractorDetailsController : ControllerBase
    {
        private readonly IContractordetailsRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        public ContractorDetailsController(IContractordetailsRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }
        #region Contractor Details Crud APIs Functionality

        //[HttpGet("getallContractorMaster")]
        //public async Task<IActionResult> GetAllContractorMaster()
        //{
        //    var apiResponse = new ApiResponseModel<IEnumerable<ContractorDetails>>();
        //    // Fetching data from the repository by executing the stored procedure
        //    var ContractorMasters = await _repository.GetAllAsync(DbConstants.GetallContractorDetails);

        //    // Check if data exists
        //    if (ContractorMasters != null && ContractorMasters.Any())
        //    {
        //        apiResponse.IsSuccess = true;
        //        apiResponse.Result = ContractorMasters;
        //        apiResponse.StatusCode = (int)HttpStatusCode.OK;
        //        return Ok(apiResponse);
        //    }
        //    else
        //    {
        //        // Handle the case where no data is returned
        //        apiResponse.IsSuccess = false;
        //        apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
        //        return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
        //    }
        //}

        [HttpGet("getallcontractor")]
        public async Task<IActionResult> GetAllContractorMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int? contractor_ID,
      int company_ID,
      int? correspondance_ID)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<ContractorDetails>>();
            // Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            int value = contractor_ID ?? 0;

            var contractorDetails = await _repository.GetAllContractorAsync(DbConstants.GetallContractorDetails, new { Contractor_ID = value, Company_ID = company_ID, Correspondance_ID = correspondance_ID });

            if (contractorDetails != null && contractorDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = contractorDetails;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
            }
        }

        /// <summary>
        /// Developer Name: Foram Patel
        /// Message Detail: API to retrieve Area details based on the provided  Contractor ID. 
        /// This method fetches data from the repository and returns the  area detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 23-05-2025
        /// Change Date: 23-05-2025
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the  area to retrieve</param>
        /// <returns>Returns an API response with area details or an error message.</returns>
        [HttpGet("getContractorMasterbyid/{id}")]
        public async Task<IActionResult> GetContractorMasterById(int id)
        {
            ApiResponseModel<IEnumerable<ContractorDetails>> apiResponse = new ApiResponseModel<IEnumerable<ContractorDetails>>();

            var ContractorMaster = await _repository.GetContractorDetailsByCompanyIdAsync(DbConstants.GetallContractorDetails, new { Company_ID = id });
            if (ContractorMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = ContractorMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        [HttpGet("GetContractorProfileByCompanyIdAsync")]
        public async Task<IActionResult> GetContractorProfileByCompanyIdAsync([FromHeader(Name = "X-API-KEY")] string apiKey, int contractorId, int id)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<Contractorprofile>>();
            // Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            var ContractorMaster = await _repository.GetContractorProfileByCompanyIdAsync(DbConstants.GetallContractorDetails, new { Contractor_ID = contractorId, Company_ID = id });
            if (ContractorMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = ContractorMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        #endregion
    }
}
