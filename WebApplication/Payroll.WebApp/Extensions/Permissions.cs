using Newtonsoft.Json;

namespace Payroll.WebApp.Extensions
{
    public enum PermissionIds
    {
        NotSet = 0, //error condition
        CompanyPage = 1,
        UserManagement = 2,
        Reports_View = 3

    }

    public class UserInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<int> Permissions { get; set; } // Store permissions as integers
    }

    public class ApiResponse
    {
        public int statusCode { get; set; }
        public string message { get; set; }

        public ApiResponse(int statusCode, string message)
        {
            this.statusCode = statusCode;
            this.message = message;
        }

        public ApiResponse()
        {

        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
