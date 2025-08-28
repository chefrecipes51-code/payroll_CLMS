using Dapper;
using Payroll.Common.EnumUtility;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Service
{
    public class PayGradeMasterServiceRepository : IPayGradeMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public PayGradeMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region Pay Grade Master Crud
        public async Task<IEnumerable<PayGradeMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<PayGradeMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<PayGradeMaster>> GetAllActiveAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<PayGradeMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<TradeMaster>> GetAllTradeAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<TradeMaster>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<SkillCategory>> GetAllSkillCategoryAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<SkillCategory>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<DistinctLocation>> GetAllDistinctLocationAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<DistinctLocation>(
                procedureName,
                dynamicParameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<PayGradeMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<PayGradeMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<PayGradeMaster> AddAsync(string procedureName, PayGradeMaster payGradeMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PayGrade_Id", payGradeMaster.PayGrade_Id);
            parameters.Add("@Cmp_Id", payGradeMaster.Cmp_Id);
            parameters.Add("@PayGradeCode", payGradeMaster.PayGradeCode);
            parameters.Add("@PayGradeName", payGradeMaster.PayGradeName);
            parameters.Add("@PayGradeDesc", payGradeMaster.PayGradeDesc);
            parameters.Add("@MinSalary", payGradeMaster.MinSalary);
            parameters.Add("@MaxSalary", payGradeMaster.MaxSalary);
            parameters.Add("@IsActive", payGradeMaster.IsActive);
            parameters.Add("@CreatedBy", payGradeMaster.CreatedBy);

            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PayGradeMaster); // Cast enum to int

            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                payGradeMaster.StatusMessage = result.ApplicationMessage;
                payGradeMaster.MessageType = result.ApplicationMessageType;
            }
            return payGradeMaster;
        }
        public async Task<PayGradeMaster> UpdateAsync(string procedureName, PayGradeMaster payGradeMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PayGrade_Id", payGradeMaster.PayGrade_Id);
            parameters.Add("@Cmp_Id", payGradeMaster.Cmp_Id);
            parameters.Add("@PayGradeCode", payGradeMaster.PayGradeCode);
            parameters.Add("@PayGradeName", payGradeMaster.PayGradeName);
            parameters.Add("@PayGradeDesc", payGradeMaster.PayGradeDesc);
            parameters.Add("@MinSalary", payGradeMaster.MinSalary);
            parameters.Add("@MaxSalary", payGradeMaster.MaxSalary);
            parameters.Add("@IsActive", payGradeMaster.IsActive);
            parameters.Add("@CreatedBy", payGradeMaster.CreatedBy);
            parameters.Add("@UpdatedBy", payGradeMaster.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PayGradeMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                payGradeMaster.StatusMessage = result.ApplicationMessage;
                payGradeMaster.MessageType = result.ApplicationMessageType;
            }
            return payGradeMaster;
        }
        public async Task<PayGradeMaster> DeleteAsync(string procedureName, object payGradeMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PayGrade_Id", ((dynamic)payGradeMaster).PayGrade_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)payGradeMaster).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PayGradeMaster); // Cast enum to int
                                                                                     // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)payGradeMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)payGradeMaster).MessageType = result.ApplicationMessageType;
            }
            return (PayGradeMaster)payGradeMaster;
        }
        #endregion

        #region Pay Grade Config Master Crud
        public async Task<IEnumerable<PayGradeConfigMaster>> GetAllPayGradeConfigAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<PayGradeConfigMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<PayGradeConfigMaster> GetPayGradeConfigByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<PayGradeConfigMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<PayGradeConfigMaster> AddPayGradeConfigAsync(string procedureName, PayGradeConfigMaster payGradeMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Cmp_Id", payGradeMaster.Cmp_Id);
            parameters.Add("@Correspondance_ID", payGradeMaster.Correspondance_ID);
            parameters.Add("@GradeConfigName", payGradeMaster.GradeConfigName);
            parameters.Add("@PayGrade_Id", payGradeMaster.PayGrade_Id);
            parameters.Add("@Trade_Id", payGradeMaster.Trade_Id);
            parameters.Add("@SkillType_Id", payGradeMaster.SkillType_Id);
            parameters.Add("@EffectiveFrom", payGradeMaster.EffectiveFrom);
            parameters.Add("@EffectiveTo", payGradeMaster.EffectiveTo);
            parameters.Add("@IsActive", payGradeMaster.IsActive);
            parameters.Add("@CreatedBy", payGradeMaster.CreatedBy);

            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PayGradeMaster); // Cast enum to int

            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                payGradeMaster.StatusMessage = result.ApplicationMessage;
                payGradeMaster.MessageType = result.ApplicationMessageType;
            }
            return payGradeMaster;
        }
        public async Task<PayGradeConfigMaster> UpdatePayGradeConfigAsync(string procedureName, PayGradeConfigMaster payGradeMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PayGradeConfig_Id", payGradeMaster.PayGradeConfig_Id);
            parameters.Add("@Cmp_Id", payGradeMaster.Cmp_Id);
            parameters.Add("@Correspondance_ID", payGradeMaster.Correspondance_ID);
            parameters.Add("@GradeConfigName", payGradeMaster.GradeConfigName);
            parameters.Add("@PayGrade_Id", payGradeMaster.PayGrade_Id);
            parameters.Add("@Trade_Id", payGradeMaster.Trade_Id);
            parameters.Add("@SkillType_Id", payGradeMaster.SkillType_Id);
            parameters.Add("@EffectiveFrom", payGradeMaster.EffectiveFrom);
            parameters.Add("@EffectiveTo", payGradeMaster.EffectiveTo);
            parameters.Add("@IsActive", payGradeMaster.IsActive);
            parameters.Add("@CreatedBy", payGradeMaster.CreatedBy);
            parameters.Add("@UpdatedBy", payGradeMaster.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PayGradeMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                payGradeMaster.StatusMessage = result.ApplicationMessage;
                payGradeMaster.MessageType = result.ApplicationMessageType;
            }
            return payGradeMaster;
        }
        public async Task<PayGradeConfigMaster> DeletePayGradeConfigAsync(string procedureName, object payGradeMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PayGradeConfig_Id", ((dynamic)payGradeMaster).PayGradeConfig_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)payGradeMaster).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PayGradeMaster); // Cast enum to int
                                                                                     // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)payGradeMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)payGradeMaster).MessageType = result.ApplicationMessageType;
            }
            return (PayGradeConfigMaster)payGradeMaster;
        }

        #endregion
    }
}
