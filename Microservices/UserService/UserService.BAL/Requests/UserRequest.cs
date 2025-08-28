using Payroll.Common.ApplicationModel;
using UserService.BAL.Models;

namespace UserService.BAL.Requests
{
    public class UserRequest : BaseModel
    {
        public int UserId { get; set; }
        public int userType_Id { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string ActivationCode { get; set; }
        public string ActivationLink { get; set; }
        public DateTime? LinkExpireDate { get; set; } //payroll-384 chirag gurjar, resolve error in user activation link module by adding this line.
        public int CountryId { get; set; }
        public int CompanyId { get; set; }
        public int Company_Id { get; set; }
        public DateTime? EffectiveFromDt { get; set; } // Aligns with `@Effective_From_Dt`

        // Table-valued parameter properties
        public List<UserMapLocation> UserMapLocation { get; set; } = new List<UserMapLocation>(); // For `@UserMapLocation_Id`
        public List<UserRoleMapping> MapUserRole { get; set; } = new List<UserRoleMapping>(); // For `@Mapuserrole`
        public List<UserRequest> userDetails { get; set; } = new List<UserRequest>();
        public List<UserRoleMapping> maproleDetails { get; set; } = new List<UserRoleMapping>();
    
        public List<int> Correspondance_ID { get; set; } // Matches `Correspondance_ID`
        public List<int> Role_Menu_Header_Id { get; set; } // Matches `Role_Menu_Header_Id`
        // Other properties (optional or not directly referenced in the AddAsync method)
        
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
        public bool IsLinkActivated { get; set; }
        public string RoleName { get; set; }
        public DateTime UserCurrentDateTime { get; set; }
        public int PasswordExpiryReminderDays { get; set; }
        public DateTime? NextPasswordChangeDate { get; set; }
        public int Deactivation_Type { get; set; }
        public string Deactivation_Reason { get; set; }
        public DateTime? EffectiveOn { get; set; }

        #region Added By Harshida 23-12-'24
        public string ContactNo { get; set; }
        public string UserFullName { get; set; }
        public List<UserCompanyDetails> CompanyDetails { get; set; } = new List<UserCompanyDetails>();
        public List<UserLocationDetails> LocationDetails { get; set; } = new List<UserLocationDetails>();
        public List<UserRoleDetails> RoleDetails { get; set; } = new List<UserRoleDetails>();
        public string CompanyName { get; set; } //Added By Harshida 30-12-'24
        #endregion
        #region Added By Priyanshi 31 Dec 2024
        public string UserTypeName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string CityIds { get; set; }
        public string StateIds { get; set; }
        //public int  City_Id { get; set; }
        public string StateName { get; set; }
        public string LocationName { get; set; }

        public static implicit operator UserRequest((IEnumerable<UserRequest>, IEnumerable<UserRoleMapping>) v)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region added by Abhishek 13-02-2025 
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        #endregion
    }

    #region Added By Harshida 23-12-'24
    public class UserCompanyDetails 
    {
        public int Company_Id { get; set; }
        public int UserMapCompany_Id { get; set; }
        public string CompanyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FinYear { get; set; }
        public int Currency_Id { get; set; }

    }
    public class UserLocationDetails 
    {
        public int UserMapLocation_Id { get; set; }
        public int Location_ID { get; set; }
        public string LocationName { get; set; }
        public string CompanyFullLocation { get; set; }
        public bool Default_Location { get; set; }
        public string Primary_Email_Id { get; set; }
        public string Primary_Phone_No { get; set; }
    }
    public class UserRoleDetails
    {
        public int Role_User_Id { get; set; }
        public bool IsDefault_Role { get; set; }
        public int Role_Id { get; set; }
        public string RoleName { get; set; }
    }
    public class UserHeaderRoleDetails
    {
        public int Role_Menu_Hdr_Id { get; set; }
        public string RoleName { get; set; }
    }
    public class UserCompanyRoleLocation
    {
        public int UserId { get; set; }
     
        public IEnumerable<UserRoleDetails> RoleDetails { get; set; }
        public UserCompanyRoleLocation()
        { 
            RoleDetails = new List<UserRoleDetails>();
            //UserRoleHeaderDetails = new List<UserHeaderRoleDetails>();
        }
    }

    #endregion

    #region Added by Priyanshi For UserCreation Mapping
    public class UserMapLocation
    {
        public int UserMapLocation_Id { get; set; }
        public int User_ID { get; set; }
        public byte Company_Id { get; set; }
        public int Correspondance_ID { get; set; }
        public bool IsUserMapToLowLevel { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
    #endregion
}
