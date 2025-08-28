using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class CompanyCurrencyMaster : BaseModel
    {
        public int CompanyCurrency_Id { get; set; }
        public int Currency_Id { get; set; }
        public int Country_Id { get; set; }
        public string Currency_Name { get; set; }
        public string Currency_Symbol { get; set; }
        public string Currency_Description { get; set; }
        public int Currncy_decimalPoint { get; set; }
        public bool IsDefault { get; set; }
        public decimal Currency_Exchange_Rate { get; set; }

    }
}
