using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Payroll.Common.EnumUtility.EnumUtility;

namespace PayrollMasterService.BAL.Models
{
    public class SalaryStructure : BaseModel
    {

        public int SalaryStructure_Hdr_Id { get; set; }
        public int FinancialYr_Id { get; set; }
        public int Company_Id { get; set; }
        public int Correspondance_ID { get; set; }
        public string SalaryStructureName { get; set; }
        public Nullable<DateTime> EffectiveFrom { get; set; }
        public Nullable<DateTime> Effective_To { get; set; }
        public int SalaryFrequency { get; set; }
        public int PayGradeConfig_Id { get; set; }
        public Nullable<decimal> MinSalary { get; set; }
        public Nullable<decimal> MaxSalary { get; set; }
        public int SalaryBasis { get; set; }



    }

    public class SalaryStructureDTO : BaseModel
    {
        public int SalaryStructure_Hdr_Id { get; set; }
        public int FinancialYr_Id { get; set; }
        public int Company_Id { get; set; }
        public int Correspondance_ID { get; set; }
        public string SalaryStructureName { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? Effective_To { get; set; }
        public int SalaryFrequency { get; set; }
        public int PayGradeConfig_Id { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public int SalaryBasis { get; set; }
        public string PayGradeConfigName { get; set; }
        public string SalaryFrequencyName
        {
            get
            {
                return Enum.IsDefined(typeof(SalaryFrequencyEnum), SalaryFrequency)
                    ? ((SalaryFrequencyEnum)SalaryFrequency).ToString()
                    : "Unknown";
            }
        }
        public decimal SimulatedAmount { get; set; }
        //public bool IsActive { get; set; }
        //public int CreatedBy { get; set; }
        //public int? UpdatedBy { get; set; }
        public List<SalaryStructureDetailDTO> SalaryStructureDetails { get; set; }
        public List<SalarySimulatorTotalDTO> SalarySimulatorTotal { get; set; }
    }

    public class SalaryStructureDetailDTO
    {


        public int SalaryStructure_Dtl_Id { get; set; }
        public int SalaryStructure_Hdr_Id { get; set; }
        public int EarningDeductionID { get; set; }
        public int SubEarningDeductionID { get; set; }
        public string EarningDeductionName { get; set; }
        public string SubEarningDeductionName { get; set; }
        public int CalculationType { get; set; }
        public decimal? EarningDeductionValue { get; set; }
        public int ComponentSequence { get; set; }

        public int Formula_ID { get; set; }
        public bool IStaxable { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string FormulaContent { get; set; }
        public string Formula_Computation { get; set; }

        public int EarningDeductionType { get; set; }
        public string EarningDeductionTypeName
        {
            get
            {
                return Enum.IsDefined(typeof(PayrollHeads), EarningDeductionType)
                    ? ((PayrollHeads)EarningDeductionType).ToString()
                    : "Unknown";
            }
        }
    }

    public class SalarySimulatorDTO : BaseModel
    {
        public decimal ctc { get; set; }
        public int payrollMonth { get; set; }
      
        public List<SalarySimulatorDetailDTO> SalarySimulatorDetails { get; set; }
        public List<SalarySimulatorTotalDTO> SalarySimulatorTotal { get; set; }
    }

    public class SalarySimulatorDetailDTO
    {
        public int SalaryComponentId { get; set; }
        public int PayComponentType { get; set; }
        public string PayComponentName { get; set; }
        public int CalculationType { get; set; }
        public int ComponentSequence { get; set; }
        public int Formula_ID { get; set; }
        public string Formula { get; set; }

        // For returned values.
        public decimal? ComputedValue { get; set; }
        public decimal? Employer_Contribution { get; set; }
        public decimal? Employee_Contribution { get; set; }
    }
    public class SalarySimulatorTotalDTO
    {
        // For returned values
        public decimal? TotalEarnings { get; set; }
        public decimal? TotalDeduction { get; set; }
        public decimal? NetSalary { get; set; }
        public decimal? EmployerContribution { get; set; }
    }
    public class SalaryStructureGrid : BaseModel
    {
        public int SalaryStructure_Hdr_Id { get; set; }
        public string CompanyName { get; set; }
        public string SalaryStructureName { get; set; }
        public string PayGradeName { get; set; }
        public int SalaryFrequency { get; set; }
        public int PayGradeConfig_Id { get; set; }

    }

    public class SalaryStructureApiResponseModel<T>
    {
        public bool IsSuccess { get; set; }

        [JsonProperty("data")]
        public T Result { get; set; }   // ← Map JSON "data" to typed Result

        public string Message { get; set; }
        public int MessageType { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string RedirectUrl { get; set; }
        public object JsonResponse { get; set; }
        public int StatusCode { get; set; }
        public int? returnCount { get; set; }
        public string AuthCode { get; set; }
        public string VerifyUserCode { get; set; }
        public string TemplateType { get; set; }
      
    }
}
