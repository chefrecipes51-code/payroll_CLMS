/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the FormulaMaster entity.                                  *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all FormulaMaster records using a stored procedure.                             *
 *  - GetByIdAsync   : Retrieves a specific FormulaMaster record by ID using a stored procedure.                 *
 *  - AddAsync       : Inserts a new FormulaMaster record into the database using a stored procedure.            *
 *  - UpdateAsync    : Updates an existing FormulaMaster record using a stored procedure.                        *
 *  - DeleteAsync    : Soft-deletes an FormulaMaster record using a stored procedure.                            *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the IFormulaMasterRepository interface.                                                         *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Foram Patel                                                                                         *
 *  Date  : 12-Feb-2025                                                                                           *
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
    public class FormulaMasterServiceRepository : IFormulaMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public FormulaMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #region Formula Master Crud
        public async Task<IEnumerable<FormulaMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<FormulaMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<FormulaMaster> GetByIdAsync(string procedureName, object parameters, IDbConnection _dbConnection)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<FormulaMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<FormulaMaster> AddAsync(string procedureName, FormulaMaster FormulaDetail)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Formula_Id", FormulaDetail.Formula_Id);
            parameters.Add("@Cmp_Id", FormulaDetail.Cmp_Id);//Added 17-04-25 Jira Ticket 720
            parameters.Add("@Formula_Code", FormulaDetail.Formula_Code);//Added 17-04-25 Jira Ticket 720
            parameters.Add("@FormulaName", FormulaDetail.FormulaName);
            parameters.Add("@Formula_Computation", FormulaDetail.Formula_Computation);
            parameters.Add("@IsActive", FormulaDetail.IsActive);
            parameters.Add("@CreatedBy", FormulaDetail.CreatedBy);

            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.FormulaMaster); // Cast enum to int
            parameters.Add("@SelectedformulaId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            var selectedFormulaId = parameters.Get<int?>("@SelectedformulaId");
            FormulaDetail.Formula_Id = selectedFormulaId ?? 0; // Or handle with null check if needed

            if (result != null)
            {
                FormulaDetail.StatusMessage = result.ApplicationMessage;
                FormulaDetail.MessageType = result.ApplicationMessageType;
            }
            return FormulaDetail;
        }
        public async Task<FormulaMaster> UpdateAsync(string procedureName, FormulaMaster FormulaDetail)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Formula_Id", FormulaDetail.Formula_Id);
            parameters.Add("@Cmp_Id", FormulaDetail.Cmp_Id); //Added 17-04-25 Jira Ticket 720
            parameters.Add("@Formula_Code", FormulaDetail.Formula_Code);//Added 17-04-25 Jira Ticket 720
            parameters.Add("@FormulaName", FormulaDetail.FormulaName);
            parameters.Add("@Formula_Computation", FormulaDetail.Formula_Computation);
            parameters.Add("@IsActive", FormulaDetail.IsActive);
            parameters.Add("@CreatedBy", FormulaDetail.CreatedBy);
            parameters.Add("@UpdatedBy", FormulaDetail.UpdatedBy);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.FormulaMaster); // Cast enum to int
            parameters.Add("@SelectedformulaId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                FormulaDetail.StatusMessage = result.ApplicationMessage;
                FormulaDetail.MessageType = result.ApplicationMessageType;
            }
            return FormulaDetail;
        }
        public async Task<FormulaMaster> DeleteAsync(string procedureName, object FormulaDetail)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Formula_Id", ((dynamic)FormulaDetail).Formula_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)FormulaDetail).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.FormulaMaster); // Cast enum to int
                                                                                    // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)FormulaDetail).StatusMessage = result.ApplicationMessage;
                ((dynamic)FormulaDetail).MessageType = result.ApplicationMessageType;
            }
            return (FormulaMaster)FormulaDetail;
        }
        public async Task<IEnumerable<FormulaMaster>> GetFormulaSuggestionsAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Create parameters
            return await _dbConnection.QueryAsync<FormulaMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Test DrowpDown Values With Caching
        public List<SelectListItem> GetDropdownItems()
        {
            // SQL query to fetch Formula_Id and FormulaName
            string sql = "SELECT Formula_Id, FormulaName FROM tbl_mst_formula";

            // Execute the query and map the result
            var FormulaItems = _dbConnection.Query(sql).Select(w =>
                new SelectListItem
                {
                    Value = w.Formula_Id.ToString(),  // Formula_Id as value
                    Text = w.FormulaName         // FormulaName as text
                }).ToList();

            return FormulaItems;
        }

        public async Task<FormulaMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<FormulaMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
    }


}
    #endregion


