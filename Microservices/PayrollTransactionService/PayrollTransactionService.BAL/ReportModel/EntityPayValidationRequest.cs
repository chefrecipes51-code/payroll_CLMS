using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class EntityPayValidationRequest
    {
        public int Entity_ID { get; set; }
        public string EntityCode { get; set; }
        public string EntityNamed { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfDeployment { get; set; }
        public DateTime? ValidityDate { get; set; }
        public string AadharNo { get; set; }
        public string CompanyMastCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentCode { get; set; }
        public string SkillCategory { get; set; }
        public string EntityType { get; set; }
        public string ContractorName { get; set; }
        public string WorkorderNo { get; set; }
        public bool IsActive { get; set; }
        public string GradeConfigName { get; set; }
        public DateTime? WorkOrderStart { get; set; }
        public DateTime? WorkOrderEnd { get; set; }
        public decimal? WorkOrderValue { get; set; }
        public string SalaryStructureName { get; set; }

        // Validation flag
        public bool IsMissing_SalaryStructureName { get; set; }
    }
    public class EntityPayValidationRequestModel
    {
        public byte CompanyId { get; set; }
        public List<int> EntityIds { get; set; }
    }

}
