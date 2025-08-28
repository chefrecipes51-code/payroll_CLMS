using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class ServiceImportMaster : BaseModel
    {
        public int Service_Id { get; set; } 
        public int ServiceId { get; set; } 
        public int ServiceType { get; set;} 
        public string ServiceName { get; set;} 
        public string TemplateFilePath { get; set;}  
        public string HelpFilePath { get; set;}
        public string DataFilePath { get; set;}
    }
}
