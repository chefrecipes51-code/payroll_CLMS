using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Requests
{
    public class AccountingHeadRequest : BaseModel
    {
        public int Accounting_Head_Id { get; set; }
        public string Accounting_Head_Name { get; set; } = string.Empty;
        public string GL_Code { get; set; } = string.Empty;
        public string Tran_Type_Name { get; set; } = string.Empty;
        public int Account_Type { get; set; } 
        public int Tran_Type { get; set; }
        public int Parent_Gl_Group_Id { get; set; } 
        public int? Sub_Gl_Group_Id { get; set; }  
        public string? Parent_Gl_Group_Name { get; set; } = string.Empty;
        public string? Sub_Gl_Group_Name { get; set; } = string.Empty;
    }
    public class AccountType : BaseModel
    {
        public int AccountType_ID { get; set; }
        public string? AccountType_Name { get; set; }       
    }
    public class GLGroup 
    {
        public int GL_Group_Id { get; set; }
        public string? Group_Name { get; set; }
    }
}
