using Payroll.Common.ApplicationModel;
using PayrollMasterService.BAL.Models;

namespace Payroll.WebApp.Models.DTOs
{
    public class CompanyMasterDTO : BaseModel
    {
        public byte Company_Id { get; set; }
        public byte CompanyType_ID { get; set; }
        public string Company_Code { get; set; }
        public int Group_Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPrintName { get; set; }
        public bool IsParent { get; set; }
        public string CompanyShortName { get; set; }
        public byte ParentCompanyId { get; set; }
        public byte CompanyLevel { get; set; }
        public int Location_ID { get; set; }
        public bool Has_Subsidary { get; set; }

        // Relationships
        public CompanyCorrespondanceDTO CompanyCorrespondance { get; set; }
        public CompanyStatutoryDTO CompanyStatutory { get; set; }
        public int Currency_ID { get; set; }// Added By Harshida
    }
}
