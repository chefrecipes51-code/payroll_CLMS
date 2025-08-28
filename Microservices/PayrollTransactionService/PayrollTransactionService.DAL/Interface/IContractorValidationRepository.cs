using Payroll.Common.ApplicationModel;
using PayrollTransactionService.BAL.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IContractorValidationRepository
    {
        Task<IEnumerable<ContractorValidationRequest>> GetContractorValidationAsync(
                             string procedureName,
                             byte companyId,
                             List<int>? locationIds,
                             //List<int>? contractorIds,
                             List<int>? workOrderIds);
        Task<IEnumerable<EntityValidationRequest>> GetEntityValidationAsync(string procedureName, byte companyId, List<int> contractorIds);
        Task<ValidateContractorRequest> UpdateValidateContractorsAsync(string procedureName, ValidateContractorRequest request);
        Task<EntityUpdateRequest> UpdateValidateEntitiesAsync(string procedureName,EntityUpdateRequest request);
        Task<IEnumerable<EntityPayValidationRequest>> GetEntityPayValidationAsync(string procedureName, byte companyId, List<int> entityIds);
        Task<PayCalculationUpdateRequest> UpdateValidatePayCalcAsync(string procedureName, PayCalculationUpdateRequest request);
        Task<IEnumerable<EntityComplianceValidationRequest>> GetEntityComplianceValidationAsync(string procedureName, byte companyId, List<int> entityIds);
        Task<EntityComplianceUpdateRequest> UpdateValidateComplianceAsync(string procedureName, EntityComplianceUpdateRequest request);
        Task<IEnumerable<EntityAttendanceRequest>> GetEntityAttendanceValidationAsync(string procedureName, byte companyId, List<int> entityIds,
    int payrollMonth,    
    int payrollYear);
        Task<EntityAttendanceUpdateRequest> UpdateValidateAttendanceAsync(string procedureName, EntityAttendanceUpdateRequest request);
        Task<CompanyPreviousMonthYearRequest?> GetPreviousMonthYearPeriodByCompanyIdAsync(string procedureName, int companyId);


    }
}
