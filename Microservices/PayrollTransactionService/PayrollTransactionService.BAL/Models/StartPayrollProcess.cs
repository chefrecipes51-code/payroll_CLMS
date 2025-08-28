using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class StartPayrollProcess : BaseModel
    {
        public int Mode { get; set; }
        public int Cmp_Id { get; set; }
        public int Month_Id { get; set; }
        public int Year_Id { get; set; }
        public int Payroll_Process_Id { get; set; }
        public int Payroll_header_Id { get; set; }
        public int Process_Sequence_Id { get; set; }
        public string? SignalRConnectionId { get; set; }

    }
}
