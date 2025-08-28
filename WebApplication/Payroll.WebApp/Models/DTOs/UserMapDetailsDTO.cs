using Newtonsoft.Json;
using UserService.BAL.Models;

namespace Payroll.WebApp.Models.DTOs
{
    

    public class UserMapDetailsDTO
    {
        public int User_id { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public int userType_Id { get; set; }  // Matches userType_Id from SP
        public string ContactNo { get; set; }
        public string stdcode { get; set; }
        public string Country_Id { get; set; }
        public List<int> Company_Ids { get; set; } = new List<int>();  // List<int> for company IDs
        public List<LocationDetailDTO> LocationDetails { get; set; } = new List<LocationDetailDTO>();
        public List<UserRoleDTO> UserRoles { get; set; } = new List<UserRoleDTO>();
    }

    public class LocationDetailDTO
    {
        public int Location_ID { get; set; }
        public int Correspondance_ID { get; set; }
        public int Country_ID { get; set; }
        public int State_Id { get; set; }
        public int City_ID { get; set; }
        public string LocationName { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserRoleDTO
    {
        public int Role_User_Id { get; set; }
        public int Role_Id { get; set; }
        public int Role_Menu_Header_Id { get; set; }
        public string Effective_From { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
    public class UserRoleResponseModel
    {
        [JsonProperty("locationWiseRoles")]
        public List<LocationWiseRole> LocationWiseRoles { get; set; }

        [JsonProperty("roleMenuHeaders")]
        public List<RoleMenuHeader> RoleMenuHeaders { get; set; }
    }
    public class LocationWiseRoleDTO
    {
        [JsonProperty("locationName")]
        public string LocationName { get; set; }
        [JsonProperty("correspondance_Id")]
        public int correspondance_Id { get; set; }
        [JsonProperty("role_User_Id")]
        public int Role_User_Id { get; set; }
        [JsonProperty("role_Id")]
        public int Role_Id { get; set; }
        [JsonProperty("role_Menu_Header_Id")]
        public int Role_Menu_Header_Id { get; set; }
        [JsonProperty("effective_From")]
        public string Effective_From { get; set; }
        [JsonProperty("roleName")]
        public string RoleName { get; set; }
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
    }

    public class RoleMenuHeaderDTO
    {
        [JsonProperty("role_Menu_Hdr_Id")]
        public int Role_Menu_Hdr_Id { get; set; }
        [JsonProperty("roleName")]
        public string RoleName { get; set; }
    }
}
