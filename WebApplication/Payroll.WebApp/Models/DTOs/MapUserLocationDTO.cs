using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class MapUserLocationDTO : BaseModel
    {
        public List<UserMapLocationDTO> UserMapLocations { get; set; }

        public int UserMapLocation_Id { get; set; }
        public int User_ID { get; set; }
        public byte Company_Id { get; set; }
        public int Correspondance_ID { get; set; }
        public bool IsUserMapToLowLevel { get; set; }
        public string CompanyName { get; set; } // Added to match the stored procedure's output
        public string UserName { get; set; }   // Added to match the stored procedure's output
    }

    public class UserMapLocationDTO
    {
        public int UserMapLocation_Id { get; set; }
        public int User_ID { get; set; }
        public byte Company_Id { get; set; }
        public int Correspondance_ID { get; set; }
        public bool IsUserMapToLowLevel { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
