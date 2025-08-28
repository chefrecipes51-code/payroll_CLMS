using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class LocationMasterDTO : BaseModel
    {
        public int Location_Id { get; set; } // Primary key, auto-incremented
        public int CityId { get; set; } // Foreign key, required
        public string LocationName { get; set; }
        public string City_Name { get; set; }
        public int State_Id { get; set; }
        public int CountryId { get; set; }
    }
}
