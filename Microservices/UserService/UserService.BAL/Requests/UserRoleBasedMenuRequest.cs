using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Requests
{
    public class UserRoleBasedMenuRequest
    {
        public int Menu_Id { get; set; }
        public string MenuName { get; set; }
        public string ActionUrl { get; set; }
        public int? ParentMenuId { get; set; }
        public int Level { get; set; }
        public int DisplayOrder { get; set; }
        public bool HasPerDtl { get; set; } // payroll-377 krunali gohil

        public bool GrantAdd { get; set; } // payroll-377 krunali gohil
        public bool GrantView { get; set; } // payroll-377 krunali gohil

        public bool GrantEdit { get; set; } //payroll-377 krunali gohil

        public bool GrantApprove { get; set; } // payroll-377 krunali gohil
        public bool GrantDelete { get; set; } // payroll-377 krunali gohil
        public bool GrantRptDownload { get; set; } // payroll-377 krunali gohil

        public bool GrantRptPrint { get; set; } // payroll-377 krunali gohil
    }
}
