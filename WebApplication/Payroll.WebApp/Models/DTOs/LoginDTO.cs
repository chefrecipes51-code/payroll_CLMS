namespace Payroll.WebApp.Models.DTOs
{
    public class LoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Captcha { get; set; }
        public bool RememberMe { get; set; }
        public string OTP { get; set; }
        public string Email { get; set; }    //Note:- use in forgot password.
        public int RoleId { get; set; }     //Note:- use in role selection if user have multiple roles.
    }
}
