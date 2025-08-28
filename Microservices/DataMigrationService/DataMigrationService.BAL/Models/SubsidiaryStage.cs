using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class SubsidiaryStage : BaseModel 
    {
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public int Company_Id { get; set; }
        public int Location_ID { get; set; }
        public int Area_id { get; set; }
        public int Log_Id { get; set; }
        public bool IsVerified { get; set; } = false; 
        public int VerifiedBy { get; set; }
        public DateTime VerifiedDate { get; set; }
        public string OperationType { get; set; }
        public bool IsImported { get; set; } = true;
        public bool IsDeleted { get; set; } = false; 
        public bool IsRejected { get; set; }
        public List<SubsidiaryStageUDT> SubsidiariesStageUDT { get; set; }
    }
    public class SubsidiaryStageUDT
    {
        public int SubsidiaryType_Id { get; set; }
        public string Subsidiary_Code { get; set; }
        public string Subsidiary_Name { get; set; }
        public string Company_Code { get; set; }
        public string ExternalUnique_Id { get; set; } 
        public bool IsError { get; set; }
        public string ErrorRemarks { get; set; }
    }
}
