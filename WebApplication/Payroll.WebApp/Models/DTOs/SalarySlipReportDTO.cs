namespace Payroll.WebApp.Models.DTOs
{
    public class SalarySlipReportDTO
    {
        public int CompanyID { get; set; }
        public string CompanyLocationIDs { get; set; } // Comma-separated IDs
        public string ContractorIDs { get; set; } // Comma-separated IDs
        public string EntityIDs { get; set; } // Comma-separated IDs
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }
    }
}
