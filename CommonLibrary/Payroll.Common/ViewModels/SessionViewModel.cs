namespace Payroll.Common.ViewModels
{
    public class SessionViewModel
    {
        public string UserId { get; set; } // Note writeen By Harshida:- Rohit has made these field as STRING I Don;t know why???? 
        public string Username { get; set; }
        public string Pin { get; set; }
        public string Token { get; set; }
        public string Captcha { get; set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(Username);
        public object GetObjectFromJson<T>(string v)
        {
            throw new NotImplementedException();
        }
    }

    #region Added By Harshida
    public class UserSessionViewModel
    {
        public List<UserCompanyDetails> CompanyDetails { get; set; } = new List<UserCompanyDetails>();
        public List<UserLocationDetails> LocationDetails { get; set; } = new List<UserLocationDetails>();
        public List<UserRoleDetails> RoleDetails { get; set; } = new List<UserRoleDetails>();
        public string? ProfilePath { get; set; }
    }
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
    #endregion
}
