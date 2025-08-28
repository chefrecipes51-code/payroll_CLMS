/***************************************************************************************************************
 *                                                                                                             *
 *  Description:                                                                                               *
 *  This repository class handles CRUD operations for the CompanyCurrencyMaster entity.                        *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.              *
 *                                                                                                             *
 *  Methods:                                                                                                   *
 *  - GetAllAsync    : Retrieves all CompanyCurrencyMaster records using a stored procedure.                   *
 *  - GetByIdAsync   : Retrieves a specific CompanyCurrencyMaster record by ID using a stored procedure.       *
 *  - AddAsync       : Inserts a new CompanyCurrencyMaster record into the database using a stored procedure.  *
 *  - UpdateAsync    : Updates an existing CompanyCurrencyMaster record using a stored procedure.              *
 *  - DeleteAsync    : Soft-deletes an CompanyCurrencyMaster record using a stored procedure.                  *
 *                                                                                                             *
 *  Key Features:                                                                                              *
 *  - Implements the ICompanyCurrencyMasterRepository interface.                                               *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                  *
 *  - Includes application-level enums for message type, mode, and module ID.                                  *
 *  - Ensures validation of returned messages and status from stored procedure execution.                      *
 *                                                                                                             *
 *  Author: Priyanshu Jain                                                                                     *
 *  Date  : 25-Sep-2024                                                                                        *
 *                                                                                                             *
 ***************************************************************************************************************/
using Dapper;
using Payroll.Common.EnumUtility;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Service
{
    public class CompanyCurrencyMasterServiceRepository : ICompanyCurrencyMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public CompanyCurrencyMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #region Company Currency Master Crud 
        public async Task<IEnumerable<CompanyCurrencyMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<CompanyCurrencyMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<CompanyCurrencyMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<CompanyCurrencyMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<CompanyCurrencyMaster> AddAsync(string procedureName, CompanyCurrencyMaster companyCurrencyMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyCurrency_Id", companyCurrencyMaster.CompanyCurrency_Id);  // 0 means insert, greater than 0 means update
            parameters.Add("@Currency_Id", companyCurrencyMaster.Currency_Id);
            parameters.Add("@Country_Id", companyCurrencyMaster.Country_Id);
            parameters.Add("@Currency_Name", companyCurrencyMaster.Currency_Name);
            parameters.Add("@Currency_Symbol", companyCurrencyMaster.Currency_Symbol);
            parameters.Add("@Currency_Description", companyCurrencyMaster.Currency_Description);
            parameters.Add("@Currncy_decimalPoint", companyCurrencyMaster.Currncy_decimalPoint);
            parameters.Add("@IsDefault", companyCurrencyMaster.IsDefault);
            parameters.Add("@Currency_Exchange_Rate", companyCurrencyMaster.Currency_Exchange_Rate);
            parameters.Add("@IsActive", companyCurrencyMaster.IsActive);
            parameters.Add("@CreatedBy", companyCurrencyMaster.CreatedBy);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);      // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyCurrencyMaster);          // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                companyCurrencyMaster.StatusMessage = result.ApplicationMessage;
                companyCurrencyMaster.MessageType = result.ApplicationMessageType;
            }
            return companyCurrencyMaster;
        }
        public async Task<CompanyCurrencyMaster> UpdateAsync(string procedureName, CompanyCurrencyMaster companyCurrencyMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyCurrency_Id", companyCurrencyMaster.CompanyCurrency_Id);  // 0 means insert, greater than 0 means update
            parameters.Add("@Currency_Id", companyCurrencyMaster.Currency_Id);
            parameters.Add("@Country_Id", companyCurrencyMaster.Country_Id);
            parameters.Add("@Currency_Name", companyCurrencyMaster.Currency_Name);
            parameters.Add("@Currency_Symbol", companyCurrencyMaster.Currency_Symbol);
            parameters.Add("@Currency_Description", companyCurrencyMaster.Currency_Description);
            parameters.Add("@Currncy_decimalPoint", companyCurrencyMaster.Currncy_decimalPoint);
            parameters.Add("@IsDefault", companyCurrencyMaster.IsDefault);
            parameters.Add("@Currency_Exchange_Rate", companyCurrencyMaster.Currency_Exchange_Rate);
            parameters.Add("@IsActive", companyCurrencyMaster.IsActive);
            parameters.Add("@CreatedBy", companyCurrencyMaster.CreatedBy);
            parameters.Add("@UpdatedBy", companyCurrencyMaster.UpdatedBy); // Assuming it's nullable
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);      // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyCurrencyMaster);          // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                companyCurrencyMaster.StatusMessage = result.ApplicationMessage;
                companyCurrencyMaster.MessageType = result.ApplicationMessageType;
            }
            return companyCurrencyMaster;
        }
        public async Task<CompanyCurrencyMaster> DeleteAsync(string procedureName, object companyCurrencyMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyCurrency_Id", ((dynamic)companyCurrencyMaster).CompanyCurrency_Id); // Assuming 0 for insert, update with specific ID
            parameters.Add("@UpdatedBy", ((dynamic)companyCurrencyMaster).UpdatedBy);
            // Additional parameters for messages and status (using enum values)
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.CompanyCurrencyMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)companyCurrencyMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)companyCurrencyMaster).MessageType = result.ApplicationMessageType;
            }
            return (CompanyCurrencyMaster)companyCurrencyMaster;
        }
        #endregion
    }
}
