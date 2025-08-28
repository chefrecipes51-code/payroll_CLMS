using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BAL.Requests;

namespace UserService.BAL.Models
{
    public class UserRoleMapping : BaseModel
    {
        public int Role_User_Id { get; set; }
        public int Role_Menu_Header_Id { get; set; }


        public int? Role_Menu_Hdr_Id { get; set; } //Added by krunali gohil for payroll-377
        public string RoleName { get; set; } //Added by krunali gohil for payroll-377
        public int? Role_Id { get; set; } //Added by krunali gohil for payroll-377

        public int Company_Id { get; set; }
        public int User_Id { get; set; }
        public int Default_Role { get; set; }
        public List<int> Role_Ids { get; set; }
        public DateTime? Effective_From { get; set; }

    }
}

