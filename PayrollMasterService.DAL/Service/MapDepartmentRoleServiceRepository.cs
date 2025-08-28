/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the MapDepartmentRole entity.                                *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all MapDepartmentRole records using a stored procedure.                           *
 *  - GetByIdAsync   : Retrieves a specific MapDepartmentRole record by ID using a stored procedure.               *
 *  - AddAsync       : Inserts a new MapDepartmentRole record into the database using a stored procedure.          *
 *  - UpdateAsync    : Updates an existing MapDepartmentRole record using a stored procedure.                      *
 *  - DeleteAsync    : Soft-deletes an MapDepartmentRole record using a stored procedure.                          *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the IMapDepartmentRoleRepository interface.                                                       *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Priyanshu Jain                                                                                         *
 *  Date  : 08-Nov-2024                                                                                            *
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
    public class MapDepartmentRoleServiceRepository : IMapDepartmentRoleRepository
    {
        private readonly IDbConnection _dbConnection;
        public MapDepartmentRoleServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #region Map Department Role Crud
        public async Task<IEnumerable<MapDepartmentRole>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<MapDepartmentRole>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<MapDepartmentRole> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<MapDepartmentRole>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<MapDepartmentRole> AddAsync(string procedureName, MapDepartmentRole mapDepartmentRole)
        {
            var parameters = new
            {
                RoleDepartment_Id = mapDepartmentRole.RoleDepartment_Id,
                Role_Id = mapDepartmentRole.Role_Id,
                Department_Id = mapDepartmentRole.Department_Id,
                Effective_From_Dt = mapDepartmentRole.Effective_From_Dt,
                IsActive = mapDepartmentRole.IsActive,
                CreatedBy = mapDepartmentRole.CreatedBy,
                Messagetype = (int)EnumUtility.ApplicationMessageTypeEnum.Information,
                MessageMode = (int)EnumUtility.ApplicationMessageModeEnum.Insert,
                ModuleId = (int)EnumUtility.ModuleEnum.MapDepartmentRole
            };
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                mapDepartmentRole.StatusMessage = result.ApplicationMessage;
                mapDepartmentRole.MessageType = result.ApplicationMessageType;
            }
            return mapDepartmentRole;
        }
        public async Task<MapDepartmentRole> UpdateAsync(string procedureName, MapDepartmentRole mapDepartmentRole)
        {
            var parameters = new
            {
                RoleDepartment_Id = mapDepartmentRole.RoleDepartment_Id,
                Role_Id = mapDepartmentRole.Role_Id,
                Department_Id = mapDepartmentRole.Department_Id,
                Effective_From_Dt = mapDepartmentRole.Effective_From_Dt,
                IsActive = mapDepartmentRole.IsActive,
                CreatedBy = mapDepartmentRole.CreatedBy,
                UpdatedBy = mapDepartmentRole.UpdatedBy,
                Messagetype = (int)EnumUtility.ApplicationMessageTypeEnum.Information,
                MessageMode = (int)EnumUtility.ApplicationMessageModeEnum.Update,
                ModuleId = (int)EnumUtility.ModuleEnum.MapDepartmentRole
            };
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                mapDepartmentRole.StatusMessage = result.ApplicationMessage;
                mapDepartmentRole.MessageType = result.ApplicationMessageType;
            }
            return mapDepartmentRole;
        }
        public async Task<MapDepartmentRole> DeleteAsync(string procedureName, object mapDepartmentRole)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            parameters.Add("@RoleDepartment_Id", ((dynamic)mapDepartmentRole).RoleDepartment_Id); // 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)mapDepartmentRole).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.MapDepartmentRole); // Cast enum to int
            //Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)mapDepartmentRole).StatusMessage = result.ApplicationMessage;
                ((dynamic)mapDepartmentRole).MessageType = result.ApplicationMessageType;
            }
            return (MapDepartmentRole)mapDepartmentRole;
        }
        #endregion
    }
}
