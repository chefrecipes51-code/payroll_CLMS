using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class UserMapDetailModel
    {
        public int User_id { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int userType_Id { get; set; }  // Matches userType_Id from SP
        public string ContactNo { get; set; }
        public string stdcode { get; set; }
        public string Country_Id { get; set; }
        public List<int> Company_Ids { get; set; } = new List<int>();  // List<int> for company IDs
        public List<LocationDetailModel> LocationDetails { get; set; } = new List<LocationDetailModel>();
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class LocationDetailModel
    {
        public int Location_ID { get; set; }
        public int Correspondance_ID { get; set; }
        public int Country_ID { get; set; }
        public int State_Id { get; set; }
        public int City_ID { get; set; }
        public string LocationName { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserRole
    {

        public int Role_User_Id { get; set; }
        public int Role_Id { get; set; }
        public int Role_Menu_Header_Id { get; set; }
        public string Effective_From { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }

    public class LocationWiseRole
    {
        public string LocationName { get; set; }
        public int correspondance_Id { get; set; }
        public int Role_User_Id { get; set; }
        public int Role_Id { get; set; }
        public int Role_Menu_Header_Id { get; set; }
        public string Effective_From { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
    public class RoleMenuHeader
    {
        public int Role_Menu_Hdr_Id { get; set; }
        public string RoleName { get; set; }
    }
}
