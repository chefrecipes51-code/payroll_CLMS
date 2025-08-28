using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class TaxDeductionReportFilter
    {
        public int CompanyID { get; set; }
        public string? CompanyLocationIDs { get; set; }
        public string? ContractorIDs { get; set; }
        public string? EntityIDs { get; set; }
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime FinancialYearStart { get; set; }
    }

    public class TaxDeductionReport
    {
        public int Entity_ID { get; set; }
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
        public string PayrollNo { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal ProfessionalTax { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal TotalTaxDeductions { get; set; }
        public decimal NetTaxableAmount { get; set; }
    }
}
