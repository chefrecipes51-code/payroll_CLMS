/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the EventMaster entity.                                      *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all EventMaster records using a stored procedure.                                 *
 *  - GetByIdAsync   : Retrieves a specific EventMaster record by ID using a stored procedure.                     *
 *  - AddAsync       : Inserts a new EventMaster record into the database using a stored procedure.                *
 *  - UpdateAsync    : Updates an existing EventMaster record using a stored procedure.                            *
 *  - DeleteAsync    : Soft-deletes an EventMaster record using a stored procedure.                                *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the IEventMasterRepository interface.                                                             *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Priyanshu Jain                                                                                         *
 *  Date  : 12-Nov-2024                                                                                            *
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
    public class EventMasterServiceRepository : IEventMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public EventMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<EventMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<EventMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<EventMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<EventMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<EventMaster> AddAsync(string procedureName, EventMaster model)
        {
            var parameters = new
            {
                Event_Id = model.Event_Id,
                EventName = model.EventName,
                Module_Id = model.Module_Id,
                Company_Id = model.Company_Id,
                EventTable = model.EventTable,
                AutoUpdate = model.AutoUpdate,
                AuthRequired = model.AuthRequired,
                AuthLevel = model.AuthLevel,
                CanDeligate = model.CanDeligate,
                Send_Email_For_Auth = model.Send_Email_For_Auth,
                Send_Email_Post_Auth = model.Send_Email_Post_Auth,
                SendNotification = model.SendNotification,
                KeyField1 = model.KeyField1,
                UpdateField1 = model.UpdateField1,
                UpdateField2 = model.UpdateField2,
                UpdateField3 = model.UpdateField3,
                EventBehaviour = model.EventBehaviour,
                IsActive = model.IsActive,
                CreatedBy = model.CreatedBy,
                Messagetype = (int)EnumUtility.ApplicationMessageTypeEnum.Information,
                MessageMode = (int)EnumUtility.ApplicationMessageModeEnum.Insert,
                ModuleId = (int)EnumUtility.ModuleEnum.EventMaster
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
        public async Task<EventMaster> UpdateAsync(string procedureName, EventMaster model)
        {
            var parameters = new
            {
                Event_Id = model.Event_Id,
                EventName = model.EventName,
                Module_Id = model.Module_Id,
                Company_Id = model.Company_Id,
                EventTable = model.EventTable,
                AutoUpdate = model.AutoUpdate,
                AuthRequired = model.AuthRequired,
                AuthLevel = model.AuthLevel,
                CanDeligate = model.CanDeligate,
                Send_Email_For_Auth = model.Send_Email_For_Auth,
                Send_Email_Post_Auth = model.Send_Email_Post_Auth,
                SendNotification = model.SendNotification,
                KeyField1 = model.KeyField1,
                UpdateField1 = model.UpdateField1,
                UpdateField2 = model.UpdateField2,
                UpdateField3 = model.UpdateField3,
                EventBehaviour = model.EventBehaviour,
                IsActive = model.IsActive,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy,
                Messagetype = (int)EnumUtility.ApplicationMessageTypeEnum.Information,
                MessageMode = (int)EnumUtility.ApplicationMessageModeEnum.Update,
                ModuleId = (int)EnumUtility.ModuleEnum.EventMaster
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
        public async Task<EventMaster> DeleteAsync(string procedureName, object eventMaster)
        {
            var parameters = new DynamicParameters();
            // Assuming wageGradeDetail is passed as the object
            parameters.Add("@Event_Id", ((dynamic)eventMaster).Event_Id);// 0 for insert, greater than 0 for update
            parameters.Add("@UpdatedBy", ((dynamic)eventMaster).UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EventMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)eventMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)eventMaster).MessageType = result.ApplicationMessageType;
            }
            return (EventMaster)eventMaster;
        }
    }
}
