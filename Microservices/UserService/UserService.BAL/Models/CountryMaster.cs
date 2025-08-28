using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class CountryMaster : BaseModel
    {
        public int Country_Id { get; set; }
        public int ContinentId { get; set;}
        public string CountryCode { get; set;}
        public string CountryName { get; set;}
        public string TimeZone { get; set;}
        public int ContinentRegionId { get; set; }
        public string Stdcode { get; set; }

    }
}
