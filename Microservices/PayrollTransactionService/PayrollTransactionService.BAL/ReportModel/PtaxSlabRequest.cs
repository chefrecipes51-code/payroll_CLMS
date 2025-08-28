using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class PtaxSlabRequest : BaseModel
    { 
        public int Ptax_Slab_Id { get; set; }
        public int Company_Id { get; set; }
        public string State_ID { get; set; } = string.Empty;
        public decimal Min_Income { get; set; }
        public decimal Max_Income { get; set; }
        public int Frequency { get; set; }
        public int? SpecialDeductionMonth { get; set; }
        public decimal PTaxAmt { get; set; }
        public decimal SpecialPTaxAmt { get; set; }
        public int? Gender { get; set; }
        public int? SrCitizenAge { get; set; }
        public bool Is_YearEnd_Adjustment { get; set; } = false;
        public DateTime Effective_From { get; set; }
        public DateTime? Effective_To { get; set; }
        public decimal Monthly_Tax { get; set; }           
    }
    public class TaxParamRequest : BaseModel
    {
        public int State_ID { get; set; }
        public int MonthSpecific { get; set; }
        public string Gender { get; set; } = string.Empty;
        public decimal SpecialAmt { get; set; }
        public int SrCitizrnExcemption { get; set; }
    }
    public class ProfessionalTaxSlabViewModel : BaseModel
    {
        public int Ptax_Slab_Id { get; set; }
        public int? Company_Id { get; set; }
        public string State_ID { get; set; } = string.Empty;
        public string? Frequency { get; set; } // From CASE expression in first SELECT
        public int? FrequencyId { get; set; }  // You can map actual Frequency value when needed
        public decimal Min_Income { get; set; }
        public decimal Max_Income { get; set; }
        public decimal PTaxAmt { get; set; }
        public decimal? SpecialPTaxAmt { get; set; }
        public int? Gender { get; set; }
        public int? SrCitizenAge { get; set; }
        public int? SpecialDeductionMonth { get; set; }
        public bool? Is_YearEnd_Adjustment { get; set; }
        public DateTime Effective_From { get; set; }
        public DateTime? Effective_To { get; set; }

    }


}
