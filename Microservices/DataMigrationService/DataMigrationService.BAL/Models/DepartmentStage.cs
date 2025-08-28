using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class DepartmentStage : BaseModel 
    {
        // File information
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }
        public int CompanyId { get; set; }
        public int Log_Id { get; set; }

        // List of department data (UDT equivalent)
        public List<Department> Departments { get; set; }
    }
    public class Department
    {
        public string DepartmentCode { get; set; } // NVARCHAR(50)
        public string DepartmentName { get; set; } // NVARCHAR(100)
        public string ExternalUnique_Id { get; set; } // NVARCHAR(10)
        public bool IsError { get; set; } // Used for validation feedback
        public string ErrorRemarks { get; set; } 
    }

}
