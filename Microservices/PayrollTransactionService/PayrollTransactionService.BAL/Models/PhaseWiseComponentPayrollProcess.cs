using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class PhaseWiseComponentPayrollProcess
    {
        public int Company_Id { get; set; }
        public int Payroll_Process_Id { get; set; }
        public int Payroll_Header_Id { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public int Process_Sequence_Id { get; set; }
        public int Payroll_Run_Type { get; set; }
       
    }
}
