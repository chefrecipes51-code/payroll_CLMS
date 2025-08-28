using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class LoginHistory : BaseModel
    {
        public long LogInHistoryId { get; set; }
        public int UserId { get; set; }
        public DateTime LogInTime { get; set; }
        public DateTime? LogOutTime { get; set; }
        public bool? IsLoggedOut { get; set; }
        public string IpAddress { get; set; }
        public string OperationType { get; set; }
    }
    public class LoginHistoryRequestModel : BaseModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool DbStatus { get; set; }       
    }
}
