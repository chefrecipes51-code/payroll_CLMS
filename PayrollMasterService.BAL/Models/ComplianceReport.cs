using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class ComplianceReport
    {
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
        public string Skillcategory_Name { get; set; }
        public string Esic_No { get; set; }
        public string Pf_No { get; set; }
        public string Contractor_Name { get; set; }
        public int Month_Id { get; set; } // Assuming Month_Id is a numeric value (like 1 to 12)
        public string Labour_Fine { get; set; } 
        public decimal ESI_Employer_Contribution { get; set; } 
        public decimal ESI_Employee_Contribution { get; set; }
        public decimal Pf_Employeer_Contribution { get; set; }
        public decimal Pf_Employee_Contribution { get; set; }
    }
    public class ComplianceFilter
    {
        public int CompanyID { get; set; }
        public string CompanyLocationIDs { get; set; }
        public string ContractorIDs { get; set; } 
        public string EntityIDs { get; set; } 
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }

    }
}
