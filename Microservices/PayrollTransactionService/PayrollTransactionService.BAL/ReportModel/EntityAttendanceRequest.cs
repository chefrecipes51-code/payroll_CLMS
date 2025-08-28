using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class EntityAttendanceRequest
    {
        public int Attendance_Id { get; set; }
        public int Entity_ID { get; set; }
        public int Contractor_Id { get; set; }
        public int Log_Id { get; set; }
        public int Company_ID { get; set; }
        public int Correspondance_ID { get; set; }
        public string Entity_Name { get; set; }
        public string Contractor_Name { get; set; }
        public DateTime PayMonthPeriod_StartDt { get; set; }
        public DateTime PayMonthPeriod_EndDt { get; set; }
        public int Total_Days { get; set; }
        public int Present_Days { get; set; }
        public int Absent_Days { get; set; }
        public int Week_Off_Days { get; set; }
        public int Holiday_Days { get; set; }
        public int Leave_Days { get; set; }
        public int Comp_Off_Days { get; set; }
        public int Work_On_Holiday_Days { get; set; }
        public int Work_on_Week_Of { get; set; }
        public decimal Total_OT_Hours { get; set; }
        public string Ot_Shift_Code { get; set; }
        public bool IsActive { get; set; }
        public int Datasequence { get; set; }
    }
    public class EntityAttendanceIdModel
    {
        public int Entity_ID { get; set; }
    }   
    public class EntityAttendanceRequestModel : BaseModel
    {
        public int CompanyId { get; set; }
        public List<int> EntityIds { get; set; }
    }
    public class EntityAttendanceUpdateRequest : BaseModel
    {
        public int? CompanyId { get; set; }
        public List<int> EntityAttendanceIds { get; set; } = new();
    }
    public class AttendanceSaveModel
    {
        public int ContractorId { get; set; }
        public int EntityId { get; set; }
    }

}
