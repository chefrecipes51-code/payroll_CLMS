using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class WageRateMaster : BaseModel    
    {
        public int WageRate_Id { get; set; }
        public string WageRateCode { get; set; }
        public string WageRateName { get; set; }
        public string WageRateDescription { get; set; }
        public int WageRateType { get; set; }
        public decimal WageRate { get; set; }
        public decimal CalculationHour { get; set; }
        public DateTime? EffectiveDate { get; set; }          
    }    
}