using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class StateMaster : BaseModel
    {
        public int State_Id { get; set; }
        public int CountryId { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
    }
}
