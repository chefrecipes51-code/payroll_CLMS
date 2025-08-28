using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class EventAuthSetUp 
    {
        public List<EventAuthDetail> eventAuthDetails { get; set; } = new();  
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int MessageType { get; set; }                                // Message type
        public int MessageMode { get; set; }                                // Message mode
        public int ModuleId { get; set; }           
        public int CountryId { get; set; }// Module ID
    }
    public class EventAuthDetail 
    {
        public int Event_Id { get; set; }
        public int Role_Id { get; set; }          
        public int Auth_Level { get; set; }                 
        public int Approver_Id { get; set; }  
        public bool IsActive { get; set; }
    }

    public class EventAuthSelect : BaseModel
    {
        public int Event_Id { get; set; }
        public int Role_Id { get; set; }
        public int Auth_Level { get; set; }
        public int Approver_Id { get; set; }
        public string ModuleName { get; set; }
        public string EventName { get; set; }
    }
    public class EventAuthFilter
    {
     
        public int Event_Id { get; set; }
        public int Company_Id { get; set; }

        public int? Event_Auth_Id { get; set; }
        public int? Module_Id { get; set; }
        public int? Roll_Id { get; set; }

    }
}
