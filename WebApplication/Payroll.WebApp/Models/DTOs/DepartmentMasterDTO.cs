using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class DepartmentMasterDTO : BaseModel
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
