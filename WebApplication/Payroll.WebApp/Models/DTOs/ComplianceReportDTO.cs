namespace Payroll.WebApp.Models.DTOs
{
    public class ComplianceReportDTO
    {

        public int CompanyID { get; set; }
        public string CompanyLocationIDs { get; set; }
        public string ContractorIDs { get; set; }
        public string EntityIDs { get; set; }
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }

    }
}
