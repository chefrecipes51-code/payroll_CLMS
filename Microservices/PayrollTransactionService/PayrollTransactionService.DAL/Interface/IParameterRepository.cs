using PayrollTransactionService.BAL.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IParameterRepository
    {
        //Copy from One Company to Another Company 
        Task<CopySettingsRequest> CopyPayrollParameterAsync(string procedureName, CopySettingsRequest request);

        #region PayrollGlobalParamRequest CRUD
        Task<IEnumerable<EntityTypeRequest>> GetAllEntityTypeRequestAsync(string procedureName);
        Task<PayrollSettingsWrapper> GetGlobalPayrollParametersAsync(string procedureName, object parameters);
        Task<IEnumerable<PayrollGlobalParamRequest>> GetAllGlobalParamAsync(string procedureName);
        Task<PayrollGlobalParamRequest> GetGlobalParamByIdAsync(string procedureName, object parameters);
        Task<PayrollGlobalParamRequest> UpdateGlobalParamAsync(string procedureName, PayrollGlobalParamRequest model);
        Task<PayrollGlobalParamRequest> AddGlobalParamAsync(string procedureName, PayrollGlobalParamRequest model);
        //Task<PayrollGlobalParamRequest> DeleteGlobalParamAsync(string procedureName, object parameters);
        #endregion

        #region PayrollSetting CRUD
        Task<IEnumerable<PayrollSettingRequest>> GetAllPayrollSettingAsync(string procedureName);
        Task<PayrollSettingRequest> GetPayrollSettingByIdAsync(string procedureName, object parameters);
        Task<PayrollSettingRequest> UpdatePayrollSettingAsync(string procedureName, PayrollSettingRequest model);
        Task<PayrollSettingRequest> AddPayrollSettingAsync(string procedureName, PayrollSettingRequest model);
        //Task<PayrollSettingRequest> DeletePayrollSettingAsync(string procedureName, object parameters);
        #endregion

        #region PayrollCompliance CRUD
        Task<IEnumerable<PayrollComplianceRequest>> GetAllPayrollComplianceAsync(string procedureName);
        Task<PayrollComplianceRequest> GetPayrollComplianceByIdAsync(string procedureName, object parameters);
        Task<PayrollComplianceRequest> UpdatePayrollComplianceAsync(string procedureName, PayrollComplianceRequest model);
        Task<PayrollComplianceRequest> AddPayrollComplianceAsync(string procedureName, PayrollComplianceRequest model);
        //Task<PayrollComplianceRequest> DeletePayrollComplianceAsync(string procedureName, object parameters);
        #endregion

        #region ThirdPartyParameterRequest CRUD
        Task<IEnumerable<ThirdPartyParameterRequest>> GetAllThirdPartyParameterAsync(string procedureName);
        Task<ThirdPartyParameterRequest> GetThirdPartyParameterByIdAsync(string procedureName, object parameters);
        Task<ThirdPartyParameterRequest> UpdateThirdPartyParameterAsync(string procedureName, ThirdPartyParameterRequest model);
        Task<ThirdPartyParameterRequest> AddThirdPartyParameterAsync(string procedureName, ThirdPartyParameterRequest model);
        //Task<ThirdPartyParameterRequest> DeleteThirdPartyParameterAsync(string procedureName, object parameters);
        #endregion
    }
}
