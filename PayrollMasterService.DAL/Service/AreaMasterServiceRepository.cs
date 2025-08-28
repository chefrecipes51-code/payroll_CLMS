/****************************************************************************************************
 *                                                                                                  *
 *  Description:                                                                                    *
 *  This repository class handles CRUD operations for the AreaMaster entity.                        *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.   *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetAllAsync    : Retrieves all AreaMaster records using a stored procedure.                   *
 *  - GetByIdAsync   : Retrieves a specific AreaMaster record by ID using a stored procedure.       *
 *  - AddAsync       : Inserts a new AreaMaster record into the database using a stored procedure.  *
 *  - UpdateAsync    : Updates an existing AreaMaster record using a stored procedure.              *
 *  - DeleteAsync    : Soft-deletes an AreaMaster record using a stored procedure.                  *
 *                                                                                                  *
 *  Key Features:                                                                                   *
 *  - Implements the IAreaMasterRepository interface.                                               *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.       *
 *  - Includes application-level enums for message type, mode, and module ID.                       *
 *  - Ensures validation of returned messages and status from stored procedure execution.           *
 *                                                                                                  *
 *  Author: Priyanshu Jain                                                                          *
 *  Date  : 24-Sep-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
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
    public class AreaMasterServiceRepository : IAreaMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public AreaMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region  Area Master Crud
        public async Task<IEnumerable<AreaMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<AreaMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<AreaMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<AreaMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<AreaMaster> AddAsync(string procedureName, AreaMaster areaMaster)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@Area_Id", areaMaster.Area_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@Location_Id", areaMaster.Location_Id);
            parameters.Add("@AreaName", areaMaster.AreaName);
            parameters.Add("@IsActive", areaMaster.IsActive);
            parameters.Add("@CreatedBy", areaMaster.CreatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.AreaMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                areaMaster.StatusMessage = result.ApplicationMessage;
                areaMaster.MessageType = result.ApplicationMessageType;
            }

            return areaMaster;
        }
        public async Task<AreaMaster> UpdateAsync(string procedureName, AreaMaster areaMaster)
        {
            var parameters = new DynamicParameters();
            // Assuming wageGradeDetail is passed as the object
            parameters.Add("@Area_Id", areaMaster.Area_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@Location_Id", areaMaster.Location_Id);
            parameters.Add("@AreaName", areaMaster.AreaName);
            parameters.Add("@IsActive", areaMaster.IsActive);
            parameters.Add("@CreatedBy", areaMaster.CreatedBy);
            parameters.Add("@UpdatedBy", areaMaster.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.AreaMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                areaMaster.StatusMessage = result.ApplicationMessage;
                areaMaster.MessageType = result.ApplicationMessageType;
            }
            return areaMaster;
        }
        public async Task<AreaMaster> DeleteAsync(string procedureName, object areaMaster)
        {
            var parameters = new DynamicParameters();
            // Assuming wageGradeDetail is passed as the object
            parameters.Add("@Area_Id", ((dynamic)areaMaster).Area_Id);// 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)areaMaster).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.AreaMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)areaMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)areaMaster).MessageType = result.ApplicationMessageType;
            }
            return (AreaMaster)areaMaster;
        }
        public async Task<IEnumerable<AreaMaster>> GetAllByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QueryAsync<AreaMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        /// <summary>
        /// Created By :- Harshida Parmar
        /// Date:- 20-03-25
        /// IMP NOTE:- SP_SelectAreaMaster return TWO RESULT SET
        ///         1st RESULT SET:- Fetch all Area Details, Which is used in ORG structure AREA Tab
        ///                             (Check above method GetAllByIdAsync)
        ///         2nd RESULT SET:- Return Only Area_id And Area_Name Which used in Subsidiary Add Or Update
        ///         These method Used to get the SECOND RESULT SET. 
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AreaMaster>> GetAllAreaByLocationIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            var multi = await _dbConnection.QueryMultipleAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

            await multi.ReadAsync<AreaMaster>(); // Read and discard the first result set
            return await multi.ReadAsync<AreaMaster>(); // Read and return the second result set
        }

        #endregion
    }
}
