using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    
    public class ApprovalConfig
    {
        public int ConfigID { get; set; }
        public int? CompanyId { get; set; }
        public int? LocationId { get; set; }
        public int ModuleID { get; set; }
        public int ServiceID { get; set; }
        public string ApprovalProcessName { get; set; }
        public int TotalLevels { get; set; }
        public int Priority { get; set; }
        public int? AutoApproveDays { get; set; }
        public int? AutoRejectDays { get; set; }
        public int? ApproveType { get; set; }
        public int? NoOfDays { get; set; }
        public bool IsEmailAlert { get; set; }
        public bool IsActive { get; set; }
        public bool IsNotificationEnabled { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? EffectiveDate { get; set; }

        //  public List<ApprovalLevel> Levels { get; set; } = new List<ApprovalLevel>();
    }
    public class ApprovalLevel
    {
        public int LevelID { get; set; }
        public int ConfigID { get; set; }
        public int LevelNumber { get; set; }
        public string ApprovalType { get; set; }
        public bool IsApproveByAll { get; set; }
        public int? ApproveByAnyCount { get; set; }

       // public List<ApprovalDetail> Details { get; set; } = new List<ApprovalDetail>();
    }
    public class ApprovalDetail
    {
        public long ApprovalID { get; set; }
        public int ConfigID { get; set; }
        public int LevelID { get; set; }
        public int UserID { get; set; }
        public int SequenceOrder { get; set; }
        public bool IsAlternate { get; set; }
    }

    public class ApprovalConfigSelect : BaseModel // pending
    {
        public int ServiceID { get; set; }
        public int LevelNumber { get; set; }
        public int UserID { get; set; }
        public int SequenceOrder { get; set; }
        public string ModuleName { get; set; }
        public string ServiceName { get; set; }
    }

    public class ApprovalConfigCommon
    {
        public ApprovalConfig Config { get; set; }
        public List<ApprovalLevel> Levels { get; set; } = new List<ApprovalLevel>();
        public List<ApprovalDetail> Details { get; set; } = new List<ApprovalDetail>();
    }

    public class ApprovalConfigGrid : BaseModel
    {
        public int ConfigID { get; set; }
        public int? CompanyId { get; set; }
        public int? LocationId { get; set; }
        public int ModuleID { get; set; }
        public int ServiceID { get; set; }
        public string ApprovalProcessName { get; set; }
        public string Module { get; set; }
        public string Service { get; set; }
        public int TotalLevels { get; set; }
        public int Priority { get; set; }
        public int? AutoApproveDays { get; set; }
        public int? AutoRejectDays { get; set; }
        public int? ApproveType { get; set; }
        public int? NoOfDays { get; set; }
        public bool IsEmailAlert { get; set; }
        public bool IsActive { get; set; }
        public bool IsNotificationEnabled { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? EffectiveDate { get; set; }

        //  public List<ApprovalLevel> Levels { get; set; } = new List<ApprovalLevel>();
    }
}
