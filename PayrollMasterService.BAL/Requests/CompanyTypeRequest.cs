using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Requests
{
    public class CompanyTypeRequest : BaseModel
    {
        public byte CompanyType_ID { get; set; } // Using byte for tinyint
        public string CompanyType_Name { get; set; }
    }
}
