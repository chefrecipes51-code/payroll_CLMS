namespace Payroll.WebApp.Models.DTOs
{
    public class CompanyTypeDTO
    {
        public byte CompanyType_ID { get; set; } // Using byte for tinyint
        public string CompanyType_Name { get; set; }
    }
}
