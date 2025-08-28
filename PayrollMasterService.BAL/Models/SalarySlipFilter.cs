using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class SalarySlipReport
    {
        public int Entity_ID { get; set; }
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
        public string Father_Name { get; set; }
        public string Skillcategory_Name { get; set; }
        public string EntityAddress { get; set; }
        public string Contractor_Name { get; set; }
        public int Month_Id { get; set; } // Formatted as dd/MM/yyyy
        public string Unit_Worked { get; set; }
        public int total_Hrs_Worked { get; set; } // Period range formatted as "start - end"
        public decimal Total_Ot_Amount { get; set; } // Period range formatted as "start - end"
        public decimal GrossPay { get; set; } // Formatted date
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }
        public decimal Daily_wage { get; set; }
    }
    public class SalarySlipFilter
    {
        public int CompanyID { get; set; }
        public string CompanyLocationIDs { get; set; } // Comma-separated IDs
        public string ContractorIDs { get; set; } // Comma-separated IDs
        public string EntityIDs { get; set; } // Comma-separated IDs
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }
    }

}
