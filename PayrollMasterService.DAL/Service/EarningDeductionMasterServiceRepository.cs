/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the EarningDeductionMaster entity.                           *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all EarningDeductionMaster records using a stored procedure.                      *
 *  - GetByIdAsync   : Retrieves a specific EarningDeductionMaster record by ID using a stored procedure.          *
 *  - AddAsync       : Inserts a new EarningDeductionMaster record into the database using a stored procedure.     *
 *  - UpdateAsync    : Updates an existing EarningDeductionMaster record using a stored procedure.                 *
 *  - DeleteAsync    : Soft-deletes an EarningDeductionMaster record using a stored procedure.                     *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the IEarningDeductionMasterRepository interface.                                                        *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Priyanshu Jain                                                                                         *
 *  Date  : 19-Sep-2024                                                                                            *
 *                                                                                                                 *
 *******************************************************************************************************************/
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Payroll.Common.EnumUtility;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace PayrollMasterService.DAL.Service
{
    public class EarningDeductionMasterServiceRepository : IEarningDeductionMasterRepository
    {
        private readonly IDbConnection _dbConnection;
        public EarningDeductionMasterServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region Earning Deduction Master Crud
        public async Task<IEnumerable<EarningDeductionMaster>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<EarningDeductionMaster>(procedureName, commandType: CommandType.StoredProcedure);
        }        
        public async Task<IEnumerable<EarningDeductionMaster>> GetAllByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<EarningDeductionMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<EarningDeductionMaster> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<EarningDeductionMaster>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<EarningDeductionMaster> AddAsync(string procedureName, EarningDeductionMaster earningDeductionMaster)
        {
            // Executing the stored procedure with the given parameters
            var Parameters = new DynamicParameters();
            //Parameters.Add("@EarningDeduction_Id", earningDeductionMaster.EarningDeduction_Id, DbType.Int32);
            Parameters.Add("@Company_Id", earningDeductionMaster.Company_Id, DbType.Int32);
            Parameters.Add("@EarningDeductionName", earningDeductionMaster.EarningDeductionName, DbType.String);
            Parameters.Add("@CalculationType", earningDeductionMaster.CalculationType, DbType.Int32);
            Parameters.Add("@EarningDeductionType", earningDeductionMaster.EarningDeductionType, DbType.Int32);
            //Parameters.Add("@Taxable", earningDeductionMaster.Taxable, DbType.Boolean);
            //Parameters.Add("@Exempted", earningDeductionMaster.Exempted, DbType.Boolean);
            //Parameters.Add("@AllowSingleEntry", earningDeductionMaster.AllowSingleEntry, DbType.Boolean);
            //Parameters.Add("@UseInSalaryRevision", earningDeductionMaster.UseInSalaryRevision, DbType.Boolean);
            //Parameters.Add("@TaxFactorPercentage", earningDeductionMaster.TaxFactorPercentage, DbType.Int32);
            Parameters.Add("@MinimumUnit_value", earningDeductionMaster.MinimumUnit_value, DbType.Decimal);
            Parameters.Add("@MaximumUnit_value", earningDeductionMaster.MaximumUnit_value, DbType.Decimal);
            Parameters.Add("@Amount", earningDeductionMaster.Amount, DbType.Decimal);
            Parameters.Add("@Formula", earningDeductionMaster.Formula, DbType.String);
            Parameters.Add("@Formula_Id", earningDeductionMaster.Formula_Id, DbType.Int64);
            //Parameters.Add("@IncludeInPension", earningDeductionMaster.IncludeInPension, DbType.Boolean);
            //Parameters.Add("@EarnDed_Id_ForUnit", earningDeductionMaster.EarnDed_Id_ForUnit, DbType.Int32);
            //Parameters.Add("@EarnDed_Id_ForRate", earningDeductionMaster.EarnDed_Id_ForRate, DbType.Int32);
            //Parameters.Add("@CostCentre_Applicable", earningDeductionMaster.CostCentre_Applicable, DbType.Boolean);
            //Parameters.Add("@Tax_In_Gross", earningDeductionMaster.Tax_In_Gross, DbType.Boolean);
            //Parameters.Add("@Amount_Type", earningDeductionMaster.Amount_Type, DbType.Int32);
            Parameters.Add("@IsActive", earningDeductionMaster.IsActive, DbType.Boolean);
            Parameters.Add("@CreatedBy", earningDeductionMaster.CreatedBy, DbType.Int32);
            //Parameters.Add("@UpdatedBy", earningDeductionMaster.UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            Parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            Parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            Parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EarningDeductionMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, Parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                earningDeductionMaster.StatusMessage = result.ApplicationMessage;
                earningDeductionMaster.MessageType = ((dynamic)result).ApplicationMessageType;
            }
            return earningDeductionMaster;
        }
        public async Task<EarningDeductionMaster> UpdateAsync(string procedureName, EarningDeductionMaster earningDeductionMaster)
        {
            // Executing the stored procedure with the given parameters
            var Parameters = new DynamicParameters();
            Parameters.Add("@EarningDeduction_Id", earningDeductionMaster.EarningDeduction_Id, DbType.Int32);
            Parameters.Add("@Company_Id", earningDeductionMaster.Company_Id, DbType.Int32);
            Parameters.Add("@EarningDeductionName", earningDeductionMaster.EarningDeductionName, DbType.String);
            Parameters.Add("@CalculationType", earningDeductionMaster.CalculationType, DbType.Int32);
            Parameters.Add("@EarningDeductionType", earningDeductionMaster.EarningDeductionType, DbType.Int32);
            //Parameters.Add("@Taxable", earningDeductionMaster.Taxable, DbType.Boolean);
            //Parameters.Add("@Exempted", earningDeductionMaster.Exempted, DbType.Boolean);
            //Parameters.Add("@AllowSingleErntry", earningDeductionMaster.AllowSingleEntry, DbType.Boolean);
            //Parameters.Add("@UseInSalaryRevision", earningDeductionMaster.UseInSalaryRevision, DbType.Boolean);
            //Parameters.Add("@TaxFactorPercentage", earningDeductionMaster.TaxFactorPercentage, DbType.Int32);
            Parameters.Add("@MinimumUnit_value", earningDeductionMaster.MinimumUnit_value, DbType.Decimal);
            Parameters.Add("@MaximumUnit_value", earningDeductionMaster.MaximumUnit_value, DbType.Decimal);
            Parameters.Add("@Amount", earningDeductionMaster.Amount, DbType.Decimal);
            Parameters.Add("@Formula", earningDeductionMaster.Formula, DbType.String);
            Parameters.Add("@Formula_Id", earningDeductionMaster.Formula_Id, DbType.Int64);
            //Parameters.Add("@IncludeInPension", earningDeductionMaster.IncludeInPension, DbType.Boolean);
            //Parameters.Add("@EarnDed_Id_ForUnit", earningDeductionMaster.EarnDed_Id_ForUnit, DbType.Int32);
            //Parameters.Add("@EarnDed_Id_ForRate", earningDeductionMaster.EarnDed_Id_ForRate, DbType.Int32);
            //Parameters.Add("@CostCentre_Applicable", earningDeductionMaster.CostCentre_Applicable, DbType.Boolean);
            //Parameters.Add("@Tax_In_Gross", earningDeductionMaster.Tax_In_Gross, DbType.Boolean);
            //Parameters.Add("@Amount_Type", earningDeductionMaster.Amount_Type, DbType.Int32);
            Parameters.Add("@IsActive", earningDeductionMaster.IsActive, DbType.Boolean);
            Parameters.Add("@CreatedBy", earningDeductionMaster.CreatedBy, DbType.Int32);
            Parameters.Add("@UpdatedBy", earningDeductionMaster.UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            Parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            Parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            Parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EarningDeductionMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, Parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                earningDeductionMaster.StatusMessage = result.ApplicationMessage;
                earningDeductionMaster.MessageType = result.ApplicationMessageType;
            }
            return earningDeductionMaster;
        }
        public async Task<EarningDeductionMaster> DeleteAsync(string procedureName, object earningDeductionMaster)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@EarningDeduction_Id", ((dynamic)earningDeductionMaster).EarningDeduction_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)earningDeductionMaster).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.EarningDeductionMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)earningDeductionMaster).StatusMessage = result.ApplicationMessage;
                ((dynamic)earningDeductionMaster).MessageType = result.ApplicationMessageType;
            }
            return (EarningDeductionMaster)earningDeductionMaster;
        }
        #endregion
    }
}
