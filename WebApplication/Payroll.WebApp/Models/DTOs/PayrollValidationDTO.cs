namespace Payroll.WebApp.Models.DTOs
{
    public class PayrollValidationDTO
    {
    }
    public class CompanyPayrollValidationDTO
    {
        public int Company_Id { get; set; }
        public string CompanyName { get; set; }
    }
    public class CompanyLocationPayrollValidationDTO
    {
        public int Location_ID { get; set; }
        public string LocationName { get; set; }
    }
    public class ContractorPayrollValidationDTO
    {
        public int Contractor_ID { get; set; }
        public string Contractor_Name { get; set; }
    }
    public class WorkOrderPayrollValidationDTO
    {
        public int WorkOrder_Id { get; set; }
        public string WorkOrder_No { get; set; }
    }
    public class PreviousMonthYearPeriodDTO
    {
        public int month_Id { get; set; }
        public string year { get; set; }
    }
}
