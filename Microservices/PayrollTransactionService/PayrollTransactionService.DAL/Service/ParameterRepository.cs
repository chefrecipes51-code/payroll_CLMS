using Dapper;
using Payroll.Common.EnumUtility;
using PayrollTransactionService.BAL.ReportModel;
using PayrollTransactionService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Service
{
    public class ParameterRepository : IParameterRepository
    {
        #region CTOR
        private readonly IDbConnection _dbConnection;
        public ParameterRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        #endregion

        #region EntityType
        public async Task<IEnumerable<EntityTypeRequest>> GetAllEntityTypeRequestAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<EntityTypeRequest>(procedureName, commandType: CommandType.StoredProcedure);
        }
        #endregion

        #region Copy From One Company to another Company 
        public async Task<CopySettingsRequest> CopyPayrollParameterAsync(string procedureName, CopySettingsRequest request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SelectParam", request.SelectParam, DbType.String);
            parameters.Add("@CopyFrmCompany_ID", request.CopyFromCompanyID, DbType.Int32);
            parameters.Add("@CopyToCompanyID", request.CopyToCompanyID, DbType.Int32);
            parameters.Add("@CreatedBy", request.CreatedBy, DbType.Int32);          

            // Static system values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.GlobalParameterSetup);

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                request.StatusMessage = result.ApplicationMessage;
                request.MessageType = result.ApplicationMessageType;
            }

            return request;
        }
        #endregion

        #region PayrollGlobalParamRequest CRUD
        public async Task<PayrollSettingsWrapper> GetGlobalPayrollParametersAsync(string procedureName, object parameters)
        {
            var result = new PayrollSettingsWrapper();

            var dynamicParameters = new DynamicParameters(parameters);

            using var multi = await _dbConnection.QueryMultipleAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

            result.GlobalParams = (await multi.ReadAsync<PayrollGlobalParamRequest>()).FirstOrDefault();
            result.Settings = (await multi.ReadAsync<PayrollSettingRequest>()).FirstOrDefault();
            result.Compliances = (await multi.ReadAsync<PayrollComplianceRequest>()).FirstOrDefault();
            result.ThirdPartyParams = (await multi.ReadAsync<ThirdPartyParameterRequest>()).FirstOrDefault();

            return result;
        }


        public async Task<IEnumerable<PayrollGlobalParamRequest>> GetAllGlobalParamAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<PayrollGlobalParamRequest>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<PayrollGlobalParamRequest> GetGlobalParamByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<PayrollGlobalParamRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<PayrollGlobalParamRequest> AddGlobalParamAsync(string procedureName, PayrollGlobalParamRequest model)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Global_Param_ID", model.Global_Param_ID, DbType.Int32);
            parameters.Add("@copy_from_Company_ID", model.CopyFromCompanyId, DbType.Int32);
            parameters.Add("@Company_ID", model.Company_ID, DbType.Int32);

            parameters.Add("@Salary_Frequency", model.Salary_Frequency, DbType.Int32);
            parameters.Add("@MonthlySalary_Based_On", model.MonthlySalary_Based_On, DbType.Int32);
            parameters.Add("@Round_Off_Components", model.Round_Off_Components, DbType.Decimal);
            parameters.Add("@EffectivePayroll_start_Mnth", model.EffectivePayroll_start_Mnth, DbType.Int32);
            parameters.Add("@Allow_Adhoc_Components", model.Allow_Adhoc_Components, DbType.Boolean);
            parameters.Add("@LOckSalary_Post_Payroll", model.LOckSalary_Post_Payroll, DbType.Boolean);
                  
            parameters.Add("@CreatedBy", model.CreatedBy, DbType.Int32);
            parameters.Add("@Isactive", model.IsActive, DbType.Boolean);
            // Application Message Settings
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.GlobalParameterSetup);

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = ((dynamic)result).ApplicationMessageType;
            }

            return model;
        }
        public async Task<PayrollGlobalParamRequest> UpdateGlobalParamAsync(string procedureName, PayrollGlobalParamRequest model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Global_Param_ID", model.Global_Param_ID, DbType.Int32);
            parameters.Add("@copy_from_Company_ID", model.CopyFromCompanyId, DbType.Int32);
            parameters.Add("@Company_ID", model.Company_ID, DbType.Int32);
            // Add parameters for the stored procedure
           
            parameters.Add("@Salary_Frequency", model.Salary_Frequency, DbType.Int32);
            parameters.Add("@MonthlySalary_Based_On", model.MonthlySalary_Based_On, DbType.Int32);
            parameters.Add("@Round_Off_Components", model.Round_Off_Components, DbType.Decimal);
            parameters.Add("@EffectivePayroll_start_Mnth", model.EffectivePayroll_start_Mnth, DbType.Int32);
            parameters.Add("@Allow_Adhoc_Components", model.Allow_Adhoc_Components, DbType.Boolean);
            parameters.Add("@LOckSalary_Post_Payroll", model.LOckSalary_Post_Payroll, DbType.Boolean);      
            parameters.Add("@Isactive", model.IsActive, DbType.Boolean);
            parameters.Add("@UpdatyedBy", model.UpdatedBy, DbType.Int32);
            // Additional parameters for messages and status using enum values
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.GlobalParameterSetup);
            parameters.Add("@IsActive", model.IsActive, DbType.Boolean);
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            // If the result is not null, populate the status and message fields
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = ((dynamic)result).ApplicationMessageType;
            }

            return model;
        }

        #endregion

        #region PayrollSettingRequest CRUD
        public async Task<IEnumerable<PayrollSettingRequest>> GetAllPayrollSettingAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<PayrollSettingRequest>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<PayrollSettingRequest> GetPayrollSettingByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<PayrollSettingRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<PayrollSettingRequest> AddPayrollSettingAsync(string procedureName, PayrollSettingRequest setting)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Payroll_Setin_ID", setting.Payroll_Setin_ID, DbType.Int32);
            parameters.Add("@Company_ID", setting.Company_ID, DbType.Int32);
            parameters.Add("@Payslip_Generation", setting.Payslip_Generation, DbType.Boolean);
            parameters.Add("@Payslip_Format", setting.Payslip_Format, DbType.Int32);
            parameters.Add("@PayslipNumber_Format", setting.PayslipNumber_Format, DbType.String);
            parameters.Add("@PaySlip_Number_Scope", setting.PaySlip_Number_Scope, DbType.Int32);
            parameters.Add("@Initial_char", setting.Initial_char, DbType.String);
            parameters.Add("@Auto_Numbering", setting.Auto_Numbering, DbType.Boolean);
            parameters.Add("@DigitalSignatur_Requirede", setting.DigitalSignatur_Requirede, DbType.Boolean);
            parameters.Add("@IsPayslipNo_Reset", setting.IsPayslipNo_Reset, DbType.Boolean);
            parameters.Add("@PaySlipAutoEmail", setting.PaySlipAutoEmail, DbType.Boolean);        
            parameters.Add("@CreatedBy", setting.CreatedBy, DbType.Int32);           
            parameters.Add("@copy_from_Company_ID", setting.CopyFromCompanyId, DbType.Int32);

            // Standard messaging

            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PayrollSettingParameter);
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                setting.StatusMessage = result.ApplicationMessage;
                setting.MessageType = result.ApplicationMessageType;
            }

            return setting;
        }
        public async Task<PayrollSettingRequest> UpdatePayrollSettingAsync(string procedureName, PayrollSettingRequest model)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Payroll_Setin_ID", model.Payroll_Setin_ID, DbType.Int32);
            parameters.Add("@Company_ID", model.Company_ID, DbType.Int32);
            parameters.Add("@Payslip_Generation", model.Payslip_Generation, DbType.Boolean);
            parameters.Add("@Payslip_Format", model.Payslip_Format, DbType.Int32);
            parameters.Add("@PayslipNumber_Format", model.PayslipNumber_Format, DbType.String, size: 10);
            parameters.Add("@PaySlip_Number_Scope", model.PaySlip_Number_Scope, DbType.Int32);
            parameters.Add("@Initial_char", model.Initial_char, DbType.String, size: 4);
            parameters.Add("@Auto_Numbering", model.Auto_Numbering, DbType.Boolean);
            parameters.Add("@DigitalSignatur_Requirede", model.DigitalSignatur_Requirede, DbType.Boolean);
            parameters.Add("@IsPayslipNo_Reset", model.IsPayslipNo_Reset, DbType.Boolean);
            parameters.Add("@PaySlipAutoEmail", model.PaySlipAutoEmail, DbType.Boolean);
            parameters.Add("@CreatedBy", model.CreatedBy, DbType.Int32);
            parameters.Add("@UpdatedBy", model.UpdatedBy, DbType.Int32);
            parameters.Add("@IsActive", model.IsActive, DbType.Boolean);
            parameters.Add("@copy_from_Company_ID", model.CopyFromCompanyId, DbType.Int32);
            // Additional parameters for message tracking
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.PayrollSettingParameter);

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = ((dynamic)result).ApplicationMessageType;
            }

            return model;
        }

        #endregion

        #region PayrollComplianceRequest CRUD
        public async Task<IEnumerable<PayrollComplianceRequest>> GetAllPayrollComplianceAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<PayrollComplianceRequest>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<PayrollComplianceRequest> GetPayrollComplianceByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<PayrollComplianceRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<PayrollComplianceRequest> AddPayrollComplianceAsync(string procedureName, PayrollComplianceRequest model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@prm_Comlliance_ID", model.Prm_Comlliance_ID, DbType.Int32);
            parameters.Add("@copy_from_Company_ID", model.CopyFromCompanyId, DbType.Int32);
            parameters.Add("@Company_ID", model.Company_ID, DbType.Int32);
            parameters.Add("@TDsDeducted_On_Actual_Date", model.TDsDeducted_On_Actual_Date, DbType.Boolean);
            parameters.Add("@Pf_Applicable_Percentage", model.Pf_Applicable_Percentage, DbType.Decimal);
            parameters.Add("@Pf_Based_on", model.Pf_Based_on, DbType.Int32);
            parameters.Add("@Esi_Applicable_Percentage", model.Esi_Applicable_Percentage, DbType.Decimal);
            parameters.Add("@Esi_Based_on", model.Esi_Based_on, DbType.Int32);

            parameters.Add("@Pf_Applicable", model.Pf_Applicable, DbType.Int32);
            parameters.Add("@Pf_Share_Mode_Employer", model.Pf_Share_Mode_Employer, DbType.Int32);
            parameters.Add("@Epf_Employer_Share_Percentage", model.Epf_Employer_Share_Percentage, DbType.Decimal);
            parameters.Add("@Eps_Employer_Share_Percentage", model.Eps_Employer_Share_Percentage, DbType.Decimal);
            parameters.Add("@VPF_Applicable", model.VPF_Applicable, DbType.Boolean);
            parameters.Add("@VPF_Mode", model.VPF_Mode, DbType.Int32);
            //parameters.Add("@Esic_Applicable", model.Esic_Applicable, DbType.Boolean);
            parameters.Add("@Esic_Salary_Limit", model.Esic_Salary_Limit, DbType.Decimal);
            parameters.Add("@PT_Applicable", model.PT_Applicable, DbType.Boolean);
            parameters.Add("@Pt_Registration_Mode", model.Pt_Registration_Mode, DbType.Int32);
            parameters.Add("@Lwf_Mode", model.Lwf_Mode, DbType.Int32);
            parameters.Add("@Lwf_Cycle", model.Lwf_Cycle, DbType.Int32);
            parameters.Add("@Lwf_Contribution", model.Lwf_Contribution, DbType.Decimal);

            parameters.Add("@IsActive", model.IsActive, DbType.Boolean);
            parameters.Add("@CreatedBy", model.CreatedBy, DbType.Int32);
            //parameters.Add("@copy_from_Company_ID", model.CopyFromCompanyId, DbType.Int32);
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.ComplianceParameter);
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = ((dynamic)result).ApplicationMessageType;
            }
            return model;
        }
        public async Task<PayrollComplianceRequest> UpdatePayrollComplianceAsync(string procedureName, PayrollComplianceRequest model)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@prm_Comlliance_ID", model.Prm_Comlliance_ID, DbType.Int32);
            parameters.Add("@Company_ID", model.Company_ID, DbType.Int32);
            parameters.Add("@TDsDeducted_On_Actual_Date", model.TDsDeducted_On_Actual_Date, DbType.Boolean);
            parameters.Add("@Pf_Applicable_Percentage", model.Pf_Applicable_Percentage, DbType.Decimal);
            parameters.Add("@Pf_Based_on", model.Pf_Based_on, DbType.Int32);
            parameters.Add("@Esi_Applicable_Percentage", model.Esi_Applicable_Percentage, DbType.Decimal);
            parameters.Add("@Esi_Based_on", model.Esi_Based_on, DbType.Int32);
            parameters.Add("@IsActive", model.IsActive, DbType.Boolean);
            parameters.Add("@CreatedBy", model.CreatedBy, DbType.Int32);
            parameters.Add("@UpdatedBy", model.UpdatedBy, DbType.Int32);
            parameters.Add("@Pf_Applicable", model.Pf_Applicable, DbType.Int32);

            // ✅ Parameters missing earlier
            parameters.Add("@Pf_Share_Mode_Employer", model.Pf_Share_Mode_Employer, DbType.Int32);
            parameters.Add("@Epf_Employer_Share_Percentage", model.Epf_Employer_Share_Percentage, DbType.Decimal);
            parameters.Add("@Eps_Employer_Share_Percentage", model.Eps_Employer_Share_Percentage, DbType.Decimal);
            parameters.Add("@VPF_Applicable", model.VPF_Applicable, DbType.Boolean);
            parameters.Add("@VPF_Mode", model.VPF_Mode, DbType.Int32);
            parameters.Add("@Esic_Salary_Limit", model.Esic_Salary_Limit, DbType.Decimal);
            parameters.Add("@PT_Applicable", model.PT_Applicable, DbType.Boolean);
            parameters.Add("@Pt_Registration_Mode", model.Pt_Registration_Mode, DbType.Int32);
            parameters.Add("@Lwf_Mode", model.Lwf_Mode, DbType.Int32);
            parameters.Add("@Lwf_Cycle", model.Lwf_Cycle, DbType.Int32);
            parameters.Add("@Lwf_Contribution", model.Lwf_Contribution, DbType.Decimal);
            // Message handling parameters
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.ComplianceParameter);

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }
            return model;
        }

        #endregion

        #region ThirdPartyParameterRequest CRUD
        public async Task<IEnumerable<ThirdPartyParameterRequest>> GetAllThirdPartyParameterAsync(string procedureName)
        {
            return await _dbConnection.QueryAsync<ThirdPartyParameterRequest>(procedureName, commandType: CommandType.StoredProcedure);
        }
        public async Task<ThirdPartyParameterRequest> GetThirdPartyParameterByIdAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters); // Pass the entire object directly to DynamicParameters
            return await _dbConnection.QuerySingleOrDefaultAsync<ThirdPartyParameterRequest>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<ThirdPartyParameterRequest> AddThirdPartyParameterAsync(string procedureName, ThirdPartyParameterRequest model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Clms_Param_ID", model.Clms_Param_ID, DbType.Int32);
            parameters.Add("@Company_Id", model.Company_Id, DbType.Int32);
            parameters.Add("@DataSync", model.DataSync, DbType.Boolean);
            parameters.Add("@Entityparam", model.Entityparam, DbType.String);
            parameters.Add("@Contractlabour_payment", model.Contractlabour_payment, DbType.Boolean);
            parameters.Add("@IsAttendanceProcessed", model.IsAttendanceProcessed, DbType.Boolean);
            parameters.Add("@AttendanceProxcessType", model.AttendanceProxcessType, DbType.Int32);
            parameters.Add("@Wo_Sync_Frequency", model.Wo_Sync_Frequency, DbType.Int32);
            parameters.Add("@Entity_Sync_Frequency", model.Entity_Sync_Frequency, DbType.Int32);
            parameters.Add("@IntegratedLog_in", model.IntegratedLog_in, DbType.Boolean);
            parameters.Add("@PayregisterFormat_ID", model.PayregisterFormat_ID, DbType.Int32);
            parameters.Add("@WorkOrder_Sync_Frequency", model.WorkOrder_Sync_Frequency, DbType.Int32);
            parameters.Add("@Cl_Sync_Frequency", model.Cl_Sync_Frequency, DbType.Int32);
            parameters.Add("@IsActive", model.IsActive, DbType.Boolean);

            parameters.Add("@CreatedBy", model.CreatedBy, DbType.Int32);
            parameters.Add("@UpdatedBy", model.UpdatedBy, DbType.Int32);
            parameters.Add("@copy_from_Company_ID", model.CopyFromCompanyId, DbType.Int32);

            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Insert);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.ThirdPartyParameter);
            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = ((dynamic)result).ApplicationMessageType;
            }
            return model;
        }

        public async Task<ThirdPartyParameterRequest> UpdateThirdPartyParameterAsync(string procedureName, ThirdPartyParameterRequest model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Clms_Param_ID", model.Clms_Param_ID, DbType.Int32);
            parameters.Add("@Company_Id", model.Company_Id, DbType.Int32);
            parameters.Add("@Entityparam", model.Entityparam, DbType.String);
            parameters.Add("@Contractlabour_payment", model.Contractlabour_payment, DbType.Boolean);
            parameters.Add("@IsAttendanceProcessed", model.IsAttendanceProcessed, DbType.Boolean);
            parameters.Add("@Wo_Sync_Frequency", model.Wo_Sync_Frequency, DbType.Int32);
            parameters.Add("@Entity_Sync_Frequency", model.Entity_Sync_Frequency, DbType.Int32);
            parameters.Add("@IntegratedLog_in", model.IntegratedLog_in, DbType.Boolean);
            parameters.Add("@PayregisterFormat_ID", model.PayregisterFormat_ID, DbType.Int32);
            parameters.Add("@CreatedBy", model.CreatedBy, DbType.Int32);
            parameters.Add("@IsActive", model.IsActive);         
            parameters.Add("@UpdatedBy", model.UpdatedBy, DbType.Int32);

            // Message flags
            parameters.Add("@Messagetype", (int)EnumUtility.ApplicationMessageTypeEnum.Information);
            parameters.Add("@MessageMode", (int)EnumUtility.ApplicationMessageModeEnum.Update);
            parameters.Add("@ModuleId", (int)EnumUtility.ModuleEnum.ThirdPartyParameter);

            var result = await _dbConnection.QueryFirstOrDefaultAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                model.StatusMessage = result.ApplicationMessage;
                model.MessageType = result.ApplicationMessageType;
            }

            return model;
        }

        #endregion
    }
}
