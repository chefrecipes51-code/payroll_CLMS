using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class ModuleMaster : BaseModel
    {
        public int Module_Id { get; set; }
        public string ModuleName { get; set;}

    }
}
