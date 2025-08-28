using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Requests
{
    public class CurrencyRequest : BaseModel
    {
        public int Currency_Id { get; set; }
        public string CurrencyCode { get; set; }
        public int? CountryId { get; set; } // Not required in future it will be removed
        public string Currency_Name { get; set; }
        public string CurrencySymbol { get; set; }
    }
}
