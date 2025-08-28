using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using System.Net;
using UserService.BAL.Models;
using UserService.BAL.Requests;
using UserService.DAL.Interface;

namespace UserService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CountryMasterApiController : ControllerBase
    {
        private readonly ICountryMasterRepository _repository;
        public CountryMasterApiController(ICountryMasterRepository repository) 
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #region Counrty Functionality

        [HttpGet("getallcountries")]
        public async Task<IActionResult> GetAllCountries()
        {
            ApiResponseModel<IEnumerable<CountryMaster>> apiResponse = new ApiResponseModel<IEnumerable<CountryMaster>>();
            try
            {
                var countries = await _repository.GetAllAsync(DbConstants.GetAllCountries);
                if (countries != null)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = countries;
                    apiResponse.StatusCode = (int)HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = MessageConstants.DataNotFound;
                    apiResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    return NotFound(apiResponse);
                }
            }
            catch
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = MessageConstants.TechnicalIssue;
                apiResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, apiResponse);
            }
        }
        #endregion
    }
}
