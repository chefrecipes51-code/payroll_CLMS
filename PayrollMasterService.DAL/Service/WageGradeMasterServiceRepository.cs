/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the WageGradeMaster entity.                                  *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all WageGradeMaster records using a stored procedure.                             *
 *  - GetByIdAsync   : Retrieves a specific WageGradeMaster record by ID using a stored procedure.                 *
 *  - AddAsync       : Inserts a new WageGradeMaster record into the database using a stored procedure.            *
 *  - UpdateAsync    : Updates an existing WageGradeMaster record using a stored procedure.                        *
 *  - DeleteAsync    : Soft-deletes an WageGradeMaster record using a stored procedure.                            *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the IWageGradeMasterRepository interface.                                                         *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Priyanshu Jain                                                                                         *
 *  Date  : 13-Sep-2024                                                                                            *
 *                                                                                                                 *
 *******************************************************************************************************************/

using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.EnumUtility;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace PayrollMasterService.DAL.Service
{
    public class WageGradeMasterServiceRepository : IWageGradeMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public WageGradeMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #region Wage Grade Master Crud
        public async Task<IEnumerable<WageGradeMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<WageGradeMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<WageGradeMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<WageGradeMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<WageGradeMaster> AddAsync(string procedureName, WageGradeMaster wageGradeDetail)
        {
            var parameters = new DynamicParameters();
            // Add the parameters required for the stored procedure
            // Add the parameters required for the stored procedure
            //parameters.Add("@Wage_Id", wageGradeDetail.Wage_Id); // 0 for insert, greater than 0 for update
            //parameters.Add("@Cmp_id", wageGradeDetail.Cmp_id);
            //parameters.Add("@WageGradeCode", wageGradeDetail.WageGradeCode);
            //parameters.Add("@WageGradeName", wageGradeDetail.WageGradeName);
            //parameters.Add("@WageGradeBasic", wageGradeDetail.WageGradeBasic);
            //parameters.Add("@PaymentModeId", wageGradeDetail.PaymentModeId);
            //parameters.Add("@IsHRAapplicable", wageGradeDetail.IsHRAapplicable);
            //parameters.Add("@HRAallownceType", wageGradeDetail.HRAallownceType);
            //parameters.Add("@NotInUse", wageGradeDetail.NotInUse);

            parameters.Add("@Wage_Id", wageGradeDetail.Wage_Id);
            parameters.Add("@Cmp_id", wageGradeDetail.Cmp_id);
            parameters.Add("@WageGradeCode", wageGradeDetail.WageGradeCode);
            parameters.Add("@WageGradeName", wageGradeDetail.WageGradeName);
            parameters.Add("@Effective_StartDate", wageGradeDetail.Effective_StartDate);
            parameters.Add("@Effective_EndDate", wageGradeDetail.Effective_EndDate);
            parameters.Add("@IsActive", wageGradeDetail.IsActive);
            parameters.Add("@CreatedBy", wageGradeDetail.CreatedBy);

            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.WageConfigMaster); // Cast enum to int

            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                wageGradeDetail.StatusMessage = result.ApplicationMessage;
                wageGradeDetail.MessageType = result.ApplicationMessageType;
            }
            return wageGradeDetail;
        }
        public async Task<WageGradeMaster> UpdateAsync(string procedureName, WageGradeMaster wageGradeDetail)
        {
            var parameters = new DynamicParameters();
            // Assuming wageGradeDetail is passed as the object
            // Add the parameters required for the stored procedure
            //parameters.Add("@Wage_Id", wageGradeDetail.Wage_Id); // 0 for insert, greater than 0 for update
            //parameters.Add("@Cmp_id", wageGradeDetail.Cmp_id);
            //parameters.Add("@WageGradeCode", wageGradeDetail.WageGradeCode);
            //parameters.Add("@WageGradeName", wageGradeDetail.WageGradeName);
            //parameters.Add("@WageGradeBasic", wageGradeDetail.WageGradeBasic);
            //parameters.Add("@PaymentModeId", wageGradeDetail.PaymentModeId);
            //parameters.Add("@IsHRAapplicable", wageGradeDetail.IsHRAapplicable);
            //parameters.Add("@HRAallownceType", wageGradeDetail.HRAallownceType);
            //parameters.Add("@NotInUse", wageGradeDetail.NotInUse);

            parameters.Add("@Wage_Id", wageGradeDetail.Wage_Id);
            parameters.Add("@Cmp_id", wageGradeDetail.Cmp_id);
            parameters.Add("@WageGradeCode", wageGradeDetail.WageGradeCode);
            parameters.Add("@WageGradeName", wageGradeDetail.WageGradeName);
            parameters.Add("@Effective_StartDate", wageGradeDetail.Effective_StartDate);
            parameters.Add("@Effective_EndDate", wageGradeDetail.Effective_EndDate);
            parameters.Add("@IsActive", wageGradeDetail.IsActive);
            parameters.Add("@CreatedBy", wageGradeDetail.CreatedBy);
            parameters.Add("@UpdatedBy", wageGradeDetail.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.WageConfigMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                wageGradeDetail.StatusMessage = result.ApplicationMessage;
                wageGradeDetail.MessageType = result.ApplicationMessageType;
            }
            return wageGradeDetail;
        }
        public async Task<WageGradeMaster> DeleteAsync(string procedureName, object wageGradeDetail)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Wage_Id", ((dynamic)wageGradeDetail).Wage_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)wageGradeDetail).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.WageConfigMaster); // Cast enum to int
                                                                                      // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)wageGradeDetail).StatusMessage = result.ApplicationMessage;
                ((dynamic)wageGradeDetail).MessageType = result.ApplicationMessageType;
            }
            return (WageGradeMaster)wageGradeDetail;
        }
        #endregion

        #region Test DrowpDown Values With Caching
        public List<SelectListItem> GetDropdownItems()
        {
            // SQL query to fetch Wage_Id and WageGradeCode
            string sql = "SELECT Wage_Id, WageGradeCode FROM tbl_mst_wagegrade";

            // Execute the query and map the result
            var wageItems = _dbConnection.Query(sql).Select(w =>
                new SelectListItem
                {
                    Value = w.Wage_Id.ToString(),  // Wage_Id as value
                    Text = w.WageGradeCode         // WageGradeCode as text
                }).ToList();

            return wageItems;
        }
        #endregion
    }
}
