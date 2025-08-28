using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class ContractMaster : BaseModel
    {
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public List<Contracts> contracts { get; set; }
    }
    public class Contracts
    {
        public int Contract_Id { get; set; }
        public int Company_Id { get; set; }
        public string Contract_Code { get; set; }
        public string Company_Code { get; set; }
        public string Contract_Name { get; set; }
        public string Contract_Description { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public int Contract_Status { get; set; }
        public int Contract_Type { get; set; }
        public decimal Contract_Value { get; set; }
        public int Payment_Terms { get; set; }
        public decimal Bank_Guarantee_Amount { get; set; }
        public bool Is_Joint_Venture { get; set; }
        public bool Is_Sublet_Allowed { get; set; }
        public bool IsError { get; set; }
    }
}
