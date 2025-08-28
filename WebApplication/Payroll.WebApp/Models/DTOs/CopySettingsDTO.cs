using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class CopySettingsDTO : BaseModel
    {
        public string? SelectParam { get; set; } // Comma-separated string like "1,2,3"
        public int CopyFromCompanyID { get; set; }
        public int CopyToCompanyID { get; set; }
    }
}
