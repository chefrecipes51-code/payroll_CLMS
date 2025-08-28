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
    public class EventAuthMappingRepository : IEventAuthMappingRepository
    {
        private readonly IDbConnection _dbConnection;
        public EventAuthMappingRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region EventAuthSetUp Endpoint Handlers (CRUD)
        #region EventAuthSetUp Add

          
        public async Task<EventAuthSetUp> AddNewAsync(string storedProcedure, EventAuthSetUp eventAuthSetUp)
        {
            using (var connection = _dbConnection)  // _dbConnection is your database connection
            {
                // Create and populate a DataTable for the Event Authentication SetUp
                var eventAuthDetailTable = new DataTable();
                eventAuthDetailTable.Columns.Add("Event_Id", typeof(int));  
                eventAuthDetailTable.Columns.Add("Role_Id", typeof(int));  
                eventAuthDetailTable.Columns.Add("Auth_Level", typeof(int));  
                eventAuthDetailTable.Columns.Add("Approver_Id", typeof(int));  
                eventAuthDetailTable.Columns.Add("IsActive", typeof(bool));  
                        
                // Populate the DataTable with Event Authentication Setup records
                foreach (var detail in eventAuthSetUp.eventAuthDetails)
                {
                    eventAuthDetailTable.Rows.Add(
                        detail.Event_Id,
                        detail.Role_Id,
                        detail.Auth_Level,
                        detail.Approver_Id,
                        detail.IsActive
                    );
                }

                // Stored Procedure parameters.
                var parameters = new DynamicParameters();
                parameters.Add("@EventAuthSetup", eventAuthDetailTable.AsTableValuedParameter("dbo.udt_trn_eventauth_setup"));
                parameters.Add("@CreatedBy", eventAuthSetUp.CreatedBy);
                parameters.Add("@UpdatedBy", eventAuthSetUp.UpdatedBy);
                parameters.Add("@CountryId", eventAuthSetUp.CountryId);

                parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
                parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
                parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EventAuthSetUp); // Cast enum to int

                try
                {
                    var result = await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {

                }


                return eventAuthSetUp;
            }
        }
     

          
        public async Task<IEnumerable<EventAuthSelect>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<EventAuthSelect>(procedureName, commandType: CommandType.StoredProcedure);

        }

         
        public async Task<EventAuthSelect> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<EventAuthSelect>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<EventAuthSelect>> GetByFilterAttributesAsync(string procedureName, object parameters)
        {
            try
            {
                var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
                return await _dbConnection.QueryAsync<EventAuthSelect>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {

                throw;
            }
       }
        
        public Task<EventAuthSelect> AddAsync(string procedureName, EventAuthSelect model)
        {
            throw new NotImplementedException();
        }
        #endregion


        public Task<EventAuthSelect> DeleteAsync(string procedureName, object parameters)
        {
            throw new NotImplementedException();
        }
        public Task<EventAuthSelect> UpdateAsync(string procedureName, EventAuthSelect model)
        {
            throw new NotImplementedException();
        }

        #endregion
      
    }
}
