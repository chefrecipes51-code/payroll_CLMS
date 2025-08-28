using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class EarningDeductionMasterDto : BaseModel
    {
        public int EarningDeduction_Id { get; set; }
        public string EarningDeductionName { get; set; }
        public int CalculationType { get; set; }
        public int EarningDeductionType { get; set; }
        public bool Taxable { get; set; }
        public bool Exempted { get; set; }
        public bool AllowSingleEntry { get; set; }
        public bool UseInSalaryRevision { get; set; }
        public int TaxFactorPercentage { get; set; }
        public decimal MinimumUnitValue { get; set; }
        public decimal MaximumUnitValue { get; set; }
        public string Formula { get; set; }
        public bool IncludeInPension { get; set; }
        public int EarnDedIdForUnit { get; set; }
        public int EarnDedIdForRate { get; set; }
    }
}
