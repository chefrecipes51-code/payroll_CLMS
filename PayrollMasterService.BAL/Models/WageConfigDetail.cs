using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class WageConfigDetail : BaseModel
    {
        public int WageConfig_Dtl_Id { get; set; }
        public int Wage_Id { get; set; }
        public decimal WageSalaryBasic { get; set; }
        public int PaymentModeId { get; set; }
        public bool IsHRAapplicable { get; set; }
        public bool? HRAallownceType { get; set; }
        public int? HRAallowncePer { get; set; }
        public decimal? HRAallownceAmt { get; set; }
        public bool Is_PFApplicable { get; set; }
        public int? PF_DeductionType { get; set; }
        public int? PF_CalculateOn { get; set; }
        public decimal? PF_Employee_Amt { get; set; }
        public decimal? PF_Employer_Amt { get; set; }
        public bool Is_PensionApplicable { get; set; }
        public int? Pension_DeductionType { get; set; }
        public int? Pension_CalculateOn { get; set; }
        public decimal? Pension_Employee_Amt { get; set; }
        public decimal? Pension_Employer_Amt { get; set; }
        public int? Wage_Grade_Type { get; set; }
        public bool Is_HRAApplicable { get; set; }
        public int? HRA_AllowanceType { get; set; }
        public int? HRA_Percentage { get; set; }
        public decimal? HRA_Amount { get; set; }
        public int? Earning_Dedcution_Id { get; set; }
        public bool? NotInUse { get; set; }

        public int? SkillTypeId { get; set; }
        public int? ContractorId { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int? HRACalculateOn { get; set; }
    }
}
