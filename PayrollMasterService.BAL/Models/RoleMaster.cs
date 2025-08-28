using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class RoleMaster : BaseModel
    {
        public int Role_Id { get; set; }
        public string RoleName { get; set;}
        public bool GrantAdd { get; set;}
        public bool GrantEdit { get; set;}
        public bool GrantView { get; set;}
        public bool GrantDelete { get; set;}
        public bool GrantApprove { get; set;}
        public bool GrantRptPrint { get; set;}
        public bool GrantRptDownload { get; set;}
        public int OrgSequence { get; set;}
        public bool Import { get; set;}
        public bool DocDownload { get; set;}
        public bool DocUpload { get; set;}
    }
}
