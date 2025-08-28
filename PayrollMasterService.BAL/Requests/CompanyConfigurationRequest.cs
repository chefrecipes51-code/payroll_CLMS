using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Requests
{
    public class CompanyConfigurationRequest : BaseModel
    {
        public int CompanyId { get; set; }  // 0 means insert, >0 means update
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CurrencyId { get; set; }
    }
}
