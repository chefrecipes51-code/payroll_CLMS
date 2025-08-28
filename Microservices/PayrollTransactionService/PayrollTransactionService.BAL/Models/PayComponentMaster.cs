using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class PayComponentMaster : BaseModel
    {
        public int EarningDeduction_Id { get; set; }
        public int Company_Id { get; set; }
        public bool Is_Child { get; set; }
        public int Parent_EarningDeduction_Id { get; set; }
        public bool IsEditable { get; set; }
        public string EarningDeductionName { get; set; }
        public int CalculationType { get; set; }
        public string CalculationTypeName { get; set; }
        public int EarningDeductionType { get; set; }
        public string EarningDeductionTypeName { get; set; }
        public string ParentEarningdeductionName { get; set; }
        public decimal MinimumUnit_value { get; set; }
        public decimal MaximumUnit_value { get; set; }
        public decimal Amount { get; set; }
        public string Formula { get; set; }
        public int Formula_Id { get; set; }
        public string? FormulaName { get; set; } //Added 02-05-2025
        
    }
}
