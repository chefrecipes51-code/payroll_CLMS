using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class EarningDeductionMaster : BaseModel
    {
        public int EarningDeduction_Id { get; set; }
        public int Company_Id { get; set; }
        public string EarningDeductionName { get; set; }
        public int CalculationType { get; set; }
        public int EarningDeductionType { get; set; }
        //public bool Taxable { get; set; }
        //public bool Exempted { get; set; }
        //public bool AllowSingleEntry { get; set; }
        //public bool UseInSalaryRevision { get; set; }
        //public int TaxFactorPercentage { get; set; }
        public decimal MinimumUnit_value { get; set; }
        public decimal MaximumUnit_value { get; set; }
        public decimal Amount { get; set; }
        public string Formula { get; set; }
        public int Formula_Id { get; set; }
        //public bool IncludeInPension { get; set; }
        //public bool CostCentre_Applicable { get; set; }
        //public int EarnDed_Id_ForUnit { get; set; }
        //public bool Tax_In_Gross { get; set; }
        //public int Amount_Type { get; set; }
        //public int EarnDed_Id_ForRate { get; set; }
    }
}
