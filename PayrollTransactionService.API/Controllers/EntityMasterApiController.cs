using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.DAL.Interface;
using System.Net;

namespace PayrollTransactionService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class EntityMasterApiController : ControllerBase
    {
        private readonly IEntityMasterRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        public EntityMasterApiController(IEntityMasterRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all wage grade details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 13-Sep-2024
        ///  Last Modified  :- 13-Sep-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with wage grade details or an appropriate message</returns>
        [HttpGet("getallcontractormaster")]
        public async Task<IActionResult> GetAllContractorMaster(
             [FromHeader(Name = "X-API-KEY")] string apiKey,
             int? contractor_ID,
             int company_ID,
             int? correspondance_ID)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<ContractorMaster>>();

            // Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidApiKey;
                return Unauthorized(apiResponse);
            }
            int value = contractor_ID ?? 0;

            var contractorDetails = await _repository.GetAllContratcorAsync(DbConstants.GetAllContractorDetail, new { Contractor_ID = value, Company_ID = company_ID, Correspondance_ID = correspondance_ID });

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
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of map user location details based on the provided organization data.
        ///  Created Date   :- 06-Nov-2024
        ///  Change Date    :- 20-Nov-2024
        ///  Change detail  :- Added a bulk insert functionality using a User-Defined Table (UDT).
        /// </summary>
        /// <param name="mapEntityGradeMaster"> map user location detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("updatemapgradeentity")]
        public async Task<IActionResult> UpdateMapGradeEntity([FromHeader(Name = "X-API-KEY")] string apiKey, [FromBody] MapEntityGradeMaster mapEntityGradeMaster)
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
            if (mapEntityGradeMaster == null || mapEntityGradeMaster.MapEntityGrade == null || !mapEntityGradeMaster.MapEntityGrade.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddAsync(DbConstants.UpdateMapGradeEntity, mapEntityGradeMaster);
            apiResponse.IsSuccess = mapEntityGradeMaster.MessageType == 1;
            apiResponse.Message = mapEntityGradeMaster.StatusMessage;
            apiResponse.MessageType = mapEntityGradeMaster.MessageType;
            apiResponse.StatusCode = mapEntityGradeMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return mapEntityGradeMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- This API retrieves all pay component details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 30-April-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with pay component details or an appropriate message</returns>
        [HttpGet("getallentitymaster")]
        public async Task<IActionResult> GetAllEntityMaster([FromHeader(Name = "X-API-KEY")] string apiKey, int entityId)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<EntityMaster>>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Fetching data from the repository by executing the stored procedure
            var entityMasters = await _repository.GetAllEntityAsync(DbConstants.GetAllEntityDetailUrl, new { Entity_ID = entityId });

            // Check if data exists
            if (entityMasters != null && entityMasters.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = entityMasters;
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

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- Updates an existing entityCompliance record in the database based on the provided ID and data.
        ///                       Validates the input, and if successful, updates the record and returns a success message.
        ///  Created Date   :- 29-May-2025
        ///  Last Updated   :- 29-May-2025
        /// </summary>
        /// <param name="id">The ID of the entityCompliance record to update.</param>
        /// <param name="entityCompliance">The entityCompliance object containing the updated data.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updateentitycompliance/{id}")]
        public async Task<IActionResult> PutEntityCompliance([FromHeader(Name = "X-API-KEY")] string apiKey, int id, [FromBody] EntityCompliance entityCompliance)
        {
            ApiResponseModel<EntityCompliance> apiResponse = new ApiResponseModel<EntityCompliance>();
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Invalid API Key.";
                apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
                return Unauthorized(apiResponse);
            }
            // Check if the provided areaMaster is null or the id doesn't match the Area_Id
            if (entityCompliance == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return Ok(apiResponse);
            }
            entityCompliance.Entity_ID = id;
            await _repository.EntityComplianceAsync(DbConstants.UpdateEntityComplianceDetailUrl, entityCompliance);// Call the UpdateAsync method in the repository
            apiResponse.IsSuccess = entityCompliance.MessageType == 1;
            apiResponse.Message = entityCompliance.StatusMessage;
            apiResponse.MessageType = entityCompliance.MessageType;
            apiResponse.StatusCode = entityCompliance.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return entityCompliance.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Abhishek Yadav
        ///  Message detail :- This API retrieves all Work order details by Contract_Code from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 27-05-2025
        ///  Last Modified  :- None
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with workorder details or an appropriate message</returns>
        [HttpGet("getWorkOrderDataByContractor")]
        public async Task<IActionResult> GetAllWorkOrderMaster(
             [FromHeader(Name = "X-API-KEY")] string apiKey,
             string ContractorCode)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<WorkOrderMaster>>();

            // Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidApiKey;
                return Unauthorized(apiResponse);
            }
            string value = !string.IsNullOrEmpty(ContractorCode) ? ContractorCode : "0";


            var workorderDetails = await _repository.GetAllWorkorderAsync(DbConstants.GetAllWorkdOrderDetail, new { ContractorCode = value });

            if (workorderDetails != null && workorderDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = workorderDetails;
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
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API retrieves all data validation details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 09-June-2025
        ///  Last Modified  :- 09-June-2025
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with data validation details or an appropriate message</returns>
        [HttpGet("getallentitydatavalidation")]
        public async Task<IActionResult> GetAllEntityDataValidation(
             int moduleId)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<EntityDataValidation>>();

            var details = await _repository.GetAllEntityDataValidationAsync(DbConstants.GetAllEntityDataValidation, new { module_ID = moduleId });

            if (details != null && details.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = details;
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
        ///  Developer Name :- HArshida
        ///  Message detail :- This API retrieves CONTRACTOR WITH WORK order details by Contract_Code from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 13-06-2025
        ///  Last Modified  :- None
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with workorder details or an appropriate message</returns>
         [HttpGet("getContractorWithWorkOrder")]
        public async Task<IActionResult> GetContractorWorkOrder(
             [FromHeader(Name = "X-API-KEY")] string apiKey,
              int company_ID)
        {
            var apiResponse = new ApiResponseModel<IEnumerable<ContractorWorkOrderRequest>>();

            // Validate API key
            var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            if (!isValid)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidApiKey;
                return Unauthorized(apiResponse);
            }
          
            var contractorWorkorderDetails = await _repository.GetContractorWorkorderAsync(DbConstants.GetContractorWorkdOrder, new { Company_Id = company_ID });

            if (contractorWorkorderDetails != null && contractorWorkorderDetails.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = contractorWorkorderDetails;
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
    }
}
