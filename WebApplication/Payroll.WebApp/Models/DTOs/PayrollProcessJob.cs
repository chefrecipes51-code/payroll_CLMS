namespace Payroll.WebApp.Models.DTOs
{
    public class PayrollProcessJob
    {
        public int CompanyId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public int Mode { get; set; }
        public string SignalRConnectionId { get; set; }
    }
    public class PayrollProgressUpdate
    {
        public int Total { get; set; }
        public int Completed { get; set; }
        public int Remaining => Total - Completed;
        public int Percent => Total == 0 ? 0 : (Completed * 100) / Total;
    }
}
