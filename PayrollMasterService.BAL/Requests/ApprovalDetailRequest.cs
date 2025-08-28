using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Requests
{
    public class ApprovalDetailRequest : BaseModel
    {
        public int Approval_Hdr_Id { get; set; }
        public string Audit_ID { get; set; }
        public string ModuleName { get; set; }
        public int Approver_ID { get; set; }
        public string ApproverName { get; set; } 
        public string Request_Date { get; set; }
        public string ApprovalStatus { get; set; } 
        public int Request_User_Id { get; set; }
        public string ReportedBy { get; set; }
        ///Property for UPDATE
        public int Approval_ID { get; set; }        
        public int Approval_Status { get; set; }
    }
    public class ApprovalSummaryCounts
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }
    public class ApprovalListViewModel
    {
        public List<ApprovalDetailRequest> ApprovalList { get; set; }
        public ApprovalSummaryCounts SummaryCounts { get; set; }
    }

}
