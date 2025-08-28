using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class ContractorStage : BaseModel
    {
        // File information
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }

        // List of department data (UDT equivalent)
        public List<ContractorUDT> ContractorUDTList { get; set; }

       // public string Department_Code { get; set; }
       // public int CreatedBy { get; set; }
        public int Module_Id { get; set; }
       // public int CompanyId { get; set; }
        public int Log_Id { get; set; }
    }


    public class ContractorUDT
    {

        public string Contractor_Code { get; set; }
        public int Company_Id { get; set; }
        public string Company_Code { get; set; }
        public int Contract_Id { get; set; }
        public string Contractor_Name { get; set; }
        public string Contact_No { get; set; }
        public string Email_Id { get; set; }
        public string Address { get; set; }
        public string License_No { get; set; }
        public int Max_Labour_Count { get; set; }
        public int Assigned_Labour_Count { get; set; }
        public int City_Id { get; set; }
        public int State_Id { get; set; }
        public int District_Id { get; set; }
        public int Country_Id { get; set; }
        public int Location_Id { get; set; }
        public int BankBranch_Id { get; set; }
        public int BankAccountNo { get; set; }
        public int Bank_Id { get; set; }
        public string Profile_Photo { get; set; }
        public string ProfilePhoto_Path { get; set; }
        public string LIN_No { get; set; }
        public string PAN_No { get; set; }
        public string TAN_No { get; set; }
        public string EPF_No { get; set; }
        public string ESIC_No { get; set; }
        public string Bank { get; set; }
        public bool Is_SubContractor { get; set; }
        public int ExportLogId { get; set; }
        public bool? IsError { get; set; }
        public int? Parent_Contractor_Id { get; set; }
        

    }

   

}
