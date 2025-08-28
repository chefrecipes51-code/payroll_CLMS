using Dapper;
using Payroll.Common.EnumUtility;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Service
{
    public class GeneralAccountingHeadRepository : IGeneralAccountingHeadRepository
    {
        private readonly IDbConnection _dbConnection;
        public GeneralAccountingHeadRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<AccountingHeadRequest>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<AccountingHeadRequest>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<AccountingHeadRequest> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<AccountingHeadRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<AccountType>> GetAllAccountTypesAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<AccountType>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<AccountingHeadRequest> AddAsync(string procedureName, AccountingHeadRequest model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Accounting_Head_Id", 0); // For Insert
            parameters.Add("@Accounting_Head_Name", model.Accounting_Head_Name);
            parameters.Add("@GL_Code", model.GL_Code);
            parameters.Add("@Account_Type", model.Account_Type);
            parameters.Add("@Parent_Gl_Group_Id", model.Parent_Gl_Group_Id);
            parameters.Add("@Sub_Gl_Group_Id", model.Sub_Gl_Group_Id ?? 0);
            parameters.Add("@Tran_Type", model.Tran_Type);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.AccountingHead);
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }
            return model;
        }
        public async Task<AccountingHeadRequest> UpdateAsync(string procedureName, AccountingHeadRequest model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Accounting_Head_Id", model.Accounting_Head_Id); // For Update
            parameters.Add("@Accounting_Head_Name", model.Accounting_Head_Name);
            parameters.Add("@GL_Code", model.GL_Code);
            parameters.Add("@Account_Type", model.Account_Type);
            parameters.Add("@Tran_Type", model.Tran_Type);
            parameters.Add("@Parent_Gl_Group_Id", model.Parent_Gl_Group_Id);
            parameters.Add("@Sub_Gl_Group_Id", model.Sub_Gl_Group_Id ?? 0);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@CreatedBy", model.CreatedBy); // Still needed
            parameters.Add("@UpdatedBy", model.UpdatedBy);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.AccountingHead);
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }
            return model;
        }
        public async Task<AccountingHeadRequest> DeleteAsync(string procedureName, object parametersObj)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Accounting_Head_Id", ((dynamic)parametersObj).Accounting_Head_Id);
            parameters.Add("@UpdatedBy", ((dynamic)parametersObj).UpdatedBy);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.AccountingHead);

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                ((dynamic)parametersObj).StatusMessage = result.ApplicationMessage;
                ((dynamic)parametersObj).MessageType = result.ApplicationMessageType;
            }

            return (AccountingHeadRequest)parametersObj;
        }



    }
}
