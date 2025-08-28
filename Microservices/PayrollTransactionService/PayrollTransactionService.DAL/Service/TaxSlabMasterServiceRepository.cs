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
    public class TaxSlabMasterServiceRepository : ITaxSlabMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public TaxSlabMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region Pay Component Master Crud

        public async Task<IEnumerable<TaxSlabMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<TaxSlabMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<TaxRegimeMaster>> GetAllTaxRegimeAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<TaxRegimeMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<TaxSlabMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<TaxSlabMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<TaxSlabMaster> AddAsync(string procedureName, TaxSlabMaster model)
        {
            // Executing the stored procedure with the given parameters
            var Parameters = new DynamicParameters();
            Parameters.Add("@YearlyItTable_Id", model.YearlyItTable_Id, DbType.Int32);
            Parameters.Add("@Company_Id", model.Company_Id, DbType.Int32);
            Parameters.Add("@SlabName", model.SlabName, DbType.String);
            Parameters.Add("@Income_From", model.Income_From, DbType.Decimal);
            Parameters.Add("@Income_To", model.Income_To, DbType.Decimal);
            Parameters.Add("@TaxPaybleInPercentage", model.TaxPaybleInPercentage, DbType.Int32);
            Parameters.Add("@TaxPaybleInAmount", model.TaxPaybleInAmount, DbType.Decimal);
            Parameters.Add("@IsActive", model.IsActive, DbType.Boolean);
            Parameters.Add("@CreatedBy", model.CreatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            Parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            Parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            Parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.DetailYearlyItTableMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, Parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = ((dynamic)result).ApplicationMessageType;
            }
            return model;
        }
        public async Task<TaxSlabMaster> UpdateAsync(string procedureName, TaxSlabMaster model)
        {
            // Executing the stored procedure with the given parameters
            var Parameters = new DynamicParameters();
            Parameters.Add("@YearlyItTableDetail_Id", model.YearlyItTableDetail_Id, DbType.Int32);
            Parameters.Add("@YearlyItTable_Id", model.YearlyItTable_Id, DbType.Int32);
            Parameters.Add("@Company_Id", model.Company_Id, DbType.Int32);
            Parameters.Add("@SlabName", model.SlabName, DbType.String);
            Parameters.Add("@Income_From", model.Income_From, DbType.Decimal);
            Parameters.Add("@Income_To", model.Income_To, DbType.Decimal);
            Parameters.Add("@TaxPaybleInPercentage", model.TaxPaybleInPercentage, DbType.Int32);
            Parameters.Add("@TaxPaybleInAmount", model.TaxPaybleInAmount, DbType.Decimal);
            Parameters.Add("@IsActive", model.IsActive, DbType.Boolean);
            Parameters.Add("@CreatedBy", model.CreatedBy, DbType.Int32);
            Parameters.Add("@UpdatedBy", model.UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            Parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            Parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            Parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.DetailYearlyItTableMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, Parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }
            return model;
        }
        public async Task<TaxSlabMaster> DeleteAsync(string procedureName, object taxSlabMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@YearlyItTableDetail_Id", ((dynamic)taxSlabMaster).YearlyItTableDetail_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)taxSlabMaster).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.DetailYearlyItTableMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)taxSlabMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)taxSlabMaster).MessageType = result.ApplicationMessageType;
            }
            return (TaxSlabMaster)taxSlabMaster;
        }

        public async Task<IEnumerable<TaxSlabMaster>> GetAllTaxRegimeByCompanyAsync(string procedureName, object parameter)
        {
            var dynamicParameters = new DynamicParameters(parameter); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QueryAsync<TaxSlabMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

        }

        #endregion
    }
}
