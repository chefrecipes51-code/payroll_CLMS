using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class FineRegisterReport
    {
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
        public string Father_Name { get; set; }
        public string Skillcategory_Name { get; set; }
        public string Reason { get; set; }
        public string ShowCaused { get; set; }
        public string Date_Of_Offence { get; set; } // Formatted as dd/MM/yyyy
        public string Name_Of_WitNess { get; set; }
        public string Waage_Period { get; set; } // Period range formatted as "start - end"
        public string Dt_Of_Realization { get; set; } // Formatted date
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
    }

    public class FineRegisterFilter
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
