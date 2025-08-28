using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class EntityTaxStatutory : BaseModel
    {

        public int Entity_statutory_Id { get; set; }
        public int Employee_id { get; set; }
        public string PayrollNo { get; set; }
        public byte Company_Id { get; set; }
        public int? PF_No { get; set; }
        public decimal? PF_Employer_Contribution { get; set; }
        public decimal? PF_Employee_Contribution { get; set; }
        public string? ESIC_No { get; set; }
        public decimal? ESIC_Employer_Contribution { get; set; }
        public decimal? ESIC_Employee_Contribution { get; set; }
        public decimal? Professional_Tax { get; set; }

        public bool? Is_Gratuity_Eligibility { get; set; }

        public string? Gratuity_Account_No { get; set; }
        public string? TIN_No { get; set; }

        //public string StatutoryType_Name { get; set; }
    }
}
