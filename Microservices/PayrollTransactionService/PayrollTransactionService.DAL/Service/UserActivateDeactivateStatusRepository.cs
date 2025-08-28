using Dapper;
using Payroll.Common.EnumUtility;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Service
{
    public class UserActivateDeactivateStatusRepository : IUserActivateDeactivateStatusRepository
    {
        #region Constructor 
        private readonly IDbConnection _dbConnection;
        public UserActivateDeactivateStatusRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion
        #region Service Approval Rejection Endpoint Handlers (CRUD)
        #region Service Approval Rejection Fetch All And By ID
        public async Task<IEnumerable<ServiceApprovalRejection>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<ServiceApprovalRejection>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<ServiceApprovalRejection> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            
            return await _dbConnection.QuerySingleOrDefaultAsync<ServiceApprovalRejection>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        #endregion
        #region Service Approval Rejection Add
        //public async Task<ServiceApprovalRejection> AddAsync(string procedureName, ServiceApprovalRejection serviceApprovalRejectionDetail)
        //{
        //    var parameters = new DynamicParameters();

        //    // Required parameters
        //    parameters.Add("@Srv_Appr_Rej_Id", serviceApprovalRejectionDetail.Srv_Appr_Rej_Id);
        //    parameters.Add("@Company_Id", serviceApprovalRejectionDetail.Company_Id);
        //    parameters.Add("@Module_Id", serviceApprovalRejectionDetail.Module_Id);
        //    parameters.Add("@KeyField_1", serviceApprovalRejectionDetail.KeyField_1);
        //    parameters.Add("@KeyField_2", serviceApprovalRejectionDetail.KeyField_2);
        //    parameters.Add("@KeyField_3", serviceApprovalRejectionDetail.KeyField_3);
        //    parameters.Add("@Requested_By", serviceApprovalRejectionDetail.Requested_By);
        //    parameters.Add("@Requested_DateTime", serviceApprovalRejectionDetail.Requested_DateTime);
        //    parameters.Add("@Request_Status", serviceApprovalRejectionDetail.Request_Status);
        //    parameters.Add("@Approve_Reject_Level", serviceApprovalRejectionDetail.Approve_Reject_Level);

        //    //    // Optional parameters (Nullable in database)

        //    parameters.Add("@Rejection_Reason", serviceApprovalRejectionDetail.Rejection_Reason);
        //    parameters.Add("@Checked_By", serviceApprovalRejectionDetail.Checked_By);
        //    parameters.Add("@Checked_Datetime", serviceApprovalRejectionDetail.Checked_Datetime);
        //    parameters.Add("@Rejection_Reason", serviceApprovalRejectionDetail.Rejection_Reason);

        //    // Additional parameters for messages and status using enum values
        //    parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
        //    parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
        //    parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.WageRateMaster); // Cast enum to int

        //    var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

        //    if (result != null)
        //    {
        //        serviceApprovalRejectionDetail.StatusMessage = result.ApplicationMessage;
        //        serviceApprovalRejectionDetail.MessageType = result.ApplicationMessageType;
        //    }
        //    return serviceApprovalRejectionDetail;
        //}
        public Task<ServiceApprovalRejection> AddAsync(string procedureName, ServiceApprovalRejection model)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Service Approval Rejection Update
        public async Task<ServiceApprovalRejection> UpdateAsync(string procedureName, ServiceApprovalRejection model)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();

                dynamicParameters.Add("@Srv_Appr_Rej_Id", model.Srv_Appr_Rej_Id);
                dynamicParameters.Add("@company_Id", model.Company_Id);
                //dynamicParameters.Add("@Module_Id", model.Module_Id);
                //dynamicParameters.Add("@KeyField_1", model.KeyField_1);
                //dynamicParameters.Add("@KeyField_2", model.KeyField_2);
                //dynamicParameters.Add("@KeyField_3", model.KeyField_3);
                dynamicParameters.Add("@Checked_By", model.Checked_By);
                //dynamicParameters.Add("@Checked_Datetime", model.Checked_Datetime);
                dynamicParameters.Add("@Request_Status", model.Request_Status);
                dynamicParameters.Add("@Rejection_Reason", model.Rejection_Reason);
                //dynamicParameters.Add("@Requested_DateTime", model.Requested_DateTime);

                dynamicParameters.Add("@MessageType", model.MessageType); // Example value from the model
                dynamicParameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Assuming mode is 'Update'
                dynamicParameters.Add("@ModuleId", model.Module_Id); // Assuming Module_Id for module

                // Output parameters
                dynamicParameters.Add("@Out_Srv_Appr_Rej_Id", dbType: DbType.Int64, direction: ParameterDirection.Output);
                dynamicParameters.Add("@Out_MessageMode", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Execute the stored procedure and get the result              
                var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

                // Fetch the output parameters correctly
                model.Srv_Appr_Rej_Id = dynamicParameters.Get<long>("@Out_Srv_Appr_Rej_Id"); // Change int to long
                model.OutMessageMode = dynamicParameters.Get<int?>("@Out_MessageMode");

                if (result != null)
                {
                    model.StatusMessage = result.ApplicationMessage;
                    model.MessageType = result.ApplicationMessageType;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while executing {procedureName}: {ex.Message}", ex);
            }
        }
        #endregion
        #region Service Approval Rejection Delete
        public Task<ServiceApprovalRejection> DeleteAsync(string procedureName, object parameters)
        {
            throw new NotImplementedException();
        }       
        #endregion
        #endregion
    }
}
