using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class MapWorkOrderContractor : BaseModel
    {
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public int Log_id { get; set; }
        public List<WorkOrderContractor> workOrderContractors { get; set; }
    }

    public class WorkOrderContractor
    {
        public string Contractor_Code { get; set; }
        public string Company_Code { get; set; }
        public int Company_Id { get; set; }
        public int WorkOrder_Id { get; set; }
        public int Contractor_Id { get; set; }
        public string Contractor_Name { get; set; }
        public string Contact_No { get; set; }
        public string Email_Id { get; set; }
        public int Location_Id { get; set; }
        public string ExternalUnique_Id { get; set; }
        public bool IsError { get; set; }
    }
}
