using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class LocationMaster : BaseModel
    {
        public int Location_Id { get; set; } // Primary key, auto-incremented
        public int CityId { get; set; } // Foreign key, required
        public string LocationName { get; set; }
        public string City_Name { get; set; }
        public int State_Id { get; set; }
        public int CountryId { get; set; }
    }
}
