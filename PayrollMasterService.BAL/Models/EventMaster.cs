using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class EventMaster : BaseModel
    {
        public int Event_Id { get; set; }
        public string EventName { get; set; }
        public int Module_Id { get; set; }
        public int Company_Id { get; set; }
        public string EventTable { get; set; }
        public bool AutoUpdate { get; set; }
        public bool AuthRequired { get; set; }
        public bool AuthLevel { get; set; }
        public bool CanDeligate { get; set; }
        public bool Send_Email_For_Auth { get; set; }
        public bool Send_Email_Post_Auth { get; set; }
        public bool SendNotification { get; set; }
        public string KeyField1 { get; set; }
        public string UpdateField1 { get; set; }
        public string UpdateField2 { get; set; }
        public string UpdateField3 { get; set; }
        public int EventBehaviour { get; set; }
    }
}
