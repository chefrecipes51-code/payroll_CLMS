using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class PayrollComplianceRequest : BaseModel
    {
        public int Prm_Comlliance_ID { get; set; }
        public int Company_ID { get; set; }
        public bool? TDsDeducted_On_Actual_Date { get; set; }  // Nullable in DB
        public decimal Pf_Applicable_Percentage { get; set; }
        public int Pf_Based_on { get; set; }
        public decimal Esi_Applicable_Percentage { get; set; }
        public int? Esi_Based_on { get; set; }  // Nullable in DB
        public int Pf_Applicable { get; set; }
        public int Pf_Share_Mode_Employer { get; set; }
        public decimal Epf_Employer_Share_Percentage { get; set; }
        public decimal Eps_Employer_Share_Percentage { get; set; }
        public bool VPF_Applicable { get; set; }
        public int VPF_Mode { get; set; }
        public bool Esic_Applicable { get; set; }
        public decimal Esic_Salary_Limit { get; set; }
        public bool PT_Applicable { get; set; }
        public int Pt_Registration_Mode { get; set; }
        public int Lwf_Mode { get; set; }
        public int Lwf_Cycle { get; set; }
        public decimal Lwf_Contribution { get; set; }
        public int? CopyFromCompanyId { get; set; }
    }
}
