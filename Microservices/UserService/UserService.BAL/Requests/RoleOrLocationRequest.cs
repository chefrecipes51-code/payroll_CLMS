using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Requests
{
    public class RoleOrLocationRequest : BaseModel
    {
        public string UpdateType { get; set; }  
        public int UserId { get; set; }        
        public int? Role_User_Id { get; set; } 
        public int? UserMapLocation_Id { get; set; }
        public List<UserCompanyDetails> CompanyDetails { get; set; } = new List<UserCompanyDetails>();
        public List<UserLocationDetails> LocationDetails { get; set; } = new List<UserLocationDetails>();
        public List<UserRoleDetails> RoleDetails { get; set; } = new List<UserRoleDetails>();

        // Added for need basis as in API CALL I am specifying the "var result = (await _dbConnection.QueryAsync<RoleOrLocationRequest>"
        public int ApplicationMessageType { get; set; } 
    }
}
