/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the LocationMaster entity.                                   *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all LocationMaster records using a stored procedure.                              *
 *  - GetByIdAsync   : Retrieves a specific LocationMaster record by ID using a stored procedure.                  *
 *  - AddAsync       : Inserts a new LocationMaster record into the database using a stored procedure.             *
 *  - UpdateAsync    : Updates an existing LocationMaster record using a stored procedure.                         *
 *  - DeleteAsync    : Soft-deletes an LocationMaster record using a stored procedure.                             *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the ILocationMasterRepository interface.                                                          *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Priyanshu Jain                                                                                         *
 *  Date  : 24-Sep-2024                                                                                            *
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
    public class LocationMasterServiceRepository : ILocationMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public LocationMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region  Location Master Crud
        public async Task<IEnumerable<LocationMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<LocationMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<LocationMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<LocationMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<LocationMaster> AddAsync(string procedureName, LocationMaster locationMaster)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@Location_Id", locationMaster.Location_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@CityId", locationMaster.CityId);
            parameters.Add("@LocationName", locationMaster.LocationName);
            parameters.Add("@IsActive", locationMaster.IsActive);
            parameters.Add("@CreatedBy", locationMaster.CreatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.LocationMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                locationMaster.StatusMessage = result.ApplicationMessage;
                locationMaster.MessageType = result.ApplicationMessageType;
            }
            return locationMaster;
        }
        public async Task<LocationMaster> UpdateAsync(string procedureName, LocationMaster locationMaster)
        {
            var parameters = new DynamicParameters();
            // Assuming wageGradeDetail is passed as the object
            parameters.Add("@Location_Id", locationMaster.Location_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@CityId", locationMaster.CityId);
            parameters.Add("@LocationName", locationMaster.LocationName);
            parameters.Add("@IsActive", locationMaster.IsActive);
            parameters.Add("@CreatedBy", locationMaster.CreatedBy);
            parameters.Add("@UpdatedBy", locationMaster.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.LocationMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                locationMaster.StatusMessage = result.ApplicationMessage;
                locationMaster.MessageType = result.ApplicationMessageType;
            }
            return locationMaster;
        }
        public async Task<LocationMaster> DeleteAsync(string procedureName, object locationMaster)
        {
            var parameters = new DynamicParameters();
            // Assuming wageGradeDetail is passed as the object
            parameters.Add("@Location_Id", ((dynamic)locationMaster).Location_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)locationMaster).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.LocationMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)locationMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)locationMaster).MessageType = result.ApplicationMessageType;
            }
            return (LocationMaster)locationMaster;
        }
        #endregion
    }
}
