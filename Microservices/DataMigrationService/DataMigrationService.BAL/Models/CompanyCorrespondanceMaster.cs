using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class CompanyCorrespondanceMaster : BaseModel
    {
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public int Log_Id { get; set; }
        public List<CompanyCorrespondance> companycorrespondanceList {  get; set; }
    }
    public class CompanyCorrespondance
    {
        public string Company_Code { get; set; }
        public string CompanyAddress { get; set; }
        public string Building_No { get; set; }
        public string Building_Name { get; set; }
        public string Street { get; set; }
        public int Country_ID { get; set; }
        public int State_Id { get; set; }
        public int City_ID { get; set; }
        public int Location_ID { get; set; }
        public string Primary_Phone_no { get; set; }
        public string Secondary_Phone_No { get; set; }
        public string Primary_Email_Id { get; set; }
        public string Secondary_Email_ID { get; set; }
        public string WebsiteUrl { get; set; }
        public string Company_LogoImage_Path { get; set; }
        public string ExternalUnique_Id { get; set; }
    }
}
