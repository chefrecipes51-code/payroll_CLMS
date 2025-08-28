/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the RoleMaster entity.                                       *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all RoleMaster records using a stored procedure.                                  *
 *  - GetByIdAsync   : Retrieves a specific RoleMaster record by ID using a stored procedure.                      *
 *  - AddAsync       : Inserts a new RoleMaster record into the database using a stored procedure.                 *
 *  - UpdateAsync    : Updates an existing RoleMaster record using a stored procedure.                             *
 *  - DeleteAsync    : Soft-deletes an RoleMaster record using a stored procedure.                                 *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the IRoleMasterRepository interface.                                                              *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Priyanshu Jain                                                                                         *
 *  Date  : 24-Oct-2024                                                                                            *
 *                                                                                                                 *
 *******************************************************************************************************************/
using Dapper;
using Payroll.Common.EnumUtility;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Service
{
    public class RoleMasterServiceRepository : IRoleMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public RoleMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #region  Role Master Crud
        public async Task<IEnumerable<RoleMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<RoleMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<RoleMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<RoleMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<RoleMaster> AddAsync(string procedureName, RoleMaster model)
        {
            var parameters = new
            {
                Role_Id = model.Role_Id,
                RoleName = model.RoleName,
                GrantAdd = model.GrantAdd,
                GrantEdit = model.GrantEdit,
                GrantView = model.GrantView,
                GrantDelete = model.GrantDelete,
                GrantApprove = model.GrantApprove,
                GrantRptPrint = model.GrantRptPrint,
                GrantRptDownload = model.GrantRptDownload,
                OrgSequence = model.OrgSequence,
                Import = model.Import,
                DocDownload = model.DocDownload,
                DocUpload = model.DocUpload,
                IsActive = model.IsActive,
                CreatedBy = model.CreatedBy,
                Messagetype = (int)EnumUtility.ApplicationMessageTypeEnum.Information,
                MessageMode = (int)EnumUtility.ApplicationMessageModeEnum.Insert,
                ModuleId = (int)EnumUtility.ModuleEnum.RoleMaster
            };
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
        public async Task<RoleMaster> UpdateAsync(string procedureName, RoleMaster model)
        {
            var parameters = new
            {
                Role_Id = model.Role_Id, //Assuming RoleId is a property of RoleMaster
                RoleName = model.RoleName,
                GrantAdd = model.GrantAdd,
                GrantEdit = model.GrantEdit,
                GrantView = model.GrantView,
                GrantDelete = model.GrantDelete,
                GrantApprove = model.GrantApprove,
                GrantRptPrint = model.GrantRptPrint,
                GrantRptDownload = model.GrantRptDownload,
                OrgSequence = model.OrgSequence,
                Import = model.Import,
                DocDownload = model.DocDownload,
                DocUpload = model.DocUpload,
                IsActive = model.IsActive,
                CreatedBy = model.CreatedBy, // Assuming CreatedBy is a property of RoleMaster
                UpdatedBy = model.UpdatedBy, // Assuming UpdatedBy is a property of RoleMaster
                Messagetype = (int)EnumUtility.ApplicationMessageTypeEnum.Information,
                MessageMode = (int)EnumUtility.ApplicationMessageModeEnum.Update,
                ModuleId = (int)EnumUtility.ModuleEnum.RoleMaster
            };
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
        public async Task<RoleMaster> DeleteAsync(string procedureName, object roleMaster)
        {
            var parameters = new DynamicParameters();
            // Assuming wageGradeDetail is passed as the object
            parameters.Add("@Role_Id", ((dynamic)roleMaster).Role_Id);// 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)roleMaster).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.AreaMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)roleMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)roleMaster).MessageType = result.ApplicationMessageType;
            }
            return (RoleMaster)roleMaster;
        }
        #endregion
    }
}


