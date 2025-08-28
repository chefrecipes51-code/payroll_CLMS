using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class ServiceRequestDTO : BaseModel
    {
        public int ServiceId { get; set; }
        public List<int> SelectedIds { get; set; }
    }
}
