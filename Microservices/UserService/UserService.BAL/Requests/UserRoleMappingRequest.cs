using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Requests
{
    public class UserRoleMappingRequest : BaseModel 
    {
        public int Role_User_Id { get; set; }
        public int Role_Id { get; set; }
        public string RoleName { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
        public int Company_Id { get; set; }
        public int User_Id { get; set; }
        public int Role_Menu_Header_Id { get; set; }
        public DateTime Effective_From { get; set; }

    }
}
