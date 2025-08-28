using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
 
    public class ContractorDocumentMaster : BaseModel
    {  
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public int Company_Id { get; set; }
        public List<ContractDocument> ContractDocumentTypes { get; set; }
    }
    public class ContractDocument
    {
        public int Contractor_Id { get; set; }
        public string Company_Code { get; set; }
        public string DocumentName { get; set; }
        public int Document_Type_Id { get; set; }
        public string DocumentPath { get; set; }
        public string ExternalUnique_Id { get; set; }
        public bool IsError { get; set; }
        public string ErrorRemarks { get; set; }
    }
    public class ContractDocumentFTP : BaseModel
    {
        public int Contractor_Doc_Id { get; set; }       
        public int Contractor_Id { get; set; }
        public string DocumentPath { get; set; }       
    }
}
