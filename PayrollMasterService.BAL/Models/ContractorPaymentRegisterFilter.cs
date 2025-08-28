using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class ContractorPaymentRegisterFilter
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

    public class ContractorPaymentRegisterReport
    {
        public string Contractor_Code { get; set; }
        public string Contractor_Name { get; set; }
        public string Payroll_MonthYear { get; set; }
        public int Total_Menpower { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }
    }
}
