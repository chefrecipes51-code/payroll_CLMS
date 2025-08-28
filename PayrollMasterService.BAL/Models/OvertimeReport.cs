using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class OvertimeReport
    {
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
        public string Gender { get; set; }
        public string Father_Name { get; set; }
        public string Skillcategory_Name { get; set; }
        public decimal Total_OT_Hours { get; set; }
        public decimal OT_Rate { get; set; }
        public decimal OT_Amount { get; set; } // Formatted as dd/MM/yyyy
        public DateTime OT_Paid_Date { get; set; }
        
    }

    public class OvertimeFilter
    {
        public int CompanyID { get; set; }
        public string CompanyLocationIDs { get; set; } // Comma-separated IDs
        public string ContractorIDs { get; set; } // Comma-separated IDs
        public string EntityIDs { get; set; } // Comma-separated IDs
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime FinancialYearStart { get; set; }
    }
}
