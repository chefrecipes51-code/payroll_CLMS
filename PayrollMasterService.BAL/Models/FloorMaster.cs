using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class FloorMaster : BaseModel
    {
        public int Floor_Id { get; set; }
        public string Floor_No { get; set; }
    }
}
