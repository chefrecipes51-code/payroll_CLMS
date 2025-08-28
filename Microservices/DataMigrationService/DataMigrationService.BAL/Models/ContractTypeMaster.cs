using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class ContractTypeMaster : BaseModel
    {  // File information
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public int CompanyId { get; set; }

        // List of department data (UDT equivalent)
        public List<ContractType> ContractTypes { get; set; }
    }
    public class ContractType
    {
        public string ContractType_Code { get; set; }
        public string ContractType_Name { get; set; }
        public bool IsError { get; set; }
    }
}
