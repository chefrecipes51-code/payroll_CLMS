using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.CommonDto
{
    public class AuthConfigModel 
    {
        public int Auth_Id { get; set; }
        public int PasswordAttempts { get; set; }  
        public bool CheckPasswordHistory { get; set; }  
        public int RePeatPasswordDuration { get; set; }  
        public int PasswordExpiryDays { get; set; }
        public int PasswordExpiryReminderDays { get; set; }  
        public bool TwoFactorAuth { get; set; }
        public int AuthFactorType { get; set; }  
        public int PasswordMinLength { get; set; }  
        public int PasswordMaxLength { get; set; }   
        public bool HasSpecialCharacter { get; set; }  
        public int NumberOfSpecialCharacters { get; set; }
        public bool StartWithCharType { get; set; }  
        public bool EndWithNumType { get; set; }  
        public int ExcludeSequence { get; set; }  
        public int NumberOfCharacters { get; set; }  
        public int NumberOfDigits { get; set; }  
        public DateTime CreatedDate { get; set; }  
        public int UpdatedBy { get; set; }  
        public DateTime UpdatedDate { get; set; }
        public int OTPStatus { get; set; }
        public string ApplicationMessage { get; set; }
        public int ApplicationMessageType { get; set; }
        public int IsSuccessType { get; set; }
    }
}
