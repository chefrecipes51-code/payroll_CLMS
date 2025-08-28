namespace Payroll.WebApp.Models.DTOs
{
    public class RegisterReportDTO
    {
        public int companyID { get; set; }
        public string contractorIDs { get; set; } = "";
        public int payrollMonth { get; set; }
        public int payrollYear { get; set; }
        public string financialYearStart { get; set; } = "";
    }
    public class LossDamageReportDTO
    {
        public int CompanyID { get; set; }
        public string CompanyLocationIDs { get; set; }
        public string ContractorIDs { get; set; }
        public string EntityIDs { get; set; }
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime FinancialYearStart { get; set; }
    }
   
}
