using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class ServiceApprovalRejection : BaseModel 
    {
        public long Srv_Appr_Rej_Id { get; set; }  
        public int Company_Id { get; set; }    
        public int Module_Id { get; set; }     
        public int KeyField_1 { get; set; }     
        public int? KeyField_2 { get; set; }   
        public int? KeyField_3 { get; set; }   
        public int Requested_By { get; set; }  
        public DateTime Requested_DateTime { get; set; }
        public char Request_Status { get; set; } 
        public int? Checked_By { get; set; }    
        public DateTime? Checked_Datetime { get; set; } 
        public string Rejection_Reason { get; set; } 
        public int Approve_Reject_Level { get; set; }
        public int? Approver_Id { get; set; }
        public int? OutMessageMode { get; set; }
    }
}
