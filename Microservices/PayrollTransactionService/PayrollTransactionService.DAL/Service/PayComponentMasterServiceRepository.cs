using Dapper;
using Payroll.Common.EnumUtility;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;
using PayrollTransactionService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Service
{
    public class PayComponentMasterServiceRepository : IPayComponentMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public PayComponentMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region Pay Component Master Crud
        public async Task<IEnumerable<PayComponentMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<PayComponentMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<PayComponentMaster>> GetAllByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<PayComponentMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<PayComponentMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<PayComponentMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<PayComponentMaster> AddAsync(string procedureName, PayComponentMaster earningDeductionMaster)
        {
            // Executing the stored procedure with the given parameters
            var Parameters = new DynamicParameters();
            Parameters.Add("@Company_Id", earningDeductionMaster.Company_Id, DbType.Int32);
            Parameters.Add("@Is_Child", earningDeductionMaster.Is_Child, DbType.Boolean);
            Parameters.Add("@Parent_EarningDeduction_Id", earningDeductionMaster.Parent_EarningDeduction_Id, DbType.Int64);
            Parameters.Add("@EarningDeductionName", earningDeductionMaster.EarningDeductionName, DbType.String);
            Parameters.Add("@CalculationType", earningDeductionMaster.CalculationType, DbType.Int32);
            Parameters.Add("@EarningDeductionType", earningDeductionMaster.EarningDeductionType, DbType.Int32);
            Parameters.Add("@MinimumUnit_value", earningDeductionMaster.MinimumUnit_value, DbType.Decimal);
            Parameters.Add("@MaximumUnit_value", earningDeductionMaster.MaximumUnit_value, DbType.Decimal);
            Parameters.Add("@Amount", earningDeductionMaster.Amount, DbType.Decimal);
            Parameters.Add("@Formula", earningDeductionMaster.Formula, DbType.String);
            Parameters.Add("@Formula_Id", earningDeductionMaster.Formula_Id, DbType.Int64);
            Parameters.Add("@IsActive", earningDeductionMaster.IsActive, DbType.Boolean);
            Parameters.Add("@CreatedBy", earningDeductionMaster.CreatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            Parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            Parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            Parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EarningDeductionMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, Parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                earningDeductionMaster.StatusMessage = result.ApplicationMessage;
                earningDeductionMaster.MessageType = ((dynamic)result).ApplicationMessageType;
            }
            return earningDeductionMaster;
        }
        public async Task<PayComponentMaster> UpdateAsync(string procedureName, PayComponentMaster earningDeductionMaster)
        {
            // Executing the stored procedure with the given parameters
            var Parameters = new DynamicParameters();
            Parameters.Add("@EarningDeduction_Id", earningDeductionMaster.EarningDeduction_Id, DbType.Int32);
            Parameters.Add("@Company_Id", earningDeductionMaster.Company_Id, DbType.Int32);
            Parameters.Add("@Is_Child", earningDeductionMaster.Is_Child, DbType.Boolean);
            Parameters.Add("@Parent_EarningDeduction_Id", earningDeductionMaster.Parent_EarningDeduction_Id, DbType.Int64);
            Parameters.Add("@EarningDeductionName", earningDeductionMaster.EarningDeductionName, DbType.String);
            Parameters.Add("@CalculationType", earningDeductionMaster.CalculationType, DbType.Int32);
            Parameters.Add("@EarningDeductionType", earningDeductionMaster.EarningDeductionType, DbType.Int32);
            Parameters.Add("@MinimumUnit_value", earningDeductionMaster.MinimumUnit_value, DbType.Decimal);
            Parameters.Add("@MaximumUnit_value", earningDeductionMaster.MaximumUnit_value, DbType.Decimal);
            Parameters.Add("@Amount", earningDeductionMaster.Amount, DbType.Decimal);
            Parameters.Add("@Formula", earningDeductionMaster.Formula, DbType.String);
            Parameters.Add("@Formula_Id", earningDeductionMaster.Formula_Id, DbType.Int64);
            Parameters.Add("@IsActive", earningDeductionMaster.IsActive, DbType.Boolean);
            Parameters.Add("@CreatedBy", earningDeductionMaster.CreatedBy, DbType.Int32);
            Parameters.Add("@UpdatedBy", earningDeductionMaster.UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            Parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            Parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            Parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EarningDeductionMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, Parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                earningDeductionMaster.StatusMessage = result.ApplicationMessage;
                earningDeductionMaster.MessageType = result.ApplicationMessageType;
            }
            return earningDeductionMaster;
        }
        public async Task<PayComponentActivationRequest> UpdatePayComponentStatusAsync(string procedureName, PayComponentActivationRequest earningDeductionMaster)
        {
            // Executing the stored procedure with the given parameters
            var Parameters = new DynamicParameters();
            Parameters.Add("@EarningDeduction_Id", earningDeductionMaster.EarningDeduction_Id, DbType.Int32);       
            Parameters.Add("@UpdatedBy", earningDeductionMaster.UpdatedBy, DbType.Int32);
            Parameters.Add("@MessageType", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            Parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            Parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EarningDeductionMaster); // Cast enum to int
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, Parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                earningDeductionMaster.StatusMessage = result.ApplicationMessage;
                earningDeductionMaster.MessageType = result.ApplicationMessageType;
            }
            return earningDeductionMaster;
        }
        public async Task<PayComponentMaster> DeleteAsync(string procedureName, object earningDeductionMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@EarningDeduction_Id", ((dynamic)earningDeductionMaster).EarningDeduction_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)earningDeductionMaster).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EarningDeductionMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)earningDeductionMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)earningDeductionMaster).MessageType = result.ApplicationMessageType;
            }
            return (PayComponentMaster)earningDeductionMaster;
        }
        #endregion
    }
}
