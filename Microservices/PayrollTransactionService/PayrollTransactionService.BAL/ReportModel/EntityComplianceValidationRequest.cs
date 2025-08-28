using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class EntityComplianceValidationRequest
    {
        public int Entity_ID { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string PFNo { get; set; }
        public string UANNo { get; set; }
        public string ESICNo { get; set; }
        public string PolicyNo { get; set; }

        public bool IsPFApplicable { get; set; }
        public decimal? PFAmount { get; set; }
        public decimal? PFPercent { get; set; }

        public bool IsVPFApplicable { get; set; }
        public decimal? VPFValue { get; set; }
        public decimal? VPFPercent { get; set; }

        public bool IsPTApplicable { get; set; }
        public decimal? PTAmount { get; set; }

        public bool IsGratuityApplicable { get; set; }
        public decimal? PolicyAmount { get; set; }

        // Validation Flags
        public bool IsMissing_PFNo { get; set; }
        public bool IsMissing_PFAmount { get; set; }
        public bool IsMissing_PFPercent { get; set; }
        public bool IsMissing_VPFValue { get; set; }
        public bool IsMissing_VPFPercent { get; set; }
        public bool IsMissing_UANNo { get; set; }
        public bool IsMissing_ESICNo { get; set; }
    }
    public class ComplianceValidationRequest : BaseModel
    {
        public int CompanyId { get; set; }
        public List<int> EntityIds { get; set; }
        public int PayrollMonth { get; set; }  
        public int PayrollYear { get; set; }  
    }
    public class EntityComplianceUpdateModel 
    {
        public int Entity_ID { get; set; }
        public string? PFNo { get; set; }
        public decimal? PFAmount { get; set; }
        public decimal? PFPercent { get; set; }
        public decimal? VPFValue { get; set; }
        public decimal? VPFPercent { get; set; }
        public string? UANNo { get; set; }
        public string? ESICNo { get; set; }
    }
    public class EntityComplianceUpdateRequest : BaseModel
    {
        public int CompanyId { get; set; }    
        public List<EntityComplianceUpdateModel> EntityComplianceUpdateList { get; set; }       
    }
}
