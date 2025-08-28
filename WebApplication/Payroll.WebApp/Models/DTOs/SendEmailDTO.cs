namespace Payroll.WebApp.Models.DTOs
{
    public class SendEmailDTO
    {
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public int TemplateType { get; set; }
        public int OTP { get; set; }
        public string NewPassword { get; set; }
    }
}
