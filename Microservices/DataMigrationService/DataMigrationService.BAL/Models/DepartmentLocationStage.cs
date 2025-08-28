using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class DepartmentLocationStage : BaseModel
    {
        // File information
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }

        // List of department data (UDT equivalent)
        public List<DepartmentLocation> DepartmentLocation { get; set; }

       // public string Department_Code { get; set; }
       // public int CreatedBy { get; set; }
        public int Module_Id { get; set; }
        public int Company_Id { get; set; }
        public int Log_Id { get; set; }
    }


    public class DepartmentLocation
    {

        public int Company_Id { get; set; }
        public int Correspondance_ID { get; set; }
        public int? Department_Id { get; set; }
        public string Department_Code { get; set; }
        public int Area_Id { get; set; }
        public int Floor_Id { get; set; }

        public bool IsError { get; set; } // Used for validation feedback
        //public string ErrorRemarks { get; set; }
    }

   

}
