using System.ComponentModel.DataAnnotations;
using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class EntityTaxStatutoryDTO :BaseModel
    {
        public int Entity_Statutory_Id { get; set; }
        public int Employee_Id { get; set; }

        [Required]
        public string PayrollNo { get; set; }

        public byte Company_Id { get; set; }
        public int? PF_No { get; set; }
        public decimal? PF_Employer_Contribution { get; set; }
        public decimal? PF_Employee_Contribution_ { get; set; }
        public string? ESIC_No { get; set; }
        public decimal? ESIC_Employer_Contribution { get; set; }
        public decimal? ESIC_Employee_Contribution { get; set; }
        public decimal? Professional_Tax { get; set; }
        public bool? Is_Gratuity_Eligibility { get; set; }
        public string? Gratuity_Account_No { get; set; }
        public string? TIN_No { get; set; }
    }
}
