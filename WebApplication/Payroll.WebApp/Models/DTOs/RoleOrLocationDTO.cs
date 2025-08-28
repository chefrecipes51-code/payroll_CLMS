using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class RoleOrLocationDTO : BaseModel
    {
        public string UpdateType { get; set; }
        public int UserId { get; set; }
        public int? Role_User_Id { get; set; }
        public int? UserMapLocation_Id { get; set; }
        public List<UserCompanyDetailsDTO> CompanyDetails { get; set; } = new List<UserCompanyDetailsDTO>();
        public List<UserLocationDetailsDTO> LocationDetails { get; set; } = new List<UserLocationDetailsDTO>();
        public List<UserRoleDetailsDTO> RoleDetails { get; set; } = new List<UserRoleDetailsDTO>();
    }
}
