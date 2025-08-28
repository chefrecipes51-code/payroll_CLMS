/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the DetailYearlyItTableMaster entity.                        *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all DetailYearlyItTableMaster records using a stored procedure.                   *
 *  - GetByIdAsync   : Retrieves a specific DetailYearlyItTableMaster record by ID using a stored procedure.       *
 *  - AddAsync       : Inserts a new DetailYearlyItTableMaster record into the database using a stored procedure.  *
 *  - UpdateAsync    : Updates an existing DetailYearlyItTableMaster record using a stored procedure.              *
 *  - DeleteAsync    : Soft-deletes an DetailYearlyItTableMaster record using a stored procedure.                  *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the IDetailYearlyItTableMasterRepository interface.                                                        *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Priyanshu Jain                                                                                         *
 *  Date  : 22-Oct-2024                                                                                            *
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
    public class DetailYearlyItTableMasterServiceRepository : IDetailYearlyItTableMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public DetailYearlyItTableMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
       
        #region Detail Yearly It Table Master Crud
        public async Task<IEnumerable<DetailYearlyItTableMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<DetailYearlyItTableMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<DetailYearlyItTableMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<DetailYearlyItTableMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<DetailYearlyItTableMaster> AddAsync(string procedureName, DetailYearlyItTableMaster model)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@YearlyItTableDetail_Id", model.YearlyItTableDetail_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@YearlyItTable_Id", model.YearlyItTable_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@Company_Id", model.Company_Id);
            parameters.Add("@Income_From", model.Income_From);
            parameters.Add("@Income_To", model.Income_To);
            parameters.Add("@TaxPaybleInPercentage", model.TaxPaybleInPercentage);
            parameters.Add("@TaxPaybleInAmount", model.TaxPaybleInAmount);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@CreatedBy", model.CreatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.DetailYearlyItTableMaster); // Cast enum to int
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
        public async Task<DetailYearlyItTableMaster> UpdateAsync(string procedureName, DetailYearlyItTableMaster model)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@YearlyItTableDetail_Id", model.YearlyItTableDetail_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@YearlyItTable_Id", model.YearlyItTable_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@Company_Id", model.Company_Id);
            parameters.Add("@Income_From", model.Income_From);
            parameters.Add("@Income_To", model.Income_To);
            parameters.Add("@TaxPaybleInPercentage", model.TaxPaybleInPercentage);
            parameters.Add("@TaxPaybleInAmount", model.TaxPaybleInAmount);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@UpdatedBy", model.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.DetailYearlyItTableMaster); // Cast enum to int
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
        public async Task<DetailYearlyItTableMaster> DeleteAsync(string procedureName, object detailYearlyItTableMaster)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@YearlyItTableDetail_Id", ((dynamic)detailYearlyItTableMaster).YearlyItTableDetail_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)detailYearlyItTableMaster).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.DetailYearlyItTableMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)detailYearlyItTableMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)detailYearlyItTableMaster).MessageType = result.ApplicationMessageType;
            }
            return (DetailYearlyItTableMaster)detailYearlyItTableMaster;
        }
        #endregion
    }
}
