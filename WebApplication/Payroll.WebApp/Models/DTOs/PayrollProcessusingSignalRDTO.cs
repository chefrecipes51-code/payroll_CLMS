namespace Payroll.WebApp.Models.DTOs
{
    public class PayrollProcessusingSignalRDTO
    {
        public int Total { get; set; }
        public int Remaining { get; set; }
        public int Completed { get; set; }
        public int Status { get; set; }
    }
    public class PayrollProcessusingSignalRRequest
    {
        public int Cmp_Id { get; set; }
        public int Month_Id { get; set; }
        public int Year_Id
        {
            get; set;
        }
    }

    public class PhaseWiseComponentPayrollProcessDTO
    {
        public int Company_Id { get; set; }
        public int Payroll_Process_Id { get; set; }
        public int Payroll_Header_Id { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public int Process_Sequence_Id { get; set; }
        public int Payroll_Run_Type { get; set; }

    }
    public class DynamicApiResponseModel
    {
        public bool IsSuccess { get; set; }
        public List<dynamic> Result { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
