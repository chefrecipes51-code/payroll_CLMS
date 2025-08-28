using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class ThirdPartyParameterDTO : BaseModel
    {
        public int Clms_Param_ID { get; set; }
        public int? Company_Id { get; set; }
        public bool? DataSync { get; set; }
        public string Entityparam { get; set; } = string.Empty;
        public bool? Contractlabour_payment { get; set; }
        public bool? IsAttendanceProcessed { get; set; }
        public int? AttendanceProxcessType { get; set; }
        public int? Wo_Sync_Frequency { get; set; }
        public int? Entity_Sync_Frequency { get; set; }
        public bool? IntegratedLog_in { get; set; }
        public int? PayregisterFormat_ID { get; set; }
        public int? WorkOrder_Sync_Frequency { get; set; }
        public int? Cl_Sync_Frequency { get; set; }
        public int? CopyFromCompanyId { get; set; }
    }
}
