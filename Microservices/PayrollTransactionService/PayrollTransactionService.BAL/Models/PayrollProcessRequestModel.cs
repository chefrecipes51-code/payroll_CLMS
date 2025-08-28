using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class PayrollProcessRequestModel
    {
        public int PayrollProcessId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public int CreatedBy { get; set; }
    }

    public class PayrollProcessOutputModel
    {
        public int Payroll_Process_Id { get; set; }
        public int Payroll_header_Id { get; set; }
    }

    public class PayrollProcessResultModel
    {
        public int PayrollProcessId { get; set; }
        public List<PayrollProcessOutputModel> Headers { get; set; } = new();
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
