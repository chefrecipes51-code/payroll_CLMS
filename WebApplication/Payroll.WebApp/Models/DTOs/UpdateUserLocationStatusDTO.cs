using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class UpdateUserLocationStatusDTO : BaseModel
    {
        public int User_Id { get; set; }
        public int Correspondance_ID { get; set; }
        public bool ActualActivestatus { get; set; }
    }
}
