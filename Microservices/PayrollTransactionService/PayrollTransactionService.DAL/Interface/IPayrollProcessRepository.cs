using PayrollTransactionService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IPayrollProcessRepository
    {
        Task<IEnumerable<ContractorPayrollValidation>> GetAllContractorPayrollValidationAsync(string procedureName, object parameters);
        Task<IEnumerable<CompanyPayrollValidation>> GetAllCompanyPayrollValidationAsync(string procedureName, object parameters);
        Task<IEnumerable<CompanyLocationPayrollValidation>> GetAllCompanyLocationPayrollValidationAsync(string procedureName, object parameters);
        Task<IEnumerable<WorkOrderPayrollValidation>> GetAllWorkOrderPayrollValidationAsync(string procedureName, object parameters);
        Task<IEnumerable<PreviousMonthYearPeriod>> GetAllPreviousMonthYearPeriodAsync(string procedureName, object parameters);
        Task<PayrollTransDataForProcess> AddPayrollTransDataForProcessAsync(string procedureName, PayrollTransDataForProcess model);
        Task<StartPayrollProcess> AddStartPayrollProcessAsync(string procedureName, StartPayrollProcess model);       
        Task<PayrollProcessResultModel> ProcessPayrollEmployeesAsync(string procedureName, PayrollProcessRequestModel model);
        Task<IEnumerable<PayrollProcessusingSignalR>> GetAllPayrollProcessusingSignalRAsync(string procedureName, object parameters);
        Task<IEnumerable<dynamic>> GetPhaseWiseComponentPayrollProcessAsync(string procedureName, PhaseWiseComponentPayrollProcess model);
    }
}
