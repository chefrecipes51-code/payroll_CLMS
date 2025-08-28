using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class ServiceMaster : BaseModel
    {
        public int ServiceId { get; set; } 
        public int ModuleID { get; set;} 
        public string ServiceName { get; set;}
        public List<int> SelectedIds { get; set; }
    }
}
