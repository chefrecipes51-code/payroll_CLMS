using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class PayrollTranStgDataRequest : BaseModel
    {
        public string Payroll_Process_No { get; set; }
        public int Entity_ID { get; set; }
        public DateTime Payroll_Month { get; set; }
        public DateTime Attendance_Month { get; set; }
        public int Days_Present { get; set; }
        public int Weekly_Offs { get; set; }
        public int Holidays { get; set; }
        public int Paid_Leaves { get; set; }
        public int Unpaid_Leaves { get; set; }
        public int Total_Working_Days { get; set; }
        public int Total_Ot_Hrs { get; set; }      
        public int ProcessStage { get; set; } = 0;
        public int? Process_Sequence_ID { get; set; }
        public bool IsLocked { get; set; } = false;
    }

    public class PayrollStgData : BaseModel
    {      
        public int? Contractor_ID { get; set; }
        public int Entity_ID { get; set; }
    }
    public class PayrollStgDataStage 
    {
        public int Contractor_ID { get; set; }
        public int Entity_ID { get; set; }
    }
    public class SavePayrollStagingRequestModel
    {
        public int Month_Id { get; set; }
        public int Year_Id { get; set; }
        public int CreatedBy { get; set; }
        public List<PayrollStgData> PayrollData { get; set; } = new();
        public string? StatusMessage { get; set; }
        public int? MessageType { get; set; }
    }

}
