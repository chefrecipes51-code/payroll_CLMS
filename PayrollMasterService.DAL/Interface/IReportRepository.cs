using Payroll.Common.Repository.Interface;
using payrollmasterservice.BAL.Models;
using PayrollMasterService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Interface
{
    public interface IReportRepository
    {
        Task<IEnumerable<PayRegisterReport>> GetPayRegisterReportAsync(string procedureName, object parameters);
        Task<IEnumerable<FineRegisterReport>> GetFineRegisterReportAsync(string procedureName, object parameters);
        Task<IEnumerable<ComplianceReport>> GetComplianceReportAsync(string procedureName, object parameters);
        Task<IEnumerable<SalarySlipReport>> GetSalarySlipReportAsync(string procedureName, object parameters);
        Task<IEnumerable<OvertimeReport>> GetOvertimeReportAsync(string procedureName, object parameters);
        Task<IEnumerable<LossDamageRegisterReport>> GetLossDamageRegisterReportAsync(string procedureName, object parameters);
        Task<IEnumerable<LoanandAdvanceReport>> GetLoanandAdvanceReportAsync(string procedureName, object parameters);


        Task<IEnumerable<TaxDeductionReport>> GetTaxDeductionReportAsync(string procedureName, object parameters);
        Task<IEnumerable<ContractorPaymentRegisterReport>> GetContractorPaymentRegisterDataAsync(string procedureName, object parameters);
    }
}
