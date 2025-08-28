/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the MapDepartmentLocation entity.                            *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all MapDepartmentLocation records using a stored procedure.                       *
 *  - GetByIdAsync   : Retrieves a specific MapDepartmentLocation record by ID using a stored procedure.           *
 *  - AddAsync       : Inserts a new MapDepartmentLocation record into the database using a stored procedure.      *
 *  - UpdateAsync    : Updates an existing MapDepartmentLocation record using a stored procedure.                  *
 *  - DeleteAsync    : Soft-deletes an MapDepartmentLocation record using a stored procedure.                      *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the IMapDepartmentLocationRepository interface.                                                   *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Priyanshu Jain                                                                                         *
 *  Date  : 09-Oct-2024                                                                                            *
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
    public class MapDepartmentLocationServiceRepository : IMapDepartmentLocationRepository
    {
        private readonly IDbConnection _dbConnection;
        public MapDepartmentLocationServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #region Map Department Location Crud
        public async Task<IEnumerable<MapDepartmentLocation>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<MapDepartmentLocation>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<MapDepartmentLocation> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<MapDepartmentLocation>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<MapDepartmentLocation> AddAsync(string procedureName, MapDepartmentLocation mapDepartmentLocation)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@Department_Location_Id", mapDepartmentLocation.Department_Location_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@Company_Id", mapDepartmentLocation.Company_Id);
            parameters.Add("@Correspondance_ID", mapDepartmentLocation.Correspondance_ID);
            parameters.Add("@Department_Id", mapDepartmentLocation.Department_Id);
            parameters.Add("@Department_Code", null);
            parameters.Add("@Area_Id", mapDepartmentLocation.Area_Id);
            parameters.Add("@Floor_Id", mapDepartmentLocation.Floor_Id);
            parameters.Add("@IsActive", mapDepartmentLocation.IsActive);
            parameters.Add("@CreatedBy", mapDepartmentLocation.CreatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MapDepartmentLocation); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                mapDepartmentLocation.StatusMessage = result.ApplicationMessage;
                mapDepartmentLocation.MessageType = result.ApplicationMessageType;
            }
            return mapDepartmentLocation;
        }
        public async Task<MapDepartmentLocation> UpdateAsync(string procedureName, MapDepartmentLocation mapDepartmentLocation)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@Department_Location_Id", mapDepartmentLocation.Department_Location_Id);
            parameters.Add("@Company_Id", mapDepartmentLocation.Company_Id);
            parameters.Add("@Correspondance_ID", mapDepartmentLocation.Correspondance_ID);
            parameters.Add("@Department_Id", mapDepartmentLocation.Department_Id);
            parameters.Add("@Department_Code", null);
            parameters.Add("@Area_Id", mapDepartmentLocation.Area_Id);
            parameters.Add("@Floor_Id", mapDepartmentLocation.Floor_Id);
            parameters.Add("@IsActive", mapDepartmentLocation.IsActive);
            parameters.Add("@CreatedBy", mapDepartmentLocation.CreatedBy);
            parameters.Add("@UpdatedBy", mapDepartmentLocation.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MapDepartmentLocation); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                mapDepartmentLocation.StatusMessage = result.ApplicationMessage;
                mapDepartmentLocation.MessageType = result.ApplicationMessageType;
            }
            return mapDepartmentLocation;
        }
        public async Task<MapDepartmentLocation> DeleteAsync(string procedureName, object mapDepartmentLocation)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@Department_Location_Id", ((dynamic)mapDepartmentLocation).Department_Location_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)mapDepartmentLocation).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MapDepartmentLocation); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)mapDepartmentLocation).StatusMessage = result.ApplicationMessage;
                ((dynamic)mapDepartmentLocation).MessageType = result.ApplicationMessageType;
            }
            return (MapDepartmentLocation)mapDepartmentLocation;
        }
        #endregion

        #region Floor Master Get
        public async Task<IEnumerable<FloorMaster>> GetAllFloorAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<FloorMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<FloorMaster>> GetFloorByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QueryAsync<FloorMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

        }
        #endregion
    }
}
