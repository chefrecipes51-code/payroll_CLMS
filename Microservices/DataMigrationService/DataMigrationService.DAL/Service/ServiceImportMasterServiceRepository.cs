/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-239                                                                  *
 *  Description:                                                                                    *
 *  This repository class handles operations for the ServiceImportMaster.                           *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.   *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - GetServiceImportAsync   : Retrieves a specific ServiceImportMaster and UploadTemplateMaster   *
 *                              record by ID using a stored procedure.                              *
 *                                                                                                  *
 *  Key Features:                                                                                   *
 *  - Implements the IServiceImportMasterRepository interface.                                      *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.       *
 *  - Includes application-level enums for message type, mode, and module ID.                       *
 *  - Ensures validation of returned messages and status from stored procedure execution.           *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 03-Dec-2024        
 *  Updated By : Abhisehk Yadav
 *  Updated On : 03-APR-2025*
 *  Jira ticket : 638
 *                                                                                                  *
 ****************************************************************************************************/
using Dapper;
using DataMigrationService.BAL.Models;
using DataMigrationService.DAL.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataMigrationService.DAL.Service
{
    public class ServiceImportMasterServiceRepository : IServiceImportMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _configuration;
        public ServiceImportMasterServiceRepository(IDbConnection dbConnection, IConfiguration configuration)
        {
            //_dbConnection = dbConnection;
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        #region Service Import Master Method 
        public async Task<IEnumerable<object>> GetServiceImportAsync(string procedureName, ServiceImportMaster model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ServiceType", model.ServiceType, DbType.Int32);
            parameters.Add("@Service_Id", model.Service_Id > 0 ? model.Service_Id : null, DbType.Int32);
            parameters.Add("@IsActive", model.IsActive, DbType.Boolean);

            if (model.Service_Id == 0 || model.Service_Id == null)
            {
                // Fetch services when Service_Id is null or zero
                var queryResult = await _dbConnection.QueryAsync<ServiceImportMaster>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return queryResult;
            }
            else
            {
                // Fetch templates when Service_Id is provided
                var queryResult = await _dbConnection.QueryAsync<UploadTemplateMaster>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return queryResult;
            }
        }
        #endregion

        #region Service Master Method 
        public async Task<IEnumerable<object>> GetServiceAsync(string procedureName, ServiceMaster model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ModuleID", model.ModuleID > 0 ? model.ModuleID : null, DbType.Int32);

            if (model.ModuleID == 0 || model.ModuleID == null)
            {
                // Fetch services when Service_Id is null or zero
                var queryResult = await _dbConnection.QueryAsync<ServiceMaster>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return queryResult;
            }
            else
            {
                // Fetch templates when Service_Id is provided
                var queryResult = await _dbConnection.QueryAsync<UploadTemplateMaster>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return queryResult;
            }
        }
        #endregion

        public async Task<IEnumerable<object>> AddExecuteAsync(string storedProcedure, DynamicParameters parameters)
        {
            return await _dbConnection.QueryAsync<object>(
            storedProcedure,
            parameters,
            commandType: CommandType.StoredProcedure);

        }

        public async Task<List<Dictionary<string, object>>> GetReturnedDataAsync(string procedureName, object parameters)
        {
            // Using Dapper to execute stored procedure and get dynamic results
            var result = await _dbConnection.QueryAsync<dynamic>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Convert each dynamic row into a Dictionary<string, object>
            return result.Select(row => (IDictionary<string, object>)row)
                         .Select(dict => new Dictionary<string, object>(dict))
                         .ToList();
        }

        public async Task<(List<Dictionary<string, object>> data, int returnCount)> GetServiceDataAsync(string procedureName, object parameters)
        {
            var multi = await _dbConnection.QueryMultipleAsync(
                      procedureName,
                      parameters,
                      commandType: CommandType.StoredProcedure
                      );

            // Read the first result set as dynamic rows
            var data = (await multi.ReadAsync())
                       .Select(row => (IDictionary<string, object>)row)
                       .Select(dict => new Dictionary<string, object>(dict))
                       .ToList();

            // Read the second result set for the return count
            int returnCount = await multi.ReadFirstAsync<int>();

            return (data, returnCount);


        }

    }
}
