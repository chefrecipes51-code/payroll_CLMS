using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{

    public class ApprovalSetUp
    {
        public List<ApprovalSetUpDetail> approvalSetUpDetails { get; set; } = new();
        public int ModuleId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int MessageType { get; set; }                                
        public int MessageMode { get; set; }                              
       
      
    }
    public class ApprovalSetUpDetail
    {
        public int ServiceID { get; set; }
        public int LevelNumber { get; set; }
        public int UserID { get; set; }
        public int SequenceOrder { get; set; }
        public bool IsAlternate { get; set; }
    }


    public class ApprovalSetUpSelect : BaseModel
    {
        public int ServiceID { get; set; }
        public int LevelNumber { get; set; }
        public int UserID { get; set; }
        public int SequenceOrder { get; set; }
        public string ModuleName { get; set; }
        public string ServiceName { get; set; }
    }
    public class ApprovalSetUpFilter
    {
        public int ApprovalID { get; set; }
        public int ServiceID { get; set; }
        public int LevelNumber { get; set; }
        public int UserID { get; set; }
        public int SequenceOrder { get; set; }
        public bool IsAlternate { get; set; }

    }
}
