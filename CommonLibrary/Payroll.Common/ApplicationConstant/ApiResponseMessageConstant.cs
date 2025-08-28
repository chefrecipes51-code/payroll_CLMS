using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.ApplicationConstant
{
    public class ApiResponseMessageConstant
    {
        /* Data is common word that work for all page */

        #region Data

        public const string GetRecord = "Record retrieved successfully.";
        public const string FetchAllRecords = "Records retrieved successfully.";
        public const string DeleteSuccessfully = "Record deleted successfully.";
        public const string RecordNotFound = "Record not found.";
        public const string CreatedSuccessfully = "Record created successfully.";
        public const string UpdateSuccessfully = "Record updated successfully.";
        public const string NullData = "Data is null.";
        public const string InvalidData = "Data is invalid.";
        public const string TechnicalIssue = "TechnicalIssue.";
        public const string OTPMatchSuccessfully = "OTP match successfully.";
        public const string OTPInvalid = "OTP is not valid, please fill correct.";
        public const string EmailSendSuccessfully = "Email send successfully.";
        public const string UpdateFailed = "Failed to updated record.";
        public const string PreviousPasswordMatch = "You cannot use your old password as the new password";
        public const string InvalidRequest = "Request is invalid.";
        public const string InvalidApiKey = "Request is invalid.";

        public const string SomethingWrong = "Something Went Wrong."; //Added By Harshida 20-01-25
        public const string NoIssues = "Email does not Exist."; //Added By Harshida 21-01-25 As SP return Either No Issue Or Already Exist

        #region For Email Validation TWO Variable Created
            //Added By Harshida 22-01-25 As SP return Either No Issue Or Already Exist
            public const string EmailalreadyConflict = "Email Already Exists.";       
        #endregion
        #endregion

    }
}
