using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class PendingApprovalRequest
    {
        public int SrvApprRejId { get; set; }
        public string RequestedBy { get; set; }
        public string ReportingTime { get; set; }
        public string EventName { get; set; }
        public string ApproveRejectLevel { get; set; }
    }
    public class PendingApprovalRejectionFilter
    {
        public int? CompanyId { get; set; }
        public int? UserId { get; set; }
    }
}
