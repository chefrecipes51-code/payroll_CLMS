using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class AreaMaster : BaseModel
    {
        public int Area_Id { get; set; } // Primary key, auto-incremented
        public int Location_Id { get; set; } // Foreign key
        public string AreaName { get; set; }
        public string LocationName { get; set; }
        public int cityid { get; set; }
        public int State_Id { get; set; }
        public int CountryId { get; set; }
    }
}
