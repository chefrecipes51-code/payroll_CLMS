using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Requests
{
    public class CLMSLoginRequest
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserStatus { get; set; }
    }
    public class CLMSLoginResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }
        public string? RedirectUrl { get; set; }
    }
}
