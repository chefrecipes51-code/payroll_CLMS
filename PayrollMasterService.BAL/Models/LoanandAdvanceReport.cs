using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class LoanandAdvanceReport
    {
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
        public string Father_Name { get; set; }
        public string Skillcategory_Name { get; set; }
        public decimal Loan_Amount { get; set; }
        public DateTime Issue_Date { get; set; }
        public int No_Of_Instalments { get; set; } // Formatted as dd/MM/yyyy
        public decimal Monthly_Installment { get; set; }
        public string purpose_of_loan { get; set; } // Period range formatted as "start - end"
        public DateTime Instalment_St_Date { get; set; } // Formatted date
        public decimal Wage_Payable { get; set; } // Formatted date
        public DateTime last_Instalment_Paymengt_Dt { get; set; } // Formatted date
    }
    public class LoanandAdvanceFilter
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
