/*******************************************************************************************************************
 *                                                                                                                 *
 *  Description:                                                                                                   *
 *  This repository class handles CRUD operations for the WageConfigMaster entity.                                  *
 *  It uses the Dapper library for efficient database interaction and stored procedure execution.                  *
 *                                                                                                                 *
 *  Methods:                                                                                                       *
 *  - GetAllAsync    : Retrieves all WageConfigMaster records using a stored procedure.                             *
 *  - GetByIdAsync   : Retrieves a specific WageConfigMaster record by ID using a stored procedure.                 *
 *  - AddAsync       : Inserts a new WageConfigMaster record into the database using a stored procedure.            *
 *  - UpdateAsync    : Updates an existing WageConfigMaster record using a stored procedure.                        *
 *  - DeleteAsync    : Soft-deletes an WageConfigMaster record using a stored procedure.                            *
 *                                                                                                                 *
 *  Key Features:                                                                                                  *
 *  - Implements the IWageConfigMasterRepository interface.                                                         *
 *  - Handles input/output parameters for stored procedures using Dapper's DynamicParameters.                      *
 *  - Includes application-level enums for message type, mode, and module ID.                                      *
 *  - Ensures validation of returned messages and status from stored procedure execution.                          *
 *                                                                                                                 *
 *  Author: Chirag Gurjar                                                                                         *
 *  Date  : 28-01-2025                                                                                            *
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
    public class WageConfigDetailServiceRepository : IWageConfigDetailRepository
    {
        private readonly IDbConnection _dbConnection;
        public WageConfigDetailServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #region Wage Grade Master Crud
        public async Task<IEnumerable<WageConfigDetail>> GetAllAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<WageConfigDetail>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<WageConfigDetail> GetByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<WageConfigDetail>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<WageConfigDetail> AddAsync(string procedureName, WageConfigDetail wageConfigDetail)
        {
            var parameters = new DynamicParameters();
      
            // Add the parameters required for the stored procedure
            parameters.Add("@WageConfig_Dtl_Id", wageConfigDetail.WageConfig_Dtl_Id);
            parameters.Add("@Wage_Id", wageConfigDetail.Wage_Id);
            parameters.Add("@WageSalaryBasic", wageConfigDetail.WageSalaryBasic);
            parameters.Add("@PaymentModeId", wageConfigDetail.PaymentModeId);
            parameters.Add("@IsHRAapplicable", wageConfigDetail.IsHRAapplicable);
            parameters.Add("@HRAallownceType", wageConfigDetail.HRAallownceType);
            parameters.Add("@HRAallowncePer", wageConfigDetail.HRAallowncePer);
            parameters.Add("@HRAallownceAmt", wageConfigDetail.HRAallownceAmt);
            parameters.Add("@Is_PFApplicable", wageConfigDetail.Is_PFApplicable);
            parameters.Add("@PF_DeductionType", wageConfigDetail.PF_DeductionType);
            parameters.Add("@PF_CalculateOn", wageConfigDetail.PF_CalculateOn);
            parameters.Add("@PF_Employee_Amt", wageConfigDetail.PF_Employee_Amt);
            parameters.Add("@PF_Employer_Amt", wageConfigDetail.PF_Employer_Amt);
            parameters.Add("@Is_PensionApplicable", wageConfigDetail.Is_PensionApplicable);
            parameters.Add("@Pension_DeductionType", wageConfigDetail.Pension_DeductionType);
            parameters.Add("@Pension_CalculateOn", wageConfigDetail.Pension_CalculateOn);
            parameters.Add("@Pension_Employee_Amt", wageConfigDetail.Pension_Employee_Amt);
            parameters.Add("@Pension_Employer_Amt", wageConfigDetail.Pension_Employer_Amt);
            parameters.Add("@Wage_Grade_Type", wageConfigDetail.Wage_Grade_Type);
            parameters.Add("@Is_HRAApplicable", wageConfigDetail.Is_HRAApplicable);
            parameters.Add("@HRA_AllowanceType", wageConfigDetail.HRA_AllowanceType);
            parameters.Add("@HRA_Percentage", wageConfigDetail.HRA_Percentage);
            parameters.Add("@HRA_Amount", wageConfigDetail.HRA_Amount);
            parameters.Add("@Earning_Dedcution_Id", wageConfigDetail.Earning_Dedcution_Id);
            parameters.Add("@NotInUse", wageConfigDetail.NotInUse);
            parameters.Add("@IsActive", wageConfigDetail.IsActive);
            parameters.Add("@CreatedBy", wageConfigDetail.CreatedBy);

            parameters.Add("@SkillTypeId", wageConfigDetail.SkillTypeId);
            parameters.Add("@ContractorId", wageConfigDetail.ContractorId);

            parameters.Add("@EffectiveFrom", wageConfigDetail.EffectiveFrom);
            parameters.Add("@EffectiveTo", wageConfigDetail.EffectiveTo);
            parameters.Add("@HRACalculateOn", wageConfigDetail.HRACalculateOn);

          // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.WageConfigMaster); // Cast enum to int

            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                wageConfigDetail.StatusMessage = result.ApplicationMessage;
                wageConfigDetail.MessageType = result.ApplicationMessageType;
            }
            return wageConfigDetail;
        }
        public async Task<WageConfigDetail> UpdateAsync(string procedureName, WageConfigDetail wageConfigDetail)
        {
            var parameters = new DynamicParameters();
            // Assuming wageGradeDetail is passed as the object
            // Add the parameters required for the stored procedure
            parameters.Add("@WageConfig_Dtl_Id", wageConfigDetail.WageConfig_Dtl_Id);
            parameters.Add("@Wage_Id", wageConfigDetail.Wage_Id);
            parameters.Add("@WageSalaryBasic", wageConfigDetail.WageSalaryBasic);
            parameters.Add("@PaymentModeId", wageConfigDetail.PaymentModeId);
            parameters.Add("@IsHRAapplicable", wageConfigDetail.IsHRAapplicable);
            parameters.Add("@HRAallownceType", wageConfigDetail.HRAallownceType);
            parameters.Add("@HRAallowncePer", wageConfigDetail.HRAallowncePer);
            parameters.Add("@HRAallownceAmt", wageConfigDetail.HRAallownceAmt);
            parameters.Add("@Is_PFApplicable", wageConfigDetail.Is_PFApplicable);
            parameters.Add("@PF_DeductionType", wageConfigDetail.PF_DeductionType);
            parameters.Add("@PF_CalculateOn", wageConfigDetail.PF_CalculateOn);
            parameters.Add("@PF_Employee_Amt", wageConfigDetail.PF_Employee_Amt);
            parameters.Add("@PF_Employer_Amt", wageConfigDetail.PF_Employer_Amt);
            parameters.Add("@Is_PensionApplicable", wageConfigDetail.Is_PensionApplicable);
            parameters.Add("@Pension_DeductionType", wageConfigDetail.Pension_DeductionType);
            parameters.Add("@Pension_CalculateOn", wageConfigDetail.Pension_CalculateOn);
            parameters.Add("@Pension_Employee_Amt", wageConfigDetail.Pension_Employee_Amt);
            parameters.Add("@Pension_Employer_Amt", wageConfigDetail.Pension_Employer_Amt);
            parameters.Add("@Wage_Grade_Type", wageConfigDetail.Wage_Grade_Type);
            parameters.Add("@Is_HRAApplicable", wageConfigDetail.Is_HRAApplicable);
            parameters.Add("@HRA_AllowanceType", wageConfigDetail.HRA_AllowanceType);
            parameters.Add("@HRA_Percentage", wageConfigDetail.HRA_Percentage);
            parameters.Add("@HRA_Amount", wageConfigDetail.HRA_Amount);
            parameters.Add("@Earning_Dedcution_Id", wageConfigDetail.Earning_Dedcution_Id);
            parameters.Add("@NotInUse", wageConfigDetail.NotInUse);
            parameters.Add("@IsActive", wageConfigDetail.IsActive);
            parameters.Add("@CreatedBy", wageConfigDetail.CreatedBy);
            parameters.Add("@UpdatedBy", wageConfigDetail.UpdatedBy);

            parameters.Add("@SkillTypeId", wageConfigDetail.SkillTypeId);
            parameters.Add("@ContractorId", wageConfigDetail.ContractorId);

            parameters.Add("@EffectiveFrom", wageConfigDetail.EffectiveFrom);
            parameters.Add("@EffectiveTo", wageConfigDetail.EffectiveTo);
            parameters.Add("@HRACalculateOn", wageConfigDetail.HRACalculateOn);

            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.WageConfigMaster); // Cast enum to int
            // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                wageConfigDetail.StatusMessage = result.ApplicationMessage;
                wageConfigDetail.MessageType = result.ApplicationMessageType;
            }
            return wageConfigDetail;
        }
        public async Task<WageConfigDetail> DeleteAsync(string procedureName, object wageConfigDetail)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@WageConfig_Dtl_Id", ((dynamic)wageConfigDetail).WageConfig_Dtl_Id, DbType.Int32);
            parameters.Add("@UpdatedBy", ((dynamic)wageConfigDetail).UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information); // Cast enum to int
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Delete); // Cast enum to int
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.WageConfigMaster); // Cast enum to int
                                                                                      // Execute stored procedure
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            // Check result for messages and status
            if (result != null)
            {
                ((dynamic)wageConfigDetail).StatusMessage = result.ApplicationMessage;
                ((dynamic)wageConfigDetail).MessageType = result.ApplicationMessageType;
            }
            return (WageConfigDetail)wageConfigDetail;
        }
        #endregion

     
    }
}
