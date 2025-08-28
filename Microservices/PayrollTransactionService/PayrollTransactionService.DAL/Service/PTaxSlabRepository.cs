using Dapper;
using Payroll.Common.ApplicationConstant;
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
    public class PTaxSlabRepository : IPTaxSlabRepository
    {
        private readonly IDbConnection _dbConnection;
        public PTaxSlabRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<PtaxSlabRequest>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<PtaxSlabRequest>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<PtaxSlabRequest> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<PtaxSlabRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<TaxParamRequest>> GetTaxParamAsync(string procedureName, int taxType, int? stateId)
        {
            var parameters = new
            {
                Tax_Type = taxType,
                State_Id = stateId
            };

            return await _dbConnection.QueryAsync<TaxParamRequest>(
                procedureName,
                new DynamicParameters(parameters),
                commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<ProfessionalTaxSlabViewModel>> GetAllAsyncWithId(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<ProfessionalTaxSlabViewModel>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<PtaxSlabRequest> AddAsync(string procedureName, PtaxSlabRequest ptaxSlab)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Ptax_Slab_Id", ptaxSlab.Ptax_Slab_Id, DbType.Int32);
            parameters.Add("@Company_Id", ptaxSlab.Company_Id, DbType.Int32);
            parameters.Add("@State_ID", ptaxSlab.State_ID, DbType.String);
            parameters.Add("@Min_Income", ptaxSlab.Min_Income, DbType.Decimal);
            parameters.Add("@Max_Income", ptaxSlab.Max_Income, DbType.Decimal);
            parameters.Add("@Frequency", ptaxSlab.Frequency, DbType.Int32);
            parameters.Add("@SpecialDeductionMonth", ptaxSlab.SpecialDeductionMonth, DbType.Int32);
            parameters.Add("@PTaxAmt", ptaxSlab.PTaxAmt, DbType.Decimal);
            parameters.Add("@SpecialPTaxAmt", ptaxSlab.SpecialPTaxAmt, DbType.Decimal);
            parameters.Add("@Gender", ptaxSlab.Gender, DbType.Int32);
            parameters.Add("@SrCitizenAge", ptaxSlab.SrCitizenAge, DbType.Int32);
            parameters.Add("@Is_YearEnd_Adjustment", ptaxSlab.Is_YearEnd_Adjustment, DbType.Boolean);
            parameters.Add("@Effective_From", ptaxSlab.Effective_From, DbType.Date);
            parameters.Add("@Effective_To", ptaxSlab.Effective_To, DbType.Date);
            //parameters.Add("@IsActive", ptaxSlab.IsActive, DbType.Boolean);
            parameters.Add("@CreatedBy", ptaxSlab.CreatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); 
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PTaxSlab); 
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                ptaxSlab.StatusMessage = result.ApplicationMessage;
                ptaxSlab.MessageType = ((dynamic)result).ApplicationMessageType;
            }
            return ptaxSlab;
        }
        public async Task<PtaxSlabRequest> DeleteAsync(string procedureName, object pTaxSlab)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Ptax_Slab_Id", ((dynamic)pTaxSlab).Ptax_Slab_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)pTaxSlab).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EarningDeductionMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)pTaxSlab).StatusMessage = result.ApplicationMessage;
                ((dynamic)pTaxSlab).MessageType = result.ApplicationMessageType;
            }
            return (PtaxSlabRequest)pTaxSlab;
        }
        public async Task<PtaxSlabRequest> UpdateAsync(string procedureName, PtaxSlabRequest ptaxSlab)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Ptax_Slab_Id", ptaxSlab.Ptax_Slab_Id, DbType.Int32);
            parameters.Add("@Company_Id", ptaxSlab.Company_Id, DbType.Int32);
            parameters.Add("@State_ID", ptaxSlab.State_ID, DbType.String);
            parameters.Add("@Min_Income", ptaxSlab.Min_Income, DbType.Decimal);
            parameters.Add("@Max_Income", ptaxSlab.Max_Income, DbType.Decimal);
            parameters.Add("@Frequency", ptaxSlab.Frequency, DbType.Int32);
            parameters.Add("@SpecialDeductionMonth", ptaxSlab.SpecialDeductionMonth, DbType.Int32);
            parameters.Add("@PTaxAmt", ptaxSlab.PTaxAmt, DbType.Decimal);
            parameters.Add("@SpecialPTaxAmt", ptaxSlab.SpecialPTaxAmt, DbType.Decimal);
            parameters.Add("@Gender", ptaxSlab.Gender, DbType.Int32);
            parameters.Add("@SrCitizenAge", ptaxSlab.SrCitizenAge, DbType.Int32);
            parameters.Add("@Is_YearEnd_Adjustment", ptaxSlab.Is_YearEnd_Adjustment, DbType.Boolean);
            parameters.Add("@Effective_From", ptaxSlab.Effective_From, DbType.Date);
            parameters.Add("@Effective_To", ptaxSlab.Effective_To, DbType.Date);
            parameters.Add("@UpdatedBy", ptaxSlab.UpdatedBy, DbType.Int32);
            //parameters.Add("@Isactive", ptaxSlab.IsActive, DbType.Boolean);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PTaxSlab);
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                ptaxSlab.StatusMessage = result.ApplicationMessage;
                ptaxSlab.MessageType = ((dynamic)result).ApplicationMessageType;
            }
            return ptaxSlab;
        }
    }
}
