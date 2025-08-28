using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class SalaryStructureDropdownDTO : BaseModel
    {
        public int SalaryStructure_Hdr_Id { get; set; }
        public string SalaryStructureName { get; set; }

    }
}
