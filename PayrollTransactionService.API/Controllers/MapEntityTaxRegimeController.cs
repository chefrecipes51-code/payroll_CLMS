using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.DAL.Interface;
using System.Net;

namespace PayrollTransactionService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class MapEntityTaxRegimeController : ControllerBase
    {
        private readonly IMapEntityTaxRegimeRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        public MapEntityTaxRegimeController(IMapEntityTaxRegimeRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all pay component details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 08-April-2025
        ///  Last Modified  :- 08-April-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with pay component details or an appropriate message</returns>
        [HttpPost("getallentityfilterrequest")]
        public async Task<IActionResult> GetAllEntityFilterRequest(
         [FromHeader(Name = "X-API-KEY")] string apiKey,
         [FromBody] EntityFilterRequest request)
        {
            var apiResponse = new ApiResponseModel<EntityFilterResponse>();

            // Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }

            // Call repository method
            var result = await _repository.GetAllEntityFiltersAsync(DbConstants.GetEntityFilterProcedure, request);
            // Check if at least one list is non-null and has items
            bool hasData = (result.Contractors?.Any() ?? false)
                        || (result.EntityCodes?.Any() ?? false)
                        || (result.EntityNames?.Any() ?? false)
                        || (result.ContractorEntities?.Any() ?? false)
                        || (result.GradeMapEntities?.Any() ?? false);

            if (hasData)
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = result;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Result = new EntityFilterResponse(); // Empty but structured
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
        }


        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of map user location details based on the provided organization data.
        ///  Created Date   :- 06-Nov-2024
        ///  Change Date    :- 20-Nov-2024
        ///  Change detail  :- Added a bulk insert functionality using a User-Defined Table (UDT).
        /// </summary>
        /// <param name="mapEntityTaxRegime"> map user location detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postmapentitytaxregime")]
        public async Task<IActionResult> PostMapEntityTaxRegime([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] MapEntityTaxRegime mapEntityTaxRegime)
        {
            var apiResponse = new ApiResponseModel<MapEntityTaxRegime>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            if (mapEntityTaxRegime == null || mapEntityTaxRegime.EntityTaxRegime == null || !mapEntityTaxRegime.EntityTaxRegime.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddAsync(DbConstants.AddMapEntityTaxRegime, mapEntityTaxRegime);
            apiResponse.IsSuccess = mapEntityTaxRegime.MessageType == 1;
            apiResponse.Message = mapEntityTaxRegime.StatusMessage;
            apiResponse.MessageType = mapEntityTaxRegime.MessageType;
            apiResponse.StatusCode = mapEntityTaxRegime.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return mapEntityTaxRegime.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all pay component details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 08-April-2025
        ///  Last Modified  :- 08-April-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with pay component details or an appropriate message</returns>
        [HttpGet("getallfinancialyear")]
        public async Task<IActionResult> GetAllFinacialYear([FromHeader(Name = "X-API-KEY")] string apiKey, int companyId)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<FinancialYearMaster>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Fetching data from the repository by executing the stored procedure
            var taxregimeMaster = await _repository.GetAllFinancialYearAsync(DbConstants.GetAllFinancialYearProcedure, new { Company_id = companyId });

            // Check if data exists
            if (taxregimeMaster != null && taxregimeMaster.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = taxregimeMaster;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            else
            {
                // Handle the case where no data is returned
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
            }
        }


    }
}
