using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class EntityInsurance
    {
        // File information
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }

        // List of data (UDT equivalent)
        public List<EntityInsuranceUDT> EntityInsuranceUDT { get; set; }


        // public string Department_Code { get; set; }
         public int CreatedBy { get; set; }
        public int Module_Id { get; set; }
        // public int CompanyId { get; set; }
        public int Log_Id { get; set; }
    }

 
    public class EntityInsuranceUDT
    {
        public int Company_Id { get; set; }
        public string Company_Code { get; set; }
        public int Employee_Id { get; set; }
        public string PayrollNo { get; set; }
        public string Ins_Company_Name { get; set; }
        public int PolicyHolder { get; set; }
        public int Age { get; set; }
        public int? Policy_Paid_By { get; set; }
        public DateTime Commencement_Date { get; set; }
        public int Payment_Premium_Day { get; set; }
        public string PremiumPaidTill_Month { get; set; }
        public int Purpose_of_Policy_Id { get; set; }
        public string PolicyFile_No { get; set; }
        public DateTime? Maturity_Date { get; set; }
        public decimal? MonthlyPremium_Paid { get; set; }
        public int Log_Id { get; set; }
        public string ExternalUnique_Id { get; set; }
        public string OperationType { get; set; }
        public bool? Is_Imported { get; set; }
        public bool? IsError { get; set; }
    }

    public class EntityInsuranceVerified : BaseModel
    {
        // File information
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }

        // List of data (UDT equivalent)
        public List<EntityBankValidateUDT> EntityBankValidateUDT { get; set; }

        public int Module_Id { get; set; }
        // public int CompanyId { get; set; }
        public int Log_Id { get; set; }
    }

    public class EntityInsuranceValidateUDT
    {
        public string PayrollNo { get; set; }
        public bool IsVerified { get; set; }
    }

}
