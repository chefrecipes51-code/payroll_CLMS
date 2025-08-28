using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class MapDepartmentRole : BaseModel
    {
        public int RoleDepartment_Id { get; set; }
        public int Role_Id { get; set; }
        public int Department_Id { get; set; } 
        public DateTime Effective_From_Dt { get; set; }
    }
}
