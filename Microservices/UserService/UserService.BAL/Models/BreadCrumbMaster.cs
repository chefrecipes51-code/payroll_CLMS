using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class BreadCrumbMaster : BaseModel
    {
        public int Breadcrumb_Id { get; set; }
        public int MenuGroup_Id { get; set; }
        public string Title { get; set; }
        public string Action_URL { get; set; }
        public int DisplayOrder { get; set; }

    }
}
