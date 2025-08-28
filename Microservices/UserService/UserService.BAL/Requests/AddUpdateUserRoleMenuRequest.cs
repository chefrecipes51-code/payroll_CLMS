using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Requests
{
    public class AddUpdateUserRoleMenuRequest : BaseModel
    {
        public int Role_User_Id { get; set; } // 0 for insert, >0 for update
        public int Role_Menu_Hdr_Id { get; set; }
        public int User_Id { get; set; }
        public int Company_Id { get; set; }
        public int Correspondance_Id { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public int CreatedBy { get; set; }
        public List<UserRoleMenuDetail> UserRoleMenuDetails { get; set; }
    }

    public class UserRoleMenuDetail
    {
        public int Role_User_Dtl_Id { get; set; }
        public int Role_Menu_Dtl_Id { get; set; }
        public int Menu_Id { get; set; }
        public bool HasPerDtl { get; set; }
        public bool GrantAdd { get; set; }
        public bool GrantEdit { get; set; }
        public bool GrantView { get; set; }
        public bool GrantDelete { get; set; }
        public bool GrantApprove { get; set; }
        public bool GrantRptPrint { get; set; }
        public bool GrantRptDownload { get; set; }
        public bool DocDownload { get; set; }
        public bool DocUpload { get; set; }
    }

}
