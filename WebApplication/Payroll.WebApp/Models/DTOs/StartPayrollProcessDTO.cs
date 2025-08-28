using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class StartPayrollProcessDTO : BaseModel
    {
        public int Mode { get; set; }
        public int Cmp_Id { get; set; }
        public int Payroll_Period_Id { get; set; }
        public int Month_Id { get; set; }
        public int Year_Id { get; set; }
        public int Payroll_Process_Id { get; set; }
        public int Payroll_header_Id { get; set; }
        public int Process_Sequence_Id { get; set; }
        public string? SignalRConnectionId { get; set; }
        public int ProcessSequence_Id { get; set; }
       
    }
}
