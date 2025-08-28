using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Requests
{
    public class GlGroupRequest : BaseModel
    {
        public int GL_Group_Id { get; set; }
        public string Group_Name { get; set; } = string.Empty;
        public int? Parent_GL_Group_Id { get; set; }
        public int Level { get; set; } = 1;
    }
}
