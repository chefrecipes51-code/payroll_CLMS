using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventAuthMappingApiController : ControllerBase
    {
        private readonly IEventAuthMappingRepository _repository;
        public EventAuthMappingApiController(IEventAuthMappingRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Developer Name :- Chirag Gurjar
        /// Message Detail :- API to add Event Auth transaction Setup 
        /// Created Date   :- 19-Nov-2024
        /// Change Date    :- 
        /// Change Detail  :- 
        /// </summary>
        /// <param name="id">The ID of the role to retrieve</param>
        /// <returns>Returns an API response with Event auth transaction details or an error message.</returns>  
        [HttpPost("postevventauthMapping")]
        public async Task<IActionResult> PostEventAuthMapping([FromBody] EventAuthSetUp eventAuthSetUp)
        {
            // Validate the incoming request
            if (eventAuthSetUp == null  || !eventAuthSetUp.eventAuthDetails.Any())
            {
                return BadRequest("Invalid request: Missing required Event Auth Mapping details.");
            }

            try
            {
                var result = await _repository.AddNewAsync(DbConstants.AddEditEventAuthSetUp, eventAuthSetUp);
                if (result != null)
                {
                    return Ok(new { message = "Event Authentication successfully added.", data = result });
                }

                return BadRequest("Failed to add Event Authentication .");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while processing your request.");
            }
        }

        /// <summary>
        /// Developer Name :- Chirag Gurjar
        /// Message Detail :- API to Get all Event Auth transaction Setup data by primary key id
        /// Created Date   :- 19-Nov-2024
        /// Change Date    :- 
        /// Change Detail  :- 
        /// </summary>
        /// <param name="id">The ID of the role to retrieve</param>
        /// <returns>Returns an API response with Event auth transaction details or an error message.</returns>  
        [HttpGet("geteventauthsetupfilters")]
        public async Task<IActionResult> GetEventAuthSetUpFilters([FromBody] EventAuthFilter eventAuthFilter)
        {
            ApiResponseModel<IEnumerable<EventAuthSelect>> apiResponse = new ApiResponseModel<IEnumerable<EventAuthSelect>>();

            var EventAuthSelect = await _repository.GetByFilterAttributesAsync(DbConstants.GetEventAuthSetUp
                , new { Event_Id = eventAuthFilter.Event_Id, Company_Id = eventAuthFilter.Company_Id, Event_Auth_Id = eventAuthFilter.Event_Auth_Id, Module_Id = eventAuthFilter.Module_Id,    Roll_Id = eventAuthFilter.Roll_Id });
            if (EventAuthSelect == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = EventAuthSelect;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        /// Developer Name :- Chirag Gurjar
        /// Message Detail :- API to Get all Event Auth transaction Setup data
        /// Created Date   :- 19-Nov-2024
        /// Change Date    :- 
        /// Change Detail  :- 
        /// </summary>
        /// <param name="id">The ID of the role to retrieve</param>
        /// <returns>Returns an API response with Event auth transaction details or an error message.</returns>  
        [HttpGet("geteventauthsetup")]
        public async Task<IActionResult> GetEventAuthSetUp()
        {
            ApiResponseModel<IEnumerable<EventAuthSelect>> apiResponse = new ApiResponseModel<IEnumerable<EventAuthSelect>>();
            var EventAuthSelect = await _repository.GetAllAsync(DbConstants.GetEventAuthSetUp);
            if (EventAuthSelect == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = EventAuthSelect;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

    }
}