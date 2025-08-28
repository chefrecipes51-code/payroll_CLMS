using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class CompanyConfigurationDTO : BaseModel
    {
        public int CompanyId { get; set; }  // 0 means insert, >0 means update
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CurrencyId { get; set; }
    }
}
