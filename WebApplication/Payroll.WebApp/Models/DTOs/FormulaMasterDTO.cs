using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class FormulaMasterDTO : BaseModel
    {
        public int Formula_Id { get; set; }
        public string FormulaName { get; set; }
        public string Formula_Computation { get; set; }
        public int? Cmp_Id { get; set; }
        public string Formula_Code { get; set; }

    }
}