using Dapper;
using Payroll.Common.EnumUtility;
using payrollmasterservice.BAL.Models;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Service
{
    public class ReportServiceRepository : IReportRepository
    {
        private readonly IDbConnection _dbConnection;
        public ReportServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<PayRegisterReport>> GetPayRegisterReportAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<PayRegisterReport>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<FineRegisterReport>> GetFineRegisterReportAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<FineRegisterReport>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ComplianceReport>> GetComplianceReportAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<ComplianceReport>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }

    
        #region LOSS AND DAMAGE

        public async Task<IEnumerable<LossDamageRegisterReport>> GetLossDamageRegisterReportAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<LossDamageRegisterReport>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }

        #endregion
        public async Task<IEnumerable<SalarySlipReport>> GetSalarySlipReportAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<SalarySlipReport>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<OvertimeReport>> GetOvertimeReportAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<OvertimeReport>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<LoanandAdvanceReport>> GetLoanandAdvanceReportAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<LoanandAdvanceReport>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<TaxDeductionReport>> GetTaxDeductionReportAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<TaxDeductionReport>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<IEnumerable<ContractorPaymentRegisterReport>> GetContractorPaymentRegisterDataAsync(string procedureName, object parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);
            return await _dbConnection.QueryAsync<ContractorPaymentRegisterReport>(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);
        }

    }
}
