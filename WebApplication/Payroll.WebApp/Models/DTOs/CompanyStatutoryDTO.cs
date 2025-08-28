using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class CompanyStatutoryDTO : BaseModel
    {
        public int Statutory_Type_Id { get; set; }
        public byte Company_Id { get; set; }
        public string StatutoryType_Name { get; set; }
    }
}
