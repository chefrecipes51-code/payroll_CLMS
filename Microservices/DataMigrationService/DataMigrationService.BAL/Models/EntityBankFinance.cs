using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class EntityBankFinance
    {
        // File information
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }

        // List of data (UDT equivalent)
        public List<EntityBankFinanceUDT> EntityBankFinanceUDT { get; set; }


        // public string Department_Code { get; set; }
         public int CreatedBy { get; set; }
        public int Module_Id { get; set; }
        // public int CompanyId { get; set; }
        public int Log_Id { get; set; }
    }

 
    public class EntityBankFinanceUDT
    {
        public int Company_Id { get; set; }
        public string Company_Code { get; set; }
        public int Employee_Id { get; set; }
        public string PayrollNo { get; set; }
        public string BankName { get; set; }
        public int? Account_Type_Id { get; set; }
        public string IFSCCode { get; set; }
        public int BankBranch_Id { get; set; }
        public int NetPay_Share { get; set; }
        public decimal? NetPay_Share_Amount { get; set; }
        
        public int? BankTransferTemplate_Id { get; set; }
        public int? PaymentMethod { get; set; }
        public string OperationType { get; set; }
        public bool? Is_Imported { get; set; }
        public string ExternalUnique_Id { get; set; }
        public int Log_id { get; set; }
        public bool? IsError { get; set; }
    }

    public class EntityBankFinanceVerified : BaseModel
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

    public class EntityBankValidateUDT
    {
        public string PayrollNo { get; set; }
        public bool IsVerified { get; set; }
    }

}
