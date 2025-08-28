using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class RoleMenuMappingHeader : BaseModel
    {
        public int Role_Menu_Hdr_Id { get; set; }
        public int Company_Id { get; set; }
        public int Role_Id { get; set; }
        public DateTime EffectiveFrom { get; set; }
    }
    public class RoleMenuDetail : BaseModel
    {
        public int Role_Menu_Dtl_Id { get; set; }
        public int Role_Menu_Hdr_Id { get; set; }          // Detail ID
        public int Menu_Id { get; set; }                 // Menu ID
        public int Company_Id { get; set; }              // Company ID
    }
    public class RoleMenuMappingRequest : BaseModel
    {
        public RoleMenuMappingHeader Header { get; set; }                // Header data
        public List<RoleMenuDetail> Details { get; set; } = new();       // List of detail data
        public int MessageType { get; set; }                                // Message type
        public int MessageMode { get; set; }                                // Message mode
        public int ModuleId { get; set; }                                   // Module ID
    }

    public class UserRoleMenu
    {
        public int Menu_Id { get; set; }
        public string MenuName { get; set; }
        public string ActionURL { get; set; }
        public int ParentMenuId { get; set; }
        public int Level { get; set; }
        public int DisplayOrder { get; set; }
        public bool HasPerDtl { get; set; }
        public int Role_Menu_Hdr_Id { get; set; } //Added By Harshida 16-01-'25
        public int Role_Menu_Dtl_Id { get; set; }//Added By Harshida 16-01-'25
        public bool GrantAdd { get; set; }
        public bool GrantView { get; set; }
        public bool GrantEdit { get; set; }
        public bool GrantDelete { get; set; }
        public bool GrantRptPrint { get; set; }
        public bool GrantRptDownload { get; set; }
        public bool DocDownload { get; set; }
        public bool DocUpload { get; set; }
    }

    //public class UserRoleMenuHeader
    //{
    //    public string MenuName { get; set; }
    //    public string ActionURL { get; set; }
    //    public int ParentMenuId { get; set; }
    //    public int Level { get; set; }
    //    public List<UserRoleMenuHeader> SubMenus { get; set; } = new List<UserRoleMenuHeader>();
    //}
}
