using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class PayComponentDTO : BaseModel
    {
        public int EarningDeduction_Id { get; set; }
        public int Company_Id { get; set; }
        public bool IsEditable { get; set; }
        public string EarningDeductionName { get; set; }
        public bool Is_Child { get; set; }
        public int Parent_EarningDeduction_Id { get; set; }
        public string ParentEarningdeductionName { get; set; }
        public int CalculationType { get; set; }
        public string CalculationTypeName { get; set; }
        public int EarningDeductionType { get; set; }
        public string EarningDeductionTypeName { get; set; }
        public decimal MinimumUnit_value { get; set; }
        public decimal MaximumUnit_value { get; set; }
        public decimal Amount { get; set; }
        public string Formula { get; set; }
        public int Formula_Id { get; set; }
        public string? FormulaName { get; set; } //Added 02-05-2025
    }

    public class PayGradeMasterDTO : BaseModel
    {
        public int PayGrade_Id { get; set; }
        public int? Cmp_Id { get; set; }
        public string PayGradeCode { get; set; }
        public string PayGradeName { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public string PayGradeDesc { get; set; }
    }

    public class PayGradeConfigDTO : BaseModel
    {
        public int PayGradeConfig_Id { get; set; }
        public int Cmp_Id { get; set; }
        public int Correspondance_ID { get; set; }
        public string GradeConfigName { get; set; }
        public int PayGrade_Id { get; set; }
        public int Trade_Id { get; set; }
        public int SkillType_Id { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
        public string PayGradeName { get; set; }
        public string Skillcategory_Name { get; set; }
        public string Trade_Name { get; set; }
        public string EffectiveFromStr { get; set; }
        public string EffectiveToStr { get; set; }
    }
    public class PayComponentActivationDTO : BaseModel
    {
        public int EarningDeduction_Id { get; set; }
        public int Company_Id { get; set; }        
    }
    public class DistinctLocationDTO
    {
        public int Company_ID { get; set; }
        public int Location_ID { get; set; }
        public string LocationName { get; set; }
    }
    public class SkillCategoryDTO : BaseModel
    {
        public int Skillcategory_Id { get; set; }
        public char ExternalUniqueID { get; set; }
        public string Skillcategory_Name { get; set; }
        public int Company_Id { get; set; }
        public int Correspondance_ID { get; set; }
    }
    public class TradeMasterDTO : BaseModel
    {
        public int Trade_mst_Id { get; set; }
        public char ExternaluniqueID { get; set; }
        public string Trade_Name { get; set; }
        public int Company_ID { get; set; }
        public int Company_Location_ID { get; set; }
    }
}
