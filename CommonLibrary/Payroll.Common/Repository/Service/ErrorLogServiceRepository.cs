/****************************************************************************************************
 *                                                                                                  *
 *  Description:                                                                                    *
 *  This repository class handles CRUD operations for the ErrorLogMaster entity.                    *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.   *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - ErrorLogAsync  : Inserts a new Exception record into the database using a stored procedure.   *
 *                                                                                                  *
 *  Key Features:                                                                                   *
 *  - Implements the IErrorLogRepository interface.                                                 *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.       *
 *  - Includes application-level enums for message type, mode, and module ID.                       *
 *  - Ensures validation of returned messages and status from stored procedure execution.           *
 *                                                                                                  *
 *  Author: Priyanshu Jain                                                                          *
 *  Date  : 18-Sep-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.CommonDto;
using Payroll.Common.Repository.Interface;
using Payroll.Common.EnumUtility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using static Payroll.Common.EnumUtility.EnumUtility;
using Payroll.Common.ApplicationModel;

namespace Payroll.Common.Repository.Service
{
    /// <summary>
    ///  Developer Name :- Priyanshi Jain
    ///  Message detail :- Error Log service for stored error message in database.
    ///  Created Date   :- 30-Sep-2024
    ///  Last Modified  :- 30-Sep-2024
    ///  Modification   :- None
    /// </summary>
    /// <returns></returns>
    public class ErrorLogServiceRepository : IErrorLogRepository
    {
        private readonly DapperContext.DapperContext _dbConnection;

        public ErrorLogServiceRepository()
        {
            _dbConnection = DapperContext.DapperContext.GetInstance();
        }
        public async Task ErrorLogAsync(Exception exception, HttpContext context)
        {
            using var con = _dbConnection.CreateConnection();

            // Determine the severity level based on exception type
            int severityLevel = (exception is SqlException sqlException)
                ? sqlException.Class // SQL severity level (based on SQL exception)
                : (int)LogLevel.Error; // Application severity level for general code errors

            int logType = (exception is SqlException)
                        ? 2  // SQL severity level
                        : 1; // Code error severity level

            var parameters = new DynamicParameters();
            parameters.Add("@LogType", logType, DbType.Int32);  // LogType can be dynamic or fixed
            parameters.Add("@LogMessage", exception.Message, DbType.String);
            parameters.Add("@SeverityLevel", severityLevel, DbType.Int32);
            parameters.Add("@ErrorCode", exception.HResult, DbType.Int32);
            parameters.Add("@ErrorLine", new StackTrace(exception, true)?.GetFrame(0)?.GetFileLineNumber() ?? 0, DbType.Int32);
            parameters.Add("@Source", exception.Source, DbType.String);
            parameters.Add("@CreatedDate", DateTime.Now, DbType.DateTime);
            parameters.Add("@UserId", context.Session.GetInt32("UserId") ?? 0, DbType.Int32);
            parameters.Add("@StackTrace", exception.StackTrace, DbType.String);
            parameters.Add("@ActionName", context.Request.RouteValues["action"]?.ToString(), DbType.String);
            parameters.Add("@ControllerName", context.Request.RouteValues["controller"]?.ToString(), DbType.String);
            parameters.Add("@UserAgent", context.Request.Headers["User-Agent"].ToString(), DbType.String);

            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)ApplicationMessageTypeEnum.Error); // Cast enum to int
            parameters.Add("@MessageMode", (int)ApplicationMessageModeEnum.Insert); // Cast enum to int


            // Output parameters
            parameters.Add("@outputMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: -1);
            parameters.Add("@StatusMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 100);
            // Execute stored procedure asynchronously
            await con.ExecuteAsync(DbConstants.AddErrorMaster, parameters, commandType: CommandType.StoredProcedure);

            // Retrieve output parameter values
            string outputMsg = parameters.Get<string>("@outputMsg");
            string statusMessage = parameters.Get<string>("@StatusMessage");

        }
    }
}
