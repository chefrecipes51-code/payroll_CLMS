/**********************************************************************************************************
 *  Jira Task Ticket : PAYROLL-96,203                                                                                                            *
 *  Description:                                                                                          *
 *  This repository class handles CRUD operations for the DepartmentMaster entity.                        *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.         *
 *                                                                                                        *
 *  Methods:                                                                                              *
 *  - GetAllAsync    : Retrieves all DepartmentMaster records using a stored procedure.                   *
 *  - GetByIdAsync   : Retrieves a specific DepartmentMaster record by ID using a stored procedure.       *
 *  - AddAsync       : Inserts a new DepartmentMaster record into the database using a stored procedure.  *
 *  - UpdateAsync    : Updates an existing DepartmentMaster record using a stored procedure.              *
 *  - DeleteAsync    : Soft-deletes an DepartmentMaster record using a stored procedure.                  *
 *                                                                                                        *
 *  Key Features:                                                                                         *
 *  - Implements the IDepartmentMasterRepository interface.                                               *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.             *
 *  - Includes application-level enums for message type, mode, and module ID.                             *
 *  - Ensures validation of returned messages and status from stored procedure execution.                 *
 *                                                                                                        *
 *  Author: Priyanshu Jain                                                                                *
 *  Date  : 03-Oct-2024                                                                                   *
 *                                                                                                        *
 **********************************************************************************************************/
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
    public class DepartmentMasterServiceRepositroy : IDepartmentMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public DepartmentMasterServiceRepositroy(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region Department Master Crud
        public async Task<IEnumerable<DepartmentMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<DepartmentMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<DepartmentMaster> GetByIdAsync(string procedureName, object parameters)
        { 
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<DepartmentMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<DepartmentMaster> AddAsync(string procedureName, DepartmentMaster departmentMaster)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@Department_Id", departmentMaster.Department_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@DepartmentCode", departmentMaster.DepartmentCode); // 0 for insert, greater than 0 for update
            parameters.Add("@DepartmentName", departmentMaster.DepartmentName);
            parameters.Add("@IsActive", departmentMaster.IsActive);
            parameters.Add("@CreatedBy", departmentMaster.CreatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.DepartmentMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                departmentMaster.StatusMessage = result.ApplicationMessage;
                departmentMaster.MessageType = result.ApplicationMessageType;
            }
            return departmentMaster;    
        }
        public async Task<DepartmentMaster> UpdateAsync(string procedureName, DepartmentMaster departmentMaster)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@Department_Id", departmentMaster.Department_Id);
            parameters.Add("@DepartmentCode", departmentMaster.DepartmentCode);
            parameters.Add("@DepartmentName", departmentMaster.DepartmentName);
            parameters.Add("@IsActive", departmentMaster.IsActive);
            parameters.Add("@CreatedBy", departmentMaster.CreatedBy);
            parameters.Add("@UpdatedBy", departmentMaster.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.DepartmentMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                departmentMaster.StatusMessage = result.ApplicationMessage;
                departmentMaster.MessageType = result.ApplicationMessageType;
            }
            return departmentMaster;
        }
        public async Task<DepartmentMaster> DeleteAsync(string procedureName, object departmentMaster)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@Department_Id", ((dynamic)departmentMaster).Department_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)departmentMaster).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.DepartmentMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)departmentMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)departmentMaster).MessageType = result.ApplicationMessageType;
            }
            return (DepartmentMaster)departmentMaster;
        }
        public async Task<IEnumerable<DepartmentMaster>> GetAllByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QueryAsync<DepartmentMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }

        #endregion
    }
}
