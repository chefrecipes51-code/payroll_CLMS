namespace Payroll.WebApp.Models.DTOs
{
    public class EntityDataValidationDTO
    {
        public int DataValidation_ID { get; set; }
        public int Module_ID { get; set; }
        public string Validation_Control_Name { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
