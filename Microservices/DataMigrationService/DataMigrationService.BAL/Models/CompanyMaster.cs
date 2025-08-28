using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class CompanyMasterDataMigration : BaseModel
    {
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public int CompanyId { get; set; }
        public int Group_Id { get; set; }
        public bool IsParent { get; set;}  
        public int CompanyLevel { get; set;}
        public int Location_ID { get; set;}
        public bool IsImported { get; set;}
        public int ExportLogId { get; set;}
        public int Log_Id { get; set;}

        public List<Company> CompanyList { get; set;}
    }
    public class Company
    {
        public int CompanyType_ID { get; set; }
        public string Company_Code { get; set; }
        public string CompanyName { get; set;}
        public string CompanyShortName { get; set;}
        public string CompanyPrintName { get; set;}
        public int ParentCompany_Id { get; set;}
        public bool Has_Subsidary { get; set;}
        public string ExternalUnique_Id { get; set;}
        //public bool IsError { get; set;}
        //public string ErrorMessage { get; set;}
    }
}
