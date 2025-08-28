using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class PayrollSettingRequest : BaseModel
    {
        public int Payroll_Setin_ID { get; set; }
        public int Company_ID { get; set; }
        public bool Payslip_Generation { get; set; }
        public int Payslip_Format { get; set; }
        public string PayslipNumber_Format { get; set; } // nchar(10)
        public int PaySlip_Number_Scope { get; set; }
        public string Initial_char { get; set; } = string.Empty;
        public bool Auto_Numbering { get; set; }
        public bool DigitalSignatur_Requirede { get; set; }
        public bool IsPayslipNo_Reset { get; set; }
        public bool PaySlipAutoEmail { get; set; }
        public int? CopyFromCompanyId { get; set; }
    }
}
