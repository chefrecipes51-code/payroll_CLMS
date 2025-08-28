using PayrollTransactionService.BAL.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IPendingApprovalRequestRepository
    {
        Task<IEnumerable<PendingApprovalRequest>> GetServiceApprovalRejectionsAsync(string procedureName, PendingApprovalRejectionFilter model);
    }
}
