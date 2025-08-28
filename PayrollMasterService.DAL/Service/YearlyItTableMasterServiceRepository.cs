/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the YearlyItTableMaster entity.                              *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all YearlyItTableMaster records using a stored procedure.                         *
 *  - GetByIdAsync   : Retrieves a specific YearlyItTableMaster record by ID using a stored procedure.             *
 *  - AddAsync       : Inserts a new YearlyItTableMaster record into the database using a stored procedure.        *
 *  - UpdateAsync    : Updates an existing YearlyItTableMaster record using a stored procedure.                    *
 *  - DeleteAsync    : Soft-deletes an YearlyItTableMaster record using a stored procedure.                        *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the IYearlyItTableMasterRepository interface.                                                     *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Priyanshu Jain                                                                                         *
 *  Date  : 21-Oct-2024                                                                                            *
 *                                                                                                                 *
 *******************************************************************************************************************/
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
    public class YearlyItTableMasterServiceRepository : IYearlyItTableMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public YearlyItTableMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #region  Yearly It Table Master Crud
        public async Task<IEnumerable<YearlyItTableMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<YearlyItTableMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<YearlyItTableMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<YearlyItTableMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<YearlyItTableMaster> AddAsync(string procedureName, YearlyItTableMaster model)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@YearlyItTable_Id", model.YearlyItTable_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@Company_Id", model.Company_Id);
            parameters.Add("@EffectiveDate", model.EffectiveDate);
            parameters.Add("@Remarks", model.Remarks);
            parameters.Add("@NewRegime", model.NewRegime);
            parameters.Add("@oldRegime", model.OldRegime);
            parameters.Add("@Currency_Id", model.Currency_Id);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@CreatedBy", model.CreatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.YearlyItTableMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }
            return model;
        }
        public async Task<YearlyItTableMaster> UpdateAsync(string procedureName, YearlyItTableMaster model)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@YearlyItTable_Id", model.YearlyItTable_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@Company_Id", model.Company_Id);
            parameters.Add("@EffectiveDate", model.EffectiveDate);
            parameters.Add("@Remarks", model.Remarks);
            parameters.Add("@NewRegime", model.NewRegime);
            parameters.Add("@oldRegime", model.OldRegime);
            parameters.Add("@Currency_Id", model.Currency_Id);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@UpdatedBy", model.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.YearlyItTableMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }
            return model; 
        }
        public async Task<YearlyItTableMaster> DeleteAsync(string procedureName, object yearlyItTableMaster)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@YearlyItTable_Id", ((dynamic)yearlyItTableMaster).YearlyItTable_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)yearlyItTableMaster).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.YearlyItTableMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)yearlyItTableMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)yearlyItTableMaster).MessageType = result.ApplicationMessageType;
            }
            return (YearlyItTableMaster)yearlyItTableMaster;
        }
        #endregion
    }
}
