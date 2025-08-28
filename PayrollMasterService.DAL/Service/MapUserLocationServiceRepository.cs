/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the MapUserLocation entity.                                  *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all MapUserLocation records using a stored procedure.                             *
 *  - GetByIdAsync   : Retrieves a specific MapUserLocation record by ID using a stored procedure.                 *
 *  - AddAsync       : Inserts a new MapUserLocation record into the database using a stored procedure.            *
 *  - UpdateAsync    : Updates an existing MapUserLocation record using a stored procedure.                        *
 *  - DeleteAsync    : Soft-deletes an MapUserLocation record using a stored procedure.                            *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the IMapUserLocationRepository interface.                                                         *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Priyanshu Jain                                                                                         *
 *  Date  : 06-Nov-2024                                                                                            *
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
    public class MapUserLocationServiceRepository : IMapUserLocationRepository
    {
        private readonly IDbConnection _dbConnection;
        public MapUserLocationServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #region Map User Location Crud
        public async Task<IEnumerable<MapUserLocation>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<MapUserLocation>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<MapUserLocation> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<MapUserLocation>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<MapUserLocation> AddAsync(string procedureName, MapUserLocation model)
        {
            var parameters = new DynamicParameters();
            // Add UDT parameter
            var userMapLocationDataTable = new DataTable();
            userMapLocationDataTable.Columns.Add("UserMapLocation_Id", typeof(int));
            userMapLocationDataTable.Columns.Add("User_ID", typeof(int));
            userMapLocationDataTable.Columns.Add("Company_Id", typeof(byte));  // Corrected to byte
            userMapLocationDataTable.Columns.Add("Correspondance_ID", typeof(int));
            userMapLocationDataTable.Columns.Add("IsUserMapToLowLevel", typeof(bool));
            userMapLocationDataTable.Columns.Add("IsActive", typeof(bool));
            userMapLocationDataTable.Columns.Add("IsDeleted", typeof(bool));
            // Populate UDT with the data from model
            foreach (var location in model.UserMapLocations)
            {
                userMapLocationDataTable.Rows.Add(
                    0,  // Allow for DBNull if UserMapLocation_Id is 0 (new entry)
                    location.User_ID,
                    location.Company_Id,
                    location.Correspondance_ID,
                    location.IsUserMapToLowLevel,
                    location.IsActive,
                    location.IsDeleted
                );
            }
            // Add parameters for the stored procedure
            parameters.Add("@UserMapLocation_Id", userMapLocationDataTable.AsTableValuedParameter("dbo.udt_map_userlocation"));
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MapUserLocation);
            // Execute the stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Map the output values
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }
            return model;
        }
        public async Task<MapUserLocation> UpdateAsync(string procedureName, MapUserLocation model)
        {
            var parameters = new DynamicParameters();
            // Add UDT parameter
            var userMapLocationDataTable = new DataTable();
            userMapLocationDataTable.Columns.Add("UserMapLocation_Id", typeof(int));
            userMapLocationDataTable.Columns.Add("User_ID", typeof(int));
            userMapLocationDataTable.Columns.Add("Company_Id", typeof(byte));  // Corrected to byte
            userMapLocationDataTable.Columns.Add("Correspondance_ID", typeof(int));
            userMapLocationDataTable.Columns.Add("IsUserMapToLowLevel", typeof(bool));
            userMapLocationDataTable.Columns.Add("IsActive", typeof(bool));
            userMapLocationDataTable.Columns.Add("IsDeleted", typeof(bool));
            // Populate UDT with the data from model
            foreach (var location in model.UserMapLocations)
            {
                userMapLocationDataTable.Rows.Add(
                    location.UserMapLocation_Id,
                    location.User_ID,
                    location.Company_Id,
                    location.Correspondance_ID,
                    location.IsUserMapToLowLevel,
                    location.IsActive,
                    location.IsDeleted
                );
            }
            // Add parameters for the stored procedure
            parameters.Add("@UserMapLocation_Id", userMapLocationDataTable.AsTableValuedParameter("dbo.udt_map_userlocation"));
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@UpdatedBy", model.UpdatedBy);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MapUserLocation);
            // Execute the stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Map the output values
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }
            return model;
        }
        public async Task<MapUserLocation> DeleteAsync(string procedureName, object mapUserLocation)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@UserMapLocation_Id", ((dynamic)mapUserLocation).UserMapLocation_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)mapUserLocation).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MapUserLocation); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)mapUserLocation).StatusMessage = result.ApplicationMessage;
                ((dynamic)mapUserLocation).MessageType = result.ApplicationMessageType;
            }
            return (MapUserLocation)mapUserLocation;
        }
        #endregion
    }
}
