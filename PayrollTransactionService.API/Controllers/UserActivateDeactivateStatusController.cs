using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.DAL.Interface;

namespace PayrollTransactionService.API.Controllers
{
    /// <summary>
    /// Developer Name :- Harshida Parmar
    /// Created Date   :- 23-Oct-2024
    /// Message detail :- Provides API endpoints for managing User Activate Deactivate Status master records.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserActivateDeactivateStatusController : ControllerBase
    {
        #region Constructor 
        private readonly IUserActivateDeactivateStatusRepository _repository;
        public UserActivateDeactivateStatusController(IUserActivateDeactivateStatusRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        #endregion
        #region User Activate Deactivate Status Update
        [HttpPut("updateServiceApprovalRejection/{id}")]
        public async Task<IActionResult> PutServiceApprovalRejection(int id, [FromBody] ServiceApprovalRejection model)
        {
            ApiResponseModel<ServiceApprovalRejection> apiResponse = new ApiResponseModel<ServiceApprovalRejection>();

            // Check if the provided model is null or the id doesn't match the Srv_Appr_Rej_Id
            if (model == null || id != model.Srv_Appr_Rej_Id)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.InvalidData;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }

            // Retrieve the existing record based on the id
            var existingRecord = await _repository.GetByIdAsync(DbConstants.GetPayrollTransactionServiceById, new { Srv_Appr_Rej_Id = id });
            if (existingRecord == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }
            model.Srv_Appr_Rej_Id = model.Srv_Appr_Rej_Id;
            model.Company_Id = existingRecord.Company_Id;
            model.Module_Id = existingRecord.Module_Id;
            model.KeyField_1 = existingRecord.KeyField_1;
            model.KeyField_2 = existingRecord.KeyField_2;
            model.KeyField_3 = existingRecord.KeyField_3;
            model.Requested_By = existingRecord.Requested_By;
            model.Requested_DateTime = existingRecord.Requested_DateTime;
            //model.Checked_Datetime = existingRecord.Checked_Datetime; 
            model.Approve_Reject_Level = existingRecord.Approve_Reject_Level;
            model.Approver_Id = existingRecord.Approver_Id;
            //model.Srv_Appr_Rej_Id = model.Srv_Appr_Rej_Id != 0 ? model.Srv_Appr_Rej_Id : existingRecord.Srv_Appr_Rej_Id;
            //model.Company_Id = model.Company_Id != 0 ? model.Company_Id : existingRecord.Company_Id;
            //model.Module_Id = model.Module_Id != 0 ? model.Module_Id : existingRecord.Module_Id;
            //model.KeyField_1 = model.KeyField_1 != 0 ? model.KeyField_1 : existingRecord.KeyField_1;
            //model.KeyField_2 = model.KeyField_2 ?? existingRecord.KeyField_2;
            //model.KeyField_3 = model.KeyField_3 ?? existingRecord.KeyField_3;
            //model.Requested_By = model.Requested_By != 0 ? model.Requested_By : existingRecord.Requested_By;
            //model.Requested_DateTime = model.Requested_DateTime != default(DateTime) ? model.Requested_DateTime : existingRecord.Requested_DateTime;
            //model.Approve_Reject_Level = model.Approve_Reject_Level != 0 ? model.Approve_Reject_Level : existingRecord.Approve_Reject_Level;


            // Call the UpdateAsync method in the repository
            var updatedRecord = await _repository.UpdateAsync(DbConstants.AddEditPayrollTransactionService, model);

            if (updatedRecord.MessageType == 1) // Assuming 1 means success in your MessageType enum
            {
                apiResponse.IsSuccess = true;
                apiResponse.Message = updatedRecord.StatusMessage;
                apiResponse.MessageType = updatedRecord.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
                apiResponse.Result = updatedRecord;
                return Ok(apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = updatedRecord.StatusMessage;
                apiResponse.MessageType = updatedRecord.MessageType;
                apiResponse.StatusCode = ApiResponseStatusConstant.BadRequest;
                return BadRequest(apiResponse);
            }
        }
        #endregion
    }
}
