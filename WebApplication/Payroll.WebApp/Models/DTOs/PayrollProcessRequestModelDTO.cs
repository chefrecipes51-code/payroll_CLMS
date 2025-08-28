namespace Payroll.WebApp.Models.DTOs
{
    public class PayrollProcessRequestModelDTO
    {
        public int PayrollProcessId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public int CreatedBy { get; set; }
    }

    public class PayrollProcessOutputModelDTO
    {
        public int Payroll_Process_Id { get; set; }
        public int Payroll_header_Id { get; set; }
    }

    public class PayrollProcessResultModelDTO
    {
        public int PayrollProcessId { get; set; }
        public List<PayrollProcessOutputModelDTO> Headers { get; set; } = new();
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
