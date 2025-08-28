using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class EntityValidationRequest
    {
        public int Entity_ID { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
       // public string Gender { get; set; }
        public DateTime? DateOfDeployment { get; set; }
        public DateTime? ValidityDate { get; set; }
        public string AadharNo { get; set; }
        public string CompanyMastCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentCode { get; set; }
        public string SkillCategory { get; set; }
        public string EntityType { get; set; }
        public string EntityGrade { get; set; }
        public string EntityTrade { get; set; }
        public string ContractorName { get; set; }
        public string WorkorderNo { get; set; }
        public bool? IsActive { get; set; }
        public string GradeConfigName { get; set; }
        public DateTime? WorkOrderStart { get; set; }
        public DateTime? WorkOrderEnd { get; set; }
        public decimal? WorkOrderValue { get; set; }
        public string? HasWorkOrderValue { get; set; }

        // Validation Flags
        public bool IsMissing_AadharNo { get; set; }
        public bool IsMissing_CompanyName { get; set; }
        public bool IsMissing_SkillCategory { get; set; }
        public bool IsMissing_EntityGrade { get; set; }
        public bool IsMissing_EntityTrade { get; set; }
        public bool IsMissing_ContractorName { get; set; }
        public bool IsMissing_WorkorderNo { get; set; }
        public bool IsMissing_GradeConfigName { get; set; }
        public bool IsMissing_WorkOrderValue { get; set; }
    }
    public class EntityValidationRequestModel
    {
        public byte CompanyId { get; set; }
        public List<int> ContractorIds { get; set; }
    }
}
