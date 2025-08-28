using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.ApplicationConstant
{
    public class MessageConstants
    {
        // Add more Application-Message constants here if needed

        #region Common

        public const string Mantra = "Mantra Softech India Pvt Ltd";

        public const string TechnicalIssue = "Oops! Something went wrong. Please try again.";
        public const string InvalidCaptcha = "Captcha is incorrect.";
        public const string OTPAlreadySend = "An OTP has already been sent to your email.";

        public const string DataNotFound = "Data Not Found !";
        public const string DataLoadFailed = "Data Load Failed !";

        public const string PasswordInfoMessage = "Invalid credential.<br> Please enter carefully this is your last attempt.";
        public const string AccountInactiveTemporary = "Too many incorrect attempts. your account is locked, please try again after some time.";

        public const string InvalidLoginRequest = "Invalid login request.";
        public const string EmailSendSuccessfully = "Email send successfully.";

        public const string OTPMatchSuccessfully = "OTP match successfully.";
        public const string OTPInvalid = "OTP is not valid, please fill correct.";

        public const string UserNotFound = "No user found with this email. Please check your email.";

        public const string PasswordChangeFailed = "Password change failed. Please try again later.";
        public const string PasswordChangeSuccess = "Password changed successfully.";

        public const string InActiveAccount = "Your account is inactive. Please contact the administrator.";


        public const string PINInvalid = "PIN is not valid, please fill correct.";
        public const string PINMatchSuccessfully = "PIN match successfully.";
        public const string AccountBlockedMessage = "Your account has been blocked. Please contact the administrator for assistance.";


        public const int AccountActivationTemplateType = 4;
        #endregion

        public const string RecordDeleted = "Record deleted successfully"; //Added By Harshida (01-01-'25)
        public const string LocationUpdated = "Default Location updated successfully.";//Added By Harshida (21-01-'25)
        public const string RoleUpdated = "Default Role updated successfully.";//Added By Harshida (21-01-'25)
        public const string RecordNotFoundInAPI = "Record not found.";
        public const string UserRoleLocationNotFound = "Contact to Network Administrator";
    }
}
