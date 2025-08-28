using Payroll.Common.ApplicationModel;
using Payroll.Common.CommonDto;
using RoleService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class UserMasterModel : BaseModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProfileImagePath { get; set; }
        public bool RememberMe { get; set; }
        public DateTime? ChangePasswordDate { get; set; }
        public bool ForgotPassword { get; set; }
        public DateTime? ForgotPasswordDate { get; set; }
        public byte[] OTP { get; set; }
        public DateTime? OTPExpiration { get; set; }
        public string AuthType { get; set; }
        public bool PayrollConfigAuthType { get; set; }
        public string Pin { get; set; }
        public bool IsScreenLocked { get; set; }
        public bool IsUserLogin { get; set; }
        public int MaxAttempts { get; set; }
        public DateTime? LockTime { get; set; }
        public bool LockAccount { get; set; }
        public string ActivationCode { get; set; }
        public string ActivationLink { get; set; }
        public DateTime? LinkExpireDate { get; set; }
        public string RoleName { get; set; }
        public DateTime CurrentDateTime { get; set; }
        public int PasswordExpiryReminderDays { get; set; }
        public DateTime NextPasswordChangeDate { get; set; }
        public int CountryId { get; set; }
        public int CompanyId { get; set; }
        public int Deactivation_Type { get; set; }
        public string Deactivation_Reason { get; set; }
        public DateTime? EffectiveOn { get; set; }

        //public List<RoleMasterModel> Roles { get; set; }
        //public string Ipaddress { get; set; }
    }
    //UserListModel added by chirag Payroll-529
    public class UserListModel : BaseModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
