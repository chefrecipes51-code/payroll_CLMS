using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class CityMaster : BaseModel
    {
        public int City_ID { get; set; }
        public int State_Id { get; set; }
        public string City_Name { get; set;}

    }
}
