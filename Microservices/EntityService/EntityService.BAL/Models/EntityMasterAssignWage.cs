using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityService.BAL.Models
{
    public class EntityMasterAssignWage : BaseModel
    {
        public int Employee_Id { get; set; }
        public int Wage_Id { get; set; }
    }

}
