using PayrollTransactionService.BAL.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IAuditTrailRepository : IGenericRepository<AuditTrail>
    {
        //Task<IEnumerable<AuditTrail>> GetAuditTrailsByDateRangeAsync(string procedureName, int companyId, DateTime? dateFrom, DateTime? dateTo);
        Task<IEnumerable<AuditTrail>> GetAuditTrailsByDateRangeAsync(string procedureName, AuditTrail model);

    }
}
