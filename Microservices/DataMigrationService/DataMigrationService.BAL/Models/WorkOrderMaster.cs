using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class WorkOrderMaster : BaseModel
    {
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public int CompanyId { get; set; }
        public int Log_id { get; set; }
        public int WorkOrder_Id {  get; set; }
        public List<WorkOrder> workOrders { get; set; }
    }
    public class WorkOrder
    {
        public int Company_Id { get; set; }
        public string Company_Code { get; set; }
        public int Contract_Id { get; set; }
        public string WorkOrder_No { get; set; }
        public int WorkOrder_Code { get; set; }
        public string Work_Description { get; set; }
        public DateTime Wo_Start_Date { get; set; }
        public DateTime Wo_End_Date { get; set; }
        public DateTime Wo_New_Date { get; set; }
        public decimal Estimated_Cost { get; set; }
        public decimal Actual_Cost { get; set; }
        public int Required_Menpower { get; set; }
        public int Total_Menpower { get; set; }
        public string Wo_FileName { get; set; }
        public string Wo_FilePath { get; set; }
        public int Status { get; set; }
        public int Compliance_Status { get; set; }
        public string ExternalUnique_Id { get; set; }
        public bool IsError { get; set; }
    }
}
