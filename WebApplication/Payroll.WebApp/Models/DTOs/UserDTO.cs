using Payroll.Common.ApplicationModel;
using UserService.BAL.Models;
using UserService.BAL.Requests;

namespace Payroll.WebApp.Models.DTOs
{
	public class UserDTO : BaseModel
	{
		public int UserId { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int CompanyId { get; set; }
		public int CountryId { get; set; }
		public int RoleId { get; set; }
		public DateTime UserCurrentDateTime { get; set; }
		public DateTime? LinkExpireDate { get; set; }
		public bool IsLinkActivated { get; set; }
		public bool IsActive { get; set; }
		public string UserType { get; set; }
		public int userType_Id { get; set; }
		public string Entity { get; set; }
		public string Salutation { get; set; }
		public string Phone { get; set; }
		public string Username { get; set; }
		public List<string> Companies { get; set; }
		public string Country { get; set; }
		public string State { get; set; }
		public string Branch { get; set; }
		public List<int> Department { get; set; }
		public List<string> Role { get; set; }
		public string PhoneNumber { get; set; }
		public string CountryName { get; set; }
		public List<int> Departments { get; set; }
		public List<int> Roles { get; set; }
		public string City { get; set; }
		public List<int> Correspondance_ID { get; set; } // Matches `Correspondance_ID`
		public List<int> Role_Menu_Header_Id { get; set; } // Matches `Role_Menu_Header_Id`
		public DateTime? EffectiveFromDt { get; set; }
		#region Added By Priyanshi 31 Dec 2024
		public string LocationName { get; set; }
		public string UserTypeName { get; set; }
		#endregion
	}

	//Payroll-387 class added by chirag. use in user activation code.
	public class UserActivationDTO
	{
		public int UserId { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int CompanyId { get; set; }
		public int CountryId { get; set; }
		public int RoleId { get; set; }
		public DateTime UserCurrentDateTime { get; set; }
		public DateTime LinkExpireDate { get; set; }
		public bool IsLinkActivated { get; set; }
		public bool IsActive { get; set; }

	}

    //Payroll-484 class added by Abhishek. - 18-02-2025
    public class UserChangepwdDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }
    public class UserInfoDTO : BaseModel
	{
		public int UserId { get; set; }
		public int userType_Id { get; set; }
		public string Salutation { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string ActivationCode { get; set; }
		public string ActivationLink { get; set; }
		public int CountryId { get; set; }
		public int CompanyId { get; set; }
		public int Company_Id { get; set; }
        
        public int RoleId { get; set; }
		public DateTime UserCurrentDateTime { get; set; }
		public DateTime? LinkExpireDate { get; set; }
		public DateTime? EffectiveFromDt { get; set; } // Aligns with `@Effective_From_Dt`
		public bool IsLinkActivated { get; set; }
		public bool IsActive { get; set; }
		public string UserType { get; set; }
		public string Entity { get; set; }
		public string Phone { get; set; }
		public string Username { get; set; }
		// Table-valued parameter properties
		public List<UserMapLocation> UserMapLocation { get; set; } = new List<UserMapLocation>(); // For `@UserMapLocation_Id`
		public List<UserRoleMapping> MapUserRole { get; set; } = new List<UserRoleMapping>(); // For `@Mapuserrole`
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
		public string RoleName { get; set; }
		public int PasswordExpiryReminderDays { get; set; }
		public DateTime? NextPasswordChangeDate { get; set; }
		public int Deactivation_Type { get; set; }
		public string Deactivation_Reason { get; set; }
		public DateTime? EffectiveOn { get; set; }

		#region Added By Harshida 23-12-'24
		public string ContactNo { get; set; }
		public string UserFullName { get; set; }
		public List<UserCompanyDetailsDTO> CompanyDetails { get; set; } = new List<UserCompanyDetailsDTO>();
		public List<UserLocationDetailsDTO> LocationDetails { get; set; } = new List<UserLocationDetailsDTO>();
		public List<UserRoleDetailsDTO> RoleDetails { get; set; } = new List<UserRoleDetailsDTO>();
		#endregion
		public string CompanyName { get; set; } //Added By Harshida 30-12-'24 
		#region Added By Priyanshi 31 Dec 2024
		public string LocationName { get; set; }
		public string UserTypeName { get; set; }
		public string Country { get; set; }
		public string City { get; set; }
		public string CityIds { get; set; }
        public string StateIds { get; set; }
        //public int City_Id { get; set; }
        public string StateName { get; set; }
		#endregion

		public List<UserRequest> userDetails { get; set; } = new List<UserRequest>(); // payroll-377 krunali
		public List<UserRoleMapping> maproleDetails { get; set; } = new List<UserRoleMapping>(); //payroll-377 krunali
	}
	#region Added By Harshida 23-12-'24
	public class UserCompanyDetailsDTO
	{
		public int Company_Id { get; set; }
		public int UserMapCompany_Id { get; set; }
		public string CompanyName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string FinYear { get; set; }
		public int Currency_Id { get; set; }

	}
	public class UserLocationDetailsDTO
	{
		public int UserMapLocation_Id { get; set; }
		public int Location_ID { get; set; }
		public string LocationName { get; set; }
		public string CompanyFullLocation { get; set; }
		public bool Default_Location { get; set; }
		public string Primary_Email_Id { get; set; }
		public string Primary_Phone_No { get; set; }
	}
	public class UserRoleDetailsDTO
	{
		public int Role_User_Id { get; set; }
		public bool IsDefault_Role { get; set; }
		public int Role_Id { get; set; }
		public string RoleName { get; set; }
	}
	#endregion
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

}
