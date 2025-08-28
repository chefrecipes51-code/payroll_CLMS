/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-82 , PAYROLL-106(Compile company entitiers in a single api call)     *
 *  Description:                                                                                    *
 *  This controller handles CRUD operations for CompanyMaster Compile entries.                      *
 *  It includes APIs to retrieve, create, update, and delete CompanyMaster                          *
 *  records using the repository pattern and stored procedures.                                     *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllCompanyMaster : Retrieves all CompanyMaster records.                                    *
 *  - GetCompanyMasterById: Retrieves a specific CompanyMaster record by ID.                        *
 *  - PostCompanyMasterMaster   : Adds a new CompanyMaster record.                                  *
 *  - PutCompanyMaster    : Updates an existing CompanyMaster record.                               *
 *  - DeleteCompanyMaster : Soft-deletes an CompanyMaster record.                                   *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 03-Oct-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;
using PayrollMasterService.DAL.Interface;
using System.Data;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CompanyMasterApiController : ControllerBase
    {
        private readonly ICompanyMasterRepository _repository;
        public CompanyMasterApiController(ICompanyMasterRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #region Company Master Old
        #region Company Master Crud APIs Functionality
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all Company details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 03-Oct-2024
        ///  Last Modified  :- 03-Oct-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Company details or an appropriate message</returns>
        [HttpGet("getallcompanymaster")]
        public async Task<IActionResult> GetAllCompanyMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<CompanyMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var companyMasters = await _repository.GetAllAsync(DbConstants.GetCompanyMaster);
            // Check if data exists
            if (companyMasters != null && companyMasters.Any())
            {
                // For each company master, fetch the related correspondence and statutory details
                foreach (var company in companyMasters)
                {
                    // Fetch company correspondence
                    var correspondence = await _repository.GetCompanyCorrespondanceByIdAsync(DbConstants.GetCompanyCorrespondanceById, new { Company_Id = company.Company_Id });

                    // Fetch company statutory details
                    var statutory = await _repository.GetCompanyStatutoryByIdAsync(DbConstants.GetCompanyStatutoryMasterById, new { Company_Id = company.Company_Id });

                    // Assign the fetched data to the company master object
                    company.CompanyCorrespondance = correspondence;
                    company.CompanyStatutory = statutory;
                }

                apiResponse.IsSuccess = true;
                apiResponse.Result = companyMasters;
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
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve Company details based on the provided  Company ID. 
        /// This method fetches data from the repository and returns the  company detail if found, 
        /// otherwise returns a not found response.
        /// Created Date: 03-Oct-2024
        /// Change Date: 23-Jan-2025
        /// Change Detail: CompanyCorrespondance and CompanyStatutory WILL return LIST SO Create new class CompanyProfile
        /// Change By: Harshida Parmar
        /// </summary>
        /// <param name="id">The ID of the Company to retrieve</param>
        /// <returns>Returns an API response with company details or an error message.</returns>
        [HttpGet("getcompanymasterbyid/{id}")]
        public async Task<IActionResult> GetCompanyMasterById(int id)
        {
            ///
            ///<summary> From CompanyMaster to CompanyProfile because we need list </summary>
            ///
            //ApiResponseModel<CompanyMaster> apiResponse = new ApiResponseModel<CompanyMaster>(); //Commented Because We need LIST
            ApiResponseModel<CompanyProfile> apiResponse = new ApiResponseModel<CompanyProfile>();
            var companyMaster = await _repository.GetByIdAsync(DbConstants.GetCompanyMasterById, new { Company_Id = id });
            if (companyMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            // Fetch the corresponding company correspondence data (BELOW METHOD WILL RETURN SINGLE RECORD ONLY)
            //var companyCorrespondance = await _repository.GetCompanyCorrespondanceByIdAsync(DbConstants.GetCompanyCorrespondanceById, new { Company_Id = id });

            // Fetch the corresponding company correspondence data
            var companyCorrespondance = await _repository.GetAllCompanyCorrespondancesByCompanyIdAsync(DbConstants.GetCompanyCorrespondanceById, new { Company_Id = id });

            // Fetch the corresponding company statutory data (BELOW METHOD WILL RETURN SINGLE RECORD ONLY)
            //var companyStatutory = await _repository.GetCompanyStatutoryByIdAsync(DbConstants.GetCompanyStatutoryMasterById, new { Company_Id = id });

            // Fetch the corresponding company statutory data
            var companyStatutory = await _repository.GetAllCompanyStatutoriesByCompanyIdAsync(DbConstants.GetCompanyStatutoryMasterById, new { Company_Id = id });

			// Assign the fetched correspondence and statutory data to the company master object
			//companyMaster.CompanyCorrespondance = companyCorrespondance;
			//companyMaster.CompanyStatutory = companyStatutory;

			#region Krunali Code For Testing Merge Here
			var subSidiaryMaster = await _repository.GetAllSubsidiaryMaster(DbConstants.GetCompanySubsidiaryMaster);
			#endregion

			#region Map From CompanyMaster To CompanyProfile [Added By Harshida 23-01-'25]
			var companyProfile = new CompanyProfile
            {
                Company_Id = companyMaster.Company_Id,
                CompanyType_ID = companyMaster.CompanyType_ID,
                Company_Code = companyMaster.Company_Code,
                Group_Id = companyMaster.Group_Id,
                CompanyName = companyMaster.CompanyName,
                CompanyPrintName = companyMaster.CompanyPrintName,
                IsParent = companyMaster.IsParent,
                CompanyShortName = companyMaster.CompanyShortName,
                ParentCompany_Id = companyMaster.ParentCompany_Id,
                CompanyLevel = companyMaster.CompanyLevel,
                Location_ID = companyMaster.Location_ID,
                Has_Subsidary = companyMaster.Has_Subsidary,
                StartDate = companyMaster.StartDate,
                EndDate = companyMaster.EndDate,
                CompanyCorrespondances = (List<CompanyCorrespondance>)companyCorrespondance,
                CompanyStatutories = (List<CompanyStatutory>)companyStatutory,
				SubsidiaryMasteries = (List<SubsidiaryMaster>)subSidiaryMaster, //Added Krunali Code Here 
			};
            #endregion
            apiResponse.IsSuccess = true;
            apiResponse.Result = companyProfile;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        [HttpGet("getcompanydemographicmasterbyid/{id}")]
        public async Task<IActionResult> GetCompanyDemographicDataById(int id)
        {           
            ApiResponseModel<CompanyMaster> apiResponse = new ApiResponseModel<CompanyMaster>();
            var companyMaster = await _repository.GetByIdAsync(DbConstants.GetCompanyMasterById, new { Company_Id = id });

            if (companyMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }           
            apiResponse.IsSuccess = true;
            apiResponse.Result = companyMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail :- This API handles the addition of company details based on the provided organization data.
        ///  Created Date   :- 03-Oct-2024
        ///  Change Date    :- 03-Oct-2024
        ///  Change detail  :- Not yet modified.
        /// </summary>
        /// <param name="companyMaster"> company detail to be added.</param>
        /// <returns>Returns a JSON response with the result of the operation.</returns>
        [HttpPost("postcompanymaster")]
        public async Task<IActionResult> PostCompanyMaster([FromBody] CompanyMaster companyMaster)
        {
            ApiResponseModel<CompanyMaster> apiResponse = new ApiResponseModel<CompanyMaster>();
            if (companyMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            // 1. Call AddOrUpdate for CompanyMaster
            await _repository.AddCompanyAsync(DbConstants.AddEditCompanyMaster, companyMaster, "CompanyMaster");
            if (companyMaster.MessageType != 1)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = companyMaster.StatusMessage;
                apiResponse.MessageType = companyMaster.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            // Ensure Company_Id is set for CompanyCorrespondence and CompanyStatutory
            var companyId = companyMaster.Company_Id; // Now the Company_Id is set correctly
            // 2. Call AddOrUpdate for CompanyCorrespondance
            if (companyMaster.CompanyCorrespondance != null)
            {
                companyMaster.CompanyCorrespondance.Company_Id = companyId; // Assign the Company_Id
                await _repository.AddCompanyAsync(DbConstants.AddEditCompanyCorrespondance, companyMaster.CompanyCorrespondance, "CompanyCorrespondance");
                if (companyMaster.CompanyCorrespondance.MessageType != 1)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = companyMaster.CompanyCorrespondance.StatusMessage;
                    apiResponse.MessageType = companyMaster.CompanyCorrespondance.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return BadRequest(apiResponse);
                }
            }

            // 3. Call AddOrUpdate for CompanyStatutory
            if (companyMaster.CompanyStatutory != null)
            {
                companyMaster.CompanyStatutory.Company_Id = companyId; // Assign the Company_Id
                await _repository.AddCompanyAsync(DbConstants.AddEditCompanyStatutoryMaster, companyMaster.CompanyStatutory, "CompanyStatutory");
                if (companyMaster.CompanyStatutory.MessageType != 1)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = companyMaster.CompanyStatutory.StatusMessage;
                    apiResponse.MessageType = companyMaster.CompanyStatutory.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return BadRequest(apiResponse);
                }
            }

            // If all steps succeeded
            apiResponse.IsSuccess = true;
            apiResponse.Message = companyMaster.StatusMessage;
            apiResponse.MessageType = companyMaster.MessageType;
            apiResponse.StatusCode = ApiResponseStatusConstant.Created;
            return StatusCode((int)HttpStatusCode.Created, apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the company detail based on the provided companyMaster and ID. 
        ///                    If the ID does not match or CompanyMaster is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 03-Oct-2024
        ///  Last Updated   :- 03-Oct-2024
        ///  Change Details :- Initial implementation.
        /// </summary>
        /// <param name="id">The ID of the  Company to update.</param>
        /// <param name="companyMaster">The  company detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpPut("updatecompanymaster/{id}")]        
        public async Task<IActionResult> PutCompanyMaster(int id, [FromBody] CompanyMaster companyMaster)
        {
            ApiResponseModel<CompanyMaster> apiResponse = new ApiResponseModel<CompanyMaster>();
            // Check if the provided companyMaster is null or the id doesn't match the Company_Id
            if (companyMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            // Retrieve the existing record based on the id
            var existingCompanyMaster = await _repository.GetByIdAsync(DbConstants.GetCompanyMasterById, new { Company_Id = id });
            if (existingCompanyMaster == null || existingCompanyMaster.Company_Id <= 0 || existingCompanyMaster.IsDeleted == true)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            // Preserve created by and created date details
            companyMaster.Company_Id = existingCompanyMaster.Company_Id;

            // Call the repository to update CompanyMaster first
            await _repository.UpdateCompanyAsync(DbConstants.AddEditCompanyMaster, companyMaster, "CompanyMaster");
            if (companyMaster.MessageType != 1)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = companyMaster.StatusMessage;
                apiResponse.MessageType = companyMaster.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            #region Code not in use
            //var companyId = companyMaster.Company_Id; // Set the Company_Id for related entities
            //var companyCorrespondance = await _repository.GetCompanyCorrespondanceByIdAsync(DbConstants.GetCompanyCorrespondanceById, new { Company_Id = id });
            //// Update CompanyCorrespondance if present
            //if (companyMaster.CompanyCorrespondance != null)
            //{
            //    companyMaster.CompanyCorrespondance.Company_Id = companyId;  // Assign Company_Id
            //    await _repository.UpdateCompanyAsync(DbConstants.AddEditCompanyCorrespondance, companyMaster.CompanyCorrespondance, "CompanyCorrespondance");

            //    if (companyMaster.CompanyCorrespondance.MessageType != 1)
            //    {
            //        apiResponse.IsSuccess = false;
            //        apiResponse.Message = companyMaster.CompanyCorrespondance.StatusMessage;
            //        apiResponse.MessageType = companyMaster.CompanyCorrespondance.MessageType;
            //        apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
            //        return BadRequest(apiResponse);
            //    }
            //}
            //var companyStatutory = await _repository.GetCompanyStatutoryByIdAsync(DbConstants.GetCompanyStatutoryMasterById, new { Company_Id = id });
            //// Update CompanyStatutory if present
            //if (companyMaster.CompanyStatutory != null)
            //{
            //    companyMaster.CompanyStatutory.Company_Id = companyId; // Assign Company_Id
            //    await _repository.UpdateCompanyAsync(DbConstants.AddEditCompanyStatutoryMaster, companyMaster.CompanyStatutory, "CompanyStatutory");
            //    if (companyMaster.CompanyStatutory.MessageType != 1)
            //    {
            //        apiResponse.IsSuccess = false;
            //        apiResponse.Message = companyMaster.CompanyStatutory.StatusMessage;
            //        apiResponse.MessageType = companyMaster.CompanyStatutory.MessageType;
            //        apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
            //        return BadRequest(apiResponse);
            //    }
            //}
            // If all updates are successful, return success response
            #endregion
            apiResponse.IsSuccess = true;
            apiResponse.Message = companyMaster.StatusMessage;
            apiResponse.MessageType = companyMaster.MessageType;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;

            return Ok(apiResponse);
        }
        #region Company Master Old Code
        //public async Task<IActionResult> PutCompanyMaster(int id, [FromBody] CompanyMaster companyMaster)
        //{
        //    ApiResponseModel<CompanyMaster> apiResponse = new ApiResponseModel<CompanyMaster>();
        //    // Check if the provided companyMaster is null or the id doesn't match the Company_Id
        //    if (companyMaster == null || id <= 0)
        //    {
        //        apiResponse.IsSuccess = false;
        //        apiResponse.Message = ApiResponseMessageConstant.InvalidData;
        //        apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
        //        return BadRequest(apiResponse);
        //    }
        //    // Retrieve the existing record based on the id
        //    var existingCompanyMaster = await _repository.GetByIdAsync(DbConstants.GetCompanyMasterById, new { Company_Id = id });
        //    if (existingCompanyMaster == null || existingCompanyMaster.Company_Id <= 0 || existingCompanyMaster.IsDeleted == true)
        //    {
        //        apiResponse.IsSuccess = false;
        //        apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
        //        apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
        //        return NotFound(apiResponse);
        //    }

        //    // Preserve created by and created date details
        //    companyMaster.Company_Id = existingCompanyMaster.Company_Id;

        //    // Call the repository to update CompanyMaster first
        //    await _repository.UpdateCompanyAsync(DbConstants.AddEditCompanyMaster, companyMaster, "CompanyMaster");
        //    if (companyMaster.MessageType != 1)
        //    {
        //        apiResponse.IsSuccess = false;
        //        apiResponse.Message = companyMaster.StatusMessage;
        //        apiResponse.MessageType = companyMaster.MessageType;
        //        apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
        //        return BadRequest(apiResponse);
        //    }
        //    var companyId = companyMaster.Company_Id; // Set the Company_Id for related entities
        //    var companyCorrespondance = await _repository.GetCompanyCorrespondanceByIdAsync(DbConstants.GetCompanyCorrespondanceById, new { Company_Id = id });
        //    // Update CompanyCorrespondance if present
        //    if (companyMaster.CompanyCorrespondance != null)
        //    {
        //        companyMaster.CompanyCorrespondance.Company_Id = companyId;  // Assign Company_Id
        //        await _repository.UpdateCompanyAsync(DbConstants.AddEditCompanyCorrespondance, companyMaster.CompanyCorrespondance, "CompanyCorrespondance");

        //        if (companyMaster.CompanyCorrespondance.MessageType != 1)
        //        {
        //            apiResponse.IsSuccess = false;
        //            apiResponse.Message = companyMaster.CompanyCorrespondance.StatusMessage;
        //            apiResponse.MessageType = companyMaster.CompanyCorrespondance.MessageType;
        //            apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
        //            return BadRequest(apiResponse);
        //        }
        //    }
        //    var companyStatutory = await _repository.GetCompanyStatutoryByIdAsync(DbConstants.GetCompanyStatutoryMasterById, new { Company_Id = id });
        //    // Update CompanyStatutory if present
        //    if (companyMaster.CompanyStatutory != null)
        //    {
        //        companyMaster.CompanyStatutory.Company_Id = companyId; // Assign Company_Id
        //        await _repository.UpdateCompanyAsync(DbConstants.AddEditCompanyStatutoryMaster, companyMaster.CompanyStatutory, "CompanyStatutory");
        //        if (companyMaster.CompanyStatutory.MessageType != 1)
        //        {
        //            apiResponse.IsSuccess = false;
        //            apiResponse.Message = companyMaster.CompanyStatutory.StatusMessage;
        //            apiResponse.MessageType = companyMaster.CompanyStatutory.MessageType;
        //            apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
        //            return BadRequest(apiResponse);
        //        }
        //    }
        //    // If all updates are successful, return success response
        //    apiResponse.IsSuccess = true;
        //    apiResponse.Message = companyMaster.StatusMessage;
        //    apiResponse.MessageType = companyMaster.MessageType;
        //    apiResponse.StatusCode = ApiResponseStatusConstant.Ok;

        //    return Ok(apiResponse);
        //}
        #endregion
        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- Updates the delete column based on the provided Company Master and ID. 
        ///                    If the ID does not match or Company Master  is null, returns a BadRequest. 
        ///                    If the record does not exist, returns a NotFound. 
        ///                    Upon successful update, returns the appropriate status and message.
        ///  Created Date   :- 03-Oct-2024
        ///  Last Updated   :- 03-Oct-2024
        ///  Change Details :- Initial implementation.
        /// </summary>
        /// <param name="id">The ID of the  Company to update.</param>
        /// <param name="companyMaster">The  Company detail to update with.</param>
        /// <returns>Returns an ApiResponseModel with a success or failure message.</returns>
        [HttpDelete("deletecompanymaster/{id}")]
        public async Task<IActionResult> DeleteCompanyMaster(int id, [FromBody] CompanyMaster companyMaster)
        {
            ApiResponseModel<CompanyMaster> apiResponse = new ApiResponseModel<CompanyMaster>();
            // Check if the provided Company is null or the id doesn't match the Company_Id
            if (companyMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            // Retrieve the existing record based on the id
            var existingcompanyMaster = await _repository.GetByIdAsync(DbConstants.GetCompanyMasterById, new { Company_Id = id });
            if (existingcompanyMaster == null || existingcompanyMaster.IsDeleted == true)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            companyMaster.Company_Id = existingcompanyMaster.Company_Id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteCompanyAsync(DbConstants.DeleteCompanyMaster, companyMaster, "CompanyMaster");
            if (companyMaster.MessageType != 1)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = companyMaster.StatusMessage;
                apiResponse.MessageType = companyMaster.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            var companyId = companyMaster.Company_Id; // Set the Company_Id for related entities
            // Update CompanyCorrespondance if present
            if (companyMaster.CompanyCorrespondance != null)
            {
                companyMaster.CompanyCorrespondance.Company_Id = companyId;  // Assign Company_Id
                await _repository.DeleteCompanyAsync(DbConstants.DeleteCompanyCorrespondance, companyMaster.CompanyCorrespondance, "CompanyCorrespondance");
                if (companyMaster.CompanyCorrespondance.MessageType != 1)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = companyMaster.CompanyCorrespondance.StatusMessage;
                    apiResponse.MessageType = companyMaster.CompanyCorrespondance.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return BadRequest(apiResponse);
                }
            }
            //var companyStatutory = await _repository.GetCompanyStatutoryByIdAsync(DbConstants.GetCompanyStatutoryMasterById, new { Company_Id = id });
            // Update CompanyStatutory if present
            if (companyMaster.CompanyStatutory != null)
            {
                companyMaster.CompanyStatutory.Company_Id = companyId; // Assign Company_Id
                                                                       // companyMaster.CompanyStatutory.StatutoryType_Name = companyMaster.CompanyStatutory.StatutoryType_Name ?? "DefaultStatutoryName";
                await _repository.DeleteCompanyAsync(DbConstants.DeleteCompanyStatutoryMaster, companyMaster.CompanyStatutory, "CompanyStatutory");

                if (companyMaster.CompanyStatutory.MessageType != 1)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = companyMaster.CompanyStatutory.StatusMessage;
                    apiResponse.MessageType = companyMaster.CompanyStatutory.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return BadRequest(apiResponse);
                }
            }
            // If all updates are successful, return success response
            apiResponse.IsSuccess = true;
            apiResponse.Message = companyMaster.StatusMessage;
            apiResponse.MessageType = companyMaster.MessageType;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;

            return Ok(apiResponse);
        }

        /// <summary>
        ///  Developer Name :- Priyanshi Jain
        ///  Message detail    :- This API retrieves all Company details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 23-Dec-2024
        ///  Last Modified  :- 23-Dec-2024
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Company details or an appropriate message</returns>
        [HttpGet("getallcompanymasterapi")]
        public async Task<IActionResult> GetAllCompanyMasterAPI()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<CompanyMaster>>();
            // Fetching data from the repository by executing the stored procedure
            var companyMasters = await _repository.GetAllAsync(DbConstants.GetCompanyMaster);
            // Check if data exists
            if (companyMasters != null && companyMasters.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = companyMasters;
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
        #endregion
        #endregion

        #region Added By Harshida 
        #region [Company Master CRUD NEW] Added By Harshida [17-02-'25]
        [HttpGet("getallcompany")]
        public async Task<IActionResult> GetAllCompany()
        {
            ApiResponseModel<List<CompanyProfile>> apiResponse = new ApiResponseModel<List<CompanyProfile>>();

            var companyMasterList = await _repository.GetAllAsync(DbConstants.GetCompanyMasterById);

            if (companyMasterList == null || !companyMasterList.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            // Manually map each record to CompanyProfile
            List<CompanyProfile> companyProfiles = companyMasterList.Select(company => new CompanyProfile
            {
                Company_Id = company.Company_Id,
                CompanyType_ID = company.CompanyType_ID,
                Company_Code = company.Company_Code,
                Group_Id = company.Group_Id,
                CompanyName = company.CompanyName,
                CompanyPrintName = company.CompanyPrintName,
                IsParent = company.IsParent,
                CompanyShortName = company.CompanyShortName,
                ParentCompany_Id = company.ParentCompany_Id,
                CompanyLevel = company.CompanyLevel,
                Location_ID = company.Location_ID,
                Has_Subsidary = company.Has_Subsidary,
                Currency_ID = company.Currency_ID
            }).ToList();

            apiResponse.IsSuccess = true;
            apiResponse.Result = companyProfiles;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        /// Developer By :- Harshida Parmar
        /// Start Date:- 14-04-25
        ///  Message detail    :- This API retrieves Company Financial Year Based on Company Id.     
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getcompanyfinYeardetailsbyid/{id}")]
        public async Task<IActionResult> GetCompanyFinYearDetailsById(int id)
        {
            ApiResponseModel<CompanyFinYear> apiResponse = new ApiResponseModel<CompanyFinYear>();
            var companyMaster = await _repository.GetByIdAsync(DbConstants.GetCompanyMasterById, new { Company_Id = id });
            if (companyMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
         
            #region Map From CompanyMaster To CompanyProfile [Added By Harshida 14-04-'25]
            var companyProfile = new CompanyFinYear
            {
                Company_Id = companyMaster.Company_Id,
                CompanyName = companyMaster.CompanyName,
                StartDate = companyMaster.StartDate,
                EndDate = companyMaster.EndDate
                
            };
            #endregion
            apiResponse.IsSuccess = true;
            apiResponse.Result = companyProfile;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        #endregion
        #region [Company Type] Added By Harshida [06-02-'25]
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail    :- This API retrieves all Company Type details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 06-Feb-2025
        ///  Last Modified  :- None
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Company details or an appropriate message</returns>
        [HttpGet("getallcompanytype")]
        public async Task<IActionResult> GetAllCompanyType()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<CompanyTypeRequest>>();
            // Fetching data from the repository by executing the stored procedure
            var companyType = await _repository.GetAllCompanyTypeAsync(DbConstants.GetCompanyType);
            // Check if data exists
            if (companyType != null && companyType.Any())
            {
               
                apiResponse.IsSuccess = true;
                apiResponse.Result = companyType;
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

        #endregion
        #region  [Company Correspondance CRUD] Added By Harshida [24-01-'25]
        /// <summary>
        ///  Developer Name :- Harshida Parmar
        ///  Message detail :- This API retrieves Company Correspondance details from the database 
        ///                    using a stored procedure. It checks if records exist and 
        ///                    returns the appropriate response.
        ///  Created Date   :- 24-Jan-2025
        ///  Last Modified  :- None
        ///  Modification   :- None
        /// </summary>
        /// <returns>A JSON response with Company details or an appropriate message</returns>
        [HttpGet("getcompanycorrespondancebyid/{id}")]
        public async Task<IActionResult> GetCompanyCorrespondanceById(int id)
        {
            ApiResponseModel<CompanyCorrespondance> apiResponse = new ApiResponseModel<CompanyCorrespondance>();

            var companyCorrespondance = await _repository.GetCompanyCorrespondanceByIdAsync(DbConstants.GetCompanyCorrespondanceById, new { Company_Id = (int?)null, Correspondance_ID = id });
            if (companyCorrespondance == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = companyCorrespondance;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        [HttpGet("getcompanycorrespondancebyCompanyid/{id}")]
        public async Task<IActionResult> GetCompanyCorrespondanceByCompanyid(int id)
        {
            //ApiResponseModel<CompanyCorrespondance> apiResponse = new ApiResponseModel<CompanyCorrespondance>();
            ApiResponseModel<IEnumerable<CompanyCorrespondance>> apiResponse = new ApiResponseModel<IEnumerable<CompanyCorrespondance>>();

            var companyCorrespondance = await _repository.GetAllCompanyCorrespondancesByCompanyIdAsync(DbConstants.GetCompanyCorrespondanceById, new { Company_Id = id });
            if (companyCorrespondance == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = companyCorrespondance;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        [HttpPut("putcompanycorrespondancemaster/{id}")]
        public async Task<IActionResult> PutCompanyCorrespondanceMaster(int id, [FromBody] CompanyCorrespondance companycorrespondanceMaster)
        {
            ApiResponseModel<CompanyCorrespondance> apiResponse = new ApiResponseModel<CompanyCorrespondance>();

            if (companycorrespondanceMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            // 2. Call AddOrUpdate for CompanyCorrespondance
            if (companycorrespondanceMaster != null)
            {
                //await _repository.AddCompanyAsync(DbConstants.AddEditCompanyCorrespondance, companycorrespondanceMaster, "CompanyCorrespondance");
                await _repository.UpdateCompanyCorrespondanceAsync(DbConstants.AddEditCompanyCorrespondance, companycorrespondanceMaster, "CompanyCorrespondance");

                if (companycorrespondanceMaster.MessageType == 1)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = companycorrespondanceMaster;
                    apiResponse.Message = companycorrespondanceMaster.StatusMessage;
                    apiResponse.MessageType = companycorrespondanceMaster.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.Created;
                    return StatusCode((int)HttpStatusCode.Created, apiResponse);
                }
                else
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = companycorrespondanceMaster.StatusMessage;
                    apiResponse.MessageType = companycorrespondanceMaster.MessageType;
                    apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                    return BadRequest(apiResponse);
                }
            }
            apiResponse.IsSuccess = true;
            apiResponse.Result = companycorrespondanceMaster;
            apiResponse.Message = ApiResponseMessageConstant.UpdateSuccessfully;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        [HttpPost("postcompanycorrespondancemaster")]
        public async Task<IActionResult> PostCompanyCorrespondanceMaster([FromBody] CompanyCorrespondance companycorrespondanceMaster)
        {
            ApiResponseModel<CompanyCorrespondance> apiResponse = new ApiResponseModel<CompanyCorrespondance>();
            if (companycorrespondanceMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddCompanyCorrespondanceAsync(DbConstants.AddEditCompanyCorrespondance, companycorrespondanceMaster, "CompanyCorrespondance");
            if (companycorrespondanceMaster.MessageType != 1)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = companycorrespondanceMaster.StatusMessage;
                apiResponse.MessageType = companycorrespondanceMaster.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            // If all steps succeeded
            apiResponse.IsSuccess = true;
            apiResponse.Message = companycorrespondanceMaster.StatusMessage;
            apiResponse.MessageType = companycorrespondanceMaster.MessageType;
            apiResponse.StatusCode = ApiResponseStatusConstant.Created;
            return StatusCode((int)HttpStatusCode.Created, apiResponse);
        }
        #endregion
        #region [Company Configuration CRUD] Added By Harshida [14-02-25]
        [HttpPost("postcompanyconfiguration")]
        public async Task<IActionResult> PostCompanyConfiguration([FromBody] CompanyConfigurationRequest companyconfiguration)
        {
            ApiResponseModel<CompanyConfigurationRequest> apiResponse = new ApiResponseModel<CompanyConfigurationRequest>();
            if (companyconfiguration == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.NullData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddCompanyConfigurationAsync(DbConstants.AddCompanyConfiguration, companyconfiguration);
            if (companyconfiguration.MessageType != 1)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = companyconfiguration.StatusMessage;
                apiResponse.MessageType = companyconfiguration.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            // If all steps succeeded
            apiResponse.IsSuccess = true;
            apiResponse.Message = companyconfiguration.StatusMessage;
            apiResponse.MessageType = companyconfiguration.MessageType;
            apiResponse.StatusCode = ApiResponseStatusConstant.Created;
            return StatusCode((int)HttpStatusCode.Created, apiResponse);
        }
        #endregion
        #region [Currency CRUD for Company currency] Added By Harshida [13-02-25]
        [HttpGet("getallcurrency")]
        public async Task<IActionResult> GetAllCurrency(int? currencyId = null, int? countryId = null, bool? isActive = null)
        {
            ApiResponseModel<IEnumerable<CurrencyRequest>> apiResponse = new ApiResponseModel<IEnumerable<CurrencyRequest>>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Currency_ID", currencyId);
                parameters.Add("@Countrty_Id", countryId);
                parameters.Add("@IsActive", isActive);

                var currencies = await _repository.GetAllCurrencyAsync(DbConstants.GetCurrencyMaster, parameters);
                if (currencies != null)
                {
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = currencies;
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
        #endregion

        #region [Added By Krunali] SubsidiaryMaster Crud APIs Functionality "payroll - 431"
        [HttpGet("getallsubsidiarymaster")]
        public async Task<IActionResult> GetAllSubsidiaryMaster()
        {
            var apiResponse = new ApiResponseModel<IEnumerable<SubsidiaryMaster>>();
            var subSidiaryMaster = await _repository.GetAllSubsidiaryMaster(DbConstants.GetCompanySubsidiaryMaster);

            if (subSidiaryMaster != null && subSidiaryMaster.Any())
            {
                apiResponse.IsSuccess = true;
                apiResponse.Result = subSidiaryMaster;
                apiResponse.StatusCode = (int)HttpStatusCode.OK;
                return Ok(subSidiaryMaster);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                return StatusCode((int)HttpStatusCode.NoContent, apiResponse);
            }
        }

        [HttpGet("getsubsidiarymasterbyid/{id}")]
        public async Task<IActionResult> GetSubsidiaryMasterById(int id)
        {
            ApiResponseModel<SubsidiaryMaster> apiResponse = new ApiResponseModel<SubsidiaryMaster>();
            var subSidiaryMaster = await _repository.GetAllSubsidiaryMasterById(DbConstants.GetCompanySubsidiaryMasterById, new { Subsidiary_Id = id });
            if (subSidiaryMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = subSidiaryMaster;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        [HttpPost("postsubsidiarymaster")]
        public async Task<IActionResult> PostSubsidiaryMaster([FromBody] SubsidiaryMaster subidiaryMaster)
        {
            ApiResponseModel<SubsidiaryMaster> apiResponse = new ApiResponseModel<SubsidiaryMaster>();
            if (subidiaryMaster == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = (ApiResponseMessageConstant.NullData);
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            await _repository.AddSubsidiaryMasterAsync(DbConstants.AddCompanySubsidiaryMaster, subidiaryMaster);
            if (subidiaryMaster.MessageType == 1)
            {
                apiResponse.IsSuccess = true;
                apiResponse.Message = subidiaryMaster.StatusMessage;
                apiResponse.MessageType = subidiaryMaster.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.Created;
                return StatusCode((int)HttpStatusCode.Created, apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = subidiaryMaster.StatusMessage;
                apiResponse.MessageType = subidiaryMaster.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                return Ok(apiResponse);
            }
        }

        [HttpPut("updataesubsidiarymaster/{id}")]
        public async Task<IActionResult> PutSubsidiaryMaster(int id, [FromBody] SubsidiaryMaster subsidiaryMaster)
        {
            ApiResponseModel<SubsidiaryMaster> apiResponse = new ApiResponseModel<SubsidiaryMaster>();
            if (subsidiaryMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
            subsidiaryMaster.Subsidiary_Id = id;
            await _repository.UpdateSubsidiaryMasterAsync(DbConstants.UpdateCompanySubsidiaryMaster, subsidiaryMaster);
            apiResponse.IsSuccess = true;
            apiResponse.Message = subsidiaryMaster.StatusMessage;
            apiResponse.MessageType = subsidiaryMaster.MessageType;
            apiResponse.StatusCode = subsidiaryMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return subsidiaryMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }

        [HttpDelete("deletesubsidiarymaster/{id}")]
        public async Task<IActionResult> DeleteSubsidiaryMaster(int id, [FromBody] SubsidiaryMaster subsidiaryMaster)
        {
            ApiResponseModel<SubsidiaryMaster> apiResponse = new ApiResponseModel<SubsidiaryMaster>();
            // Check if the provided area is null or the id doesn't match the Area_Id
            if (subsidiaryMaster == null || id <= 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            subsidiaryMaster.Subsidiary_Id = id;
            // Call the DeleteAsync method in the repository
            await _repository.DeleteSubsidiaryMasterAsync(DbConstants.DeleteCompanySubsidiaryMaster, subsidiaryMaster);
            apiResponse.IsSuccess = subsidiaryMaster.MessageType == 1;
            apiResponse.Message = subsidiaryMaster.StatusMessage;
            apiResponse.MessageType = subsidiaryMaster.MessageType;
            apiResponse.StatusCode = subsidiaryMaster.MessageType == 1 ? ApiResponseStatusConstant.Created : ApiResponseStatusConstant.BadRequest;
            return subsidiaryMaster.MessageType == 1 ? StatusCode((int)HttpStatusCode.Created, apiResponse) : BadRequest(apiResponse);
        }
        #endregion
    }
}
