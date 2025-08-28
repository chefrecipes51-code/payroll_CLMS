using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class DepartmentMaster : BaseModel
    {
        public int Department_Id { get; set; }
        public string DepartmentCode { get; set; }
        public string ExternalUnique_Id { get; set; }
        public string DepartmentName { get; set; }
        public bool ExternalData { get; set; }
        public bool Isimporterd { get; set; }
        public int ExportLogId { get; set; }
    }
}
