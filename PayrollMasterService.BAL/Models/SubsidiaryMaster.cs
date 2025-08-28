using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class SubsidiaryMaster : BaseModel
    {
        public int Subsidiary_Id { get; set; }
        public int SubsidiaryType_Id { get; set; }
        public string Subsidiary_Code { get; set; }
        public string Subsidiary_Name { get; set; }

        public int Company_Id { get; set; }
        public List<string> Companies { get; set; }

        public string CompanyName { get; set; }
        public string LocationName { get; set; }
        public string? Company_Code { get; set; }
        public int CountryId { get; set; }

        public int Location_ID { get; set; }
        //public List<UserMapLocation> UserMapLocation { get; set; } = new List<UserMapLocation>(); // For `@UserMapLocation_Id`

        public List<int> Department { get; set; }
        public List<string> Role { get; set; }
        public string Country { get; set; }
        public string State { get; set; }

        public int State_Id { get; set; }
        public string Branch { get; set; }
        public int cityid { get; set; }

        public int Area_id { get; set; }
        public string Externalunique_Id { get; set; }
        //public int Log_Id { get; set; }
    }
}
