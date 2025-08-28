using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class EntityDocuments
    {
        // File information
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }

        // List of data (UDT equivalent)
        public List<EntityDocumentsUDT> EntityDocumentsUDT { get; set; }


        // public string Department_Code { get; set; }
         public int CreatedBy { get; set; }
        public int Module_Id { get; set; }
        // public int CompanyId { get; set; }
        public int Log_Id { get; set; }
    }

 
    public class EntityDocumentsUDT
    {
        public int Company_Id { get; set; }
        public string Company_Code { get; set; }
        public int Employee_Id { get; set; }
        public string PayrollNo { get; set; }
        public string DocumentName { get; set; }
        public int? Document_Type_Id { get; set; }
        public string DocumentPath { get; set; }
        public string OperationType { get; set; }
        public bool? Is_Imported { get; set; }
        public string ExternalUnique_Id { get; set; }
        public int? Log_id { get; set; }
    }
    public class EntityDocumentsVerified : BaseModel
    {
        // File information
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }

        // List of data (UDT equivalent)
        public List<EntityDocumentsValidateUDT> EntityDocumentsValidateUDT { get; set; }

        public int Module_Id { get; set; }
        // public int CompanyId { get; set; }
        public int Log_Id { get; set; }
    }

    public class EntityDocumentsValidateUDT
    {
        public string PayrollNo { get; set; }
        public bool IsVerified { get; set; }
    }

}
