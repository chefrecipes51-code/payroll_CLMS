using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CompanyLocationApiController : ControllerBase
    {
        private readonly ICompanyLocationRepository _repository;

        public CompanyLocationApiController(ICompanyLocationRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("companylocationmap")]
        public async Task<IActionResult> GetCompanyLocationMap(
    [FromQuery] int? companyId,
    [FromQuery] int? userId)
    //[FromQuery] int? countryId,
    //[FromQuery] int? stateId,
    //[FromQuery] int? cityId,
    //[FromQuery] int? locationId,
    //[FromQuery] bool? isActive)
        {
            var parameters = new
            {
                Company_Id = companyId,
                Country_Id = 0,
                State_id = 0,
                City_id = 0,
                Location_Id = 0,
                IsActive = true,
                User_Id = userId
            };

            ApiResponseModel<CompanyLocationMapDto> apiResponse = new ApiResponseModel<CompanyLocationMapDto>();

            try
            {
                var model = await _repository.GetCompanyLocationMapAsync(DbConstants.GetCompanyLocationDtoMaster, parameters);

                if (model == null || (!model.Countries.Any() && !model.States.Any() && !model.Cities.Any() && !model.Locations.Any()))
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                    apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                    return NotFound(apiResponse);
                }

                apiResponse.IsSuccess = true;
                apiResponse.Result = model;
                apiResponse.Message = ApiResponseMessageConstant.GetRecord;
                apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                apiResponse.StatusCode = ApiResponseStatusConstant.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }

    }
}
