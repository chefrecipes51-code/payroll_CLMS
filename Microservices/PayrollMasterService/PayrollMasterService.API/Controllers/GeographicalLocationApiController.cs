using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System.ComponentModel.Design;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class GeographicalLocationApiController : ControllerBase
    {
        private readonly IGeographicalLocationRepository _repository;
        public GeographicalLocationApiController(IGeographicalLocationRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve State details based on the provided  Country ID. 
        /// This method fetches data from the repository and returns the  state detail if found, 
        /// otherwise returns a not found response.
        /// Created Date:20-Jan-2024
        /// Change Date:20-Jan-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the state to retrieve</param>
        /// <returns>Returns an API response with state details or an error message.</returns>
        [HttpGet("getallstatemaster/{id}")]
        public async Task<IActionResult> GetAllStateMaster(int id)
        {
            ApiResponseModel<IEnumerable<StateMaster>> apiResponse = new ApiResponseModel<IEnumerable<StateMaster>>();
            var parameters = new
            {
                Country_Id = id,
                State_ID = 0,
                IsActive = true
            };
            var stateMasterList = await _repository.GetAllStateAsync(DbConstants.GetAllStateMaster, parameters);

            if (stateMasterList == null || !stateMasterList.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = stateMasterList;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve State details based on the provided  Country ID. 
        /// This method fetches data from the repository and returns the  state detail if found, 
        /// otherwise returns a not found response.
        /// Created Date:20-Jan-2024
        /// Change Date:20-Jan-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the state to retrieve</param>
        /// <returns>Returns an API response with state details or an error message.</returns>
        [HttpGet("getallcitymaster/{id}")]
        public async Task<IActionResult> GetAllCityMaster(int id)
        {
            ApiResponseModel<IEnumerable<CityMaster>> apiResponse = new ApiResponseModel<IEnumerable<CityMaster>>();
            var parameters = new
            {
                State_ID = id,
                IsActive = true
            };
            var cityMasterList = await _repository.GetAllCityAsync(DbConstants.GetAllCityMaster, parameters);

            if (cityMasterList == null || !cityMasterList.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = cityMasterList;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve City wise Location details based on the provided  City ID. 
        /// This method fetches data from the repository and returns the  state detail if found, 
        /// otherwise returns a not found response.
        /// Created Date:20-Jan-2024
        /// Change Date:20-Jan-2024
        /// Change Detail: No changes yet
        /// </summary>
        /// <param name="id">The ID of the Location to retrieve</param>
        /// <returns>Returns an API response with state details or an error message.</returns>
        [HttpGet("getallcitywiselocationmaster/{id}")]
        public async Task<IActionResult> GetAllCityWiseLocationMaster(int id)
        {
            ApiResponseModel<IEnumerable<LocationMaster>> apiResponse = new ApiResponseModel<IEnumerable<LocationMaster>>();
            var parameters = new
            {
                City_ID = id,
                IsActive = true
            };
            var cityLocationMasterList = await _repository.GetAllCityWiseLocationAsync(DbConstants.GetAllCityWiseLocationMaster, parameters);

            if (cityLocationMasterList == null || !cityLocationMasterList.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = cityLocationMasterList;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
    }
}
